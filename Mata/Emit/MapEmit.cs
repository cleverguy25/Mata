// <copyright file="MapEmit.cs" company="Palador Open Source">
// Copyright (c) Palador Open Source. All rights reserved. See License.txt in the project root for license information.
// </copyright>

namespace Mata.Emit
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Reflection.Emit;
    using Mata;

    public class MapEmit<T>
    {
        private const string TypeNamespace = "SqlDataReaderMap";

        private readonly MapDefinition<T> mapDefinition;

        private readonly MapEmitDebugInfo<T> mapEmitDebugInfo;

        private ConstructorBuilder defaultConstructor;

        private Dictionary<string, FieldInfo> fieldOrdinals;

        private MethodBuilder loadMethod;

        private MethodBuilder loadOrdinalsMethod;

        private TypeBuilder typeBuilder;

        public MapEmit(MapDefinition<T> mapDefinition)
        {
            this.mapDefinition = mapDefinition;
            if (MapEmitAssembly.EmitDebugSymbols)
            {
                this.mapEmitDebugInfo = new MapEmitDebugInfo<T>(TypeNamespace, this.GetTypeName(), this.GetClassName());
            }
        }

        public Func<IMap<T>> Generate()
        {
            this.mapEmitDebugInfo?.CreateDebugSymbols();
            this.CreateType();
            this.CreateFieldOrdinals();
            this.CreateDefaultConstructor();
            this.CreateLoadOrdinalsMethod();
            this.CreateLoadMethod();
            this.ImplementInterface();
            var type = this.typeBuilder.CreateType();
            this.mapEmitDebugInfo?.AddDebugEndFile();
            return GenerateFactoryMethod(type);
        }

        private static Func<IMap<T>> GenerateFactoryMethod(Type type)
        {
            var newObject = Expression.New(type);
            var castToMap = Expression.Convert(newObject, typeof(IMap<T>));
            return Expression.Lambda<Func<IMap<T>>>(castToMap).Compile();
        }

        private static MethodInfo GetReaderGetDataMethodForField(FieldMapDefinition field)
        {
            Dictionary<Type, MethodInfo> dictionary;
            if (field.AllowNulls)
            {
                dictionary = field.DefaultValue == null ? MapEmitAssembly.NullableGetMethods : MapEmitAssembly.NullableGetWithDefaultMethods;
            }
            else
            {
                dictionary = MapEmitAssembly.DataRecordGetMethods;
            }

            var getMethod = dictionary[field.DestinationProperty.PropertyType];
            return getMethod;
        }

        private static void EmitDefaultValueConstant(ILGenerator code, FieldMapDefinition field)
        {
            if (field.DefaultValue == null)
            {
                return;
            }

            var defaultValueType = field.DefaultValue.GetType();
            if (defaultValueType == typeof(bool))
            {
                if ((bool)field.DefaultValue)
                {
                    code.Emit(OpCodes.Ldc_I4_1);
                }
                else
                {
                    code.Emit(OpCodes.Ldc_I4_0);
                }
            }
            else if (defaultValueType == typeof(byte))
            {
                code.Emit(OpCodes.Ldc_I4, (int)(byte)field.DefaultValue);
            }
            else if (defaultValueType == typeof(short))
            {
                code.Emit(OpCodes.Ldc_I4, (int)(short)field.DefaultValue);
            }
            else if (defaultValueType == typeof(int))
            {
                code.Emit(OpCodes.Ldc_I4, (int)field.DefaultValue);
            }
            else if (defaultValueType == typeof(long))
            {
                code.Emit(OpCodes.Ldc_I8, (long)field.DefaultValue);
            }
            else if (defaultValueType == typeof(char))
            {
                code.Emit(OpCodes.Ldc_I4_S, (char)field.DefaultValue);
            }
            else if (defaultValueType == typeof(string))
            {
                code.Emit(OpCodes.Ldstr, (string)field.DefaultValue);
            }
            else if (defaultValueType == typeof(float))
            {
                code.Emit(OpCodes.Ldc_R4, (float)field.DefaultValue);
            }
            else if (defaultValueType == typeof(double))
            {
                code.Emit(OpCodes.Ldc_R8, (double)field.DefaultValue);
            }
        }

        private static void WrapWithNullableIfNeeded(ILGenerator code, FieldMapDefinition field)
        {
            var propertyType = field.DestinationProperty.PropertyType;
            if (field.DefaultValue == null || propertyType.IsNullableType() == false)
            {
                return;
            }

            var constructor = MapEmitAssembly.SupportedConstantType[propertyType.GetGenericArguments()[0]];
            code.Emit(OpCodes.Newobj, constructor);
        }

        private void CreateType()
        {
            var typeName = this.GetTypeName();
            this.typeBuilder = MapEmitAssembly.ModuleBuilder.DefineType(
                typeName,
                TypeAttributes.Public);

            this.typeBuilder.AddInterfaceImplementation(typeof(IMap<T>));

            this.mapEmitDebugInfo?.AddDebugTypeDeclaration();
        }

        private string GetTypeName()
        {
            return $"{TypeNamespace}.{this.GetClassName()}";
        }

        private string GetClassName()
        {
            return typeof(T).Name + this.mapDefinition.UniqueId.Replace("-", string.Empty);
        }

        private void CreateDefaultConstructor()
        {
            this.defaultConstructor = this.typeBuilder.DefineConstructor(
                MethodAttributes.Public,
                CallingConventions.Standard,
                Type.EmptyTypes);

            this.EmitConstructor();
        }

        private void EmitConstructor()
        {
            this.mapEmitDebugInfo?.AddDebugConstructorDeclaration();
            var code = this.defaultConstructor.GetILGenerator();

            this.mapEmitDebugInfo?.AddDebugEndMethod(code);
            code.Emit(OpCodes.Ret);
        }

        private void CreateFieldOrdinals()
        {
            this.fieldOrdinals = new Dictionary<string, FieldInfo>();
            foreach (var field in this.mapDefinition.FieldMapDefinitions)
            {
                var sourceColumn = field.Value.SourceColumn;
                var fieldInfo = this.typeBuilder.DefineField(
                    sourceColumn,
                    typeof(int),
                    FieldAttributes.Public);

                this.fieldOrdinals[sourceColumn] = fieldInfo;
            }
        }

        private void ImplementInterface()
        {
            var loadOrdinalsInterface = typeof(IMap<T>).GetMethod("LoadOrdinals");
            this.typeBuilder.DefineMethodOverride(this.loadOrdinalsMethod, loadOrdinalsInterface);

            var loadInterface = typeof(IMap<T>).GetMethod("Load");
            this.typeBuilder.DefineMethodOverride(this.loadMethod, loadInterface);
        }

        private void CreateLoadOrdinalsMethod()
        {
            var parameterTypes = new[] { typeof(IDataRecord) };
            this.loadOrdinalsMethod = this.DefineVoidMethod("LoadOrdinals", parameterTypes);

            this.loadOrdinalsMethod.DefineParameter(1, ParameterAttributes.None, "reader");
            this.EmitLoadOrdinals();
        }

        private void CreateLoadMethod()
        {
            var parameterTypes = new[] { typeof(T), typeof(IDataRecord) };
            this.loadMethod = this.DefineVoidMethod("Load", parameterTypes);

            this.loadMethod.DefineParameter(1, ParameterAttributes.None, "model");
            this.loadMethod.DefineParameter(2, ParameterAttributes.None, "reader");

            this.EmitLoadMethod();
        }

        private MethodBuilder DefineVoidMethod(string methodName, Type[] parameterTypes)
        {
            return this.typeBuilder.DefineMethod(
                methodName,
                MethodAttributes.Public | MethodAttributes.NewSlot | MethodAttributes.Virtual | MethodAttributes.Final,
                typeof(void),
                parameterTypes);
        }

        private void EmitLoadMethod()
        {
            this.mapEmitDebugInfo?.AddDebugLoadDeclaration();
            var code = this.loadMethod.GetILGenerator();
            foreach (var field in this.mapDefinition.FieldMapDefinitions)
            {
                this.EmitGetValue(code, field.Value);
            }

            this.mapEmitDebugInfo?.AddDebugEndMethod(code);
            code.Emit(OpCodes.Ret);
        }

        private void EmitGetValue(ILGenerator code, FieldMapDefinition field)
        {
            var ordinalField = this.fieldOrdinals[field.SourceColumn];
            var getMethod = GetReaderGetDataMethodForField(field);

            this.mapEmitDebugInfo?.AddDebugGetValue(code, field, getMethod, ordinalField);
            code.Emit(OpCodes.Ldarg_1);
            code.Emit(OpCodes.Ldarg_2);
            code.Emit(OpCodes.Ldarg_0);
            code.Emit(OpCodes.Ldfld, ordinalField);

            EmitDefaultValueConstant(code, field);

            code.EmitCall(field.AllowNulls ? OpCodes.Call : OpCodes.Callvirt, getMethod, null);

            WrapWithNullableIfNeeded(code, field);

            code.EmitCall(OpCodes.Callvirt, field.DestinationProperty.GetSetMethod(), null);
        }

        private void EmitLoadOrdinals()
        {
            this.mapEmitDebugInfo?.AddDebugLoadOrdinalsDeclaration();
            var code = this.loadOrdinalsMethod.GetILGenerator();

            var getOrdinalMethod = typeof(IDataRecord).GetMethod("GetOrdinal");

            foreach (var fieldOrdinal in this.fieldOrdinals)
            {
                this.EmitGetOrdinal(code, fieldOrdinal.Key, getOrdinalMethod, fieldOrdinal.Value);
            }

            this.mapEmitDebugInfo?.AddDebugEndMethod(code);
            code.Emit(OpCodes.Ret);
        }

        private void EmitGetOrdinal(
            ILGenerator code,
            string sourceColumn,
            MethodInfo getOrdinalMethod,
            FieldInfo ordinalProperty)
        {
            this.mapEmitDebugInfo?.AddDebugGetOrdinal(code, ordinalProperty, sourceColumn);
            code.Emit(OpCodes.Ldarg_0);
            code.Emit(OpCodes.Ldarg_1);
            code.Emit(OpCodes.Ldstr, sourceColumn);
            code.EmitCall(OpCodes.Callvirt, getOrdinalMethod, null);
            code.Emit(OpCodes.Stfld, ordinalProperty);
        }
    }
}