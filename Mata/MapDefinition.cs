// <copyright file="MapDefinition.cs" company="Palador Open Source">
// Copyright (c) Palador Open Source. All rights reserved. See License.txt in the project root for license information.
// </copyright>

namespace Mata
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq.Expressions;
    using System.Reflection;
    using Emit;

    public class MapDefinition<T>
    {
        private readonly Lazy<Func<IMap<T>>> create;

        public MapDefinition(bool deriveParameters = true)
        {
            this.DeriveParameters = deriveParameters;
            this.create = new Lazy<Func<IMap<T>>>(this.GenerateMapCreationFunction);
        }

        public bool DeriveParameters { get; private set; }

        public string UniqueId { get; } = Guid.NewGuid().ToString();

        public Dictionary<MemberInfo, FieldMapDefinition> FieldMapDefinitions { get; } =
            new Dictionary<MemberInfo, FieldMapDefinition>();

        public void MapType()
        {
            foreach (var propertyInfo in typeof(T).GetProperties())
            {
                var allowNulls = propertyInfo.PropertyType.DoesPropertySupportNull();

                this.Map(propertyInfo, propertyInfo.Name, allowNulls, null);
            }
        }

        public void Map(Expression<Func<T, object>> property)
        {
            var destinationProperty = GetProperty(property);
            var allowNulls = destinationProperty.PropertyType.DoesPropertySupportNull();
            this.Map(destinationProperty, destinationProperty.Name, allowNulls, null);
        }

        public void Map(Expression<Func<T, object>> property, bool allowNulls, object defaultValue = null)
        {
            this.Map(property, null, allowNulls, defaultValue);
        }

        public void Map(Expression<Func<T, object>> property, string sourceColumn, bool allowNulls = false, object defaultValue = null)
        {
            var destinationProperty = GetProperty(property);
            this.Map(destinationProperty, sourceColumn, allowNulls, defaultValue);
        }

        public IMap<T> CreateMap()
        {
            return this.create.Value();
        }

        private static PropertyInfo GetProperty<TValue>(Expression<Func<T, TValue>> selector)
        {
            var lambda = selector as LambdaExpression;
            var unaryExpression = lambda?.Body as UnaryExpression;
            var operand = unaryExpression?.Operand;

            var member = operand ?? lambda?.Body;
            if (member?.NodeType == ExpressionType.MemberAccess)
            {
                return (PropertyInfo)((MemberExpression)member).Member;
            }

            throw new InvalidOperationException();
        }

        private static void CheckDefaultValuePreConditions(
            PropertyInfo destinationProperty,
            bool allowNulls,
            object defaultValue)
        {
            if (defaultValue == null)
            {
                return;
            }

            CheckIfDefaultValueAndAllowNullsMatch(destinationProperty, allowNulls);

            CheckIfDefaultValueTypeIsCompatibleWithDestinationProperty(destinationProperty, defaultValue);

            CheckTypeIsSupportedForDefaultValue(destinationProperty);
        }

        private static void CheckTypeIsSupportedForDefaultValue(PropertyInfo destinationProperty)
        {
            var innerType = GetInnerType(destinationProperty);
            if (MapEmitAssembly.SupportedConstantType.ContainsKey(innerType) == false)
            {
                throw new InvalidOperationException(
                    $"Binding for property [{destinationProperty.Name}] has type [{destinationProperty.PropertyType.Name}] is not supported as a default value.");
            }
        }

        private static void CheckIfDefaultValueTypeIsCompatibleWithDestinationProperty(PropertyInfo destinationProperty, object defaultValue)
        {
            var innerType = GetInnerType(destinationProperty);

            if (innerType != defaultValue.GetType())
            {
                throw new ArgumentException(
                    nameof(defaultValue),
                    $"Type of defaultValue [{defaultValue.GetType()}] is not compatible with property [{destinationProperty.Name}] type of [{destinationProperty.PropertyType.Name}].");
            }
        }

        private static Type GetInnerType(PropertyInfo destinationProperty)
        {
            var innerType = destinationProperty.PropertyType;
            if (destinationProperty.PropertyType.IsNullableType())
            {
                innerType = destinationProperty.PropertyType.GetGenericArguments()[0];
            }

            return innerType;
        }

        private static void CheckIfDefaultValueAndAllowNullsMatch(
            PropertyInfo destinationProperty,
            bool allowNulls)
        {
            if (allowNulls == false)
            {
                throw new InvalidOperationException(
                    $"Binding for property [{destinationProperty.Name}] sets a default value even though nulls are not allowed.");
            }
        }

        private void Map(PropertyInfo destinationProperty, string sourceColumn, bool allowNulls, object defaultValue)
        {
            CheckDefaultValuePreConditions(destinationProperty, allowNulls, defaultValue);

            MapEmitAssembly.CheckValidType(destinationProperty);
            var fieldMapDefinition = new FieldMapDefinition(
                                             destinationProperty,
                                             sourceColumn ?? destinationProperty.Name,
                                             allowNulls,
                                             defaultValue);

            this.FieldMapDefinitions[destinationProperty] = fieldMapDefinition;
        }

        private Func<IMap<T>> GenerateMapCreationFunction()
        {
            var generator = new MapEmit<T>(this);
            return generator.Generate();
        }
    }
}
