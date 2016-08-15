// <copyright file="TypeExtensions.cs" company="Palador Open Source">
//   Copyright (c) Palador Open Source. All rights reserved. See License.txt in the project root for license information.
// </copyright>

namespace Mata
{
    using System;
    using System.Reflection;

    public static class TypeExtensions
    {
        public static bool DoesPropertySupportNull(this Type propertyType)
        {
            if (propertyType == typeof(string))
            {
                return true;
            }

            return propertyType.IsNullableType();
        }

        public static bool IsNullableType(this Type propertyType)
        {
#if NETSTANDARD1_6
            return propertyType.GetTypeInfo().IsGenericType &&
                   propertyType.GetGenericTypeDefinition() == typeof(Nullable<>);
#else
            return propertyType.IsGenericType &&
                   propertyType.GetGenericTypeDefinition() == typeof(Nullable<>);
#endif
        }

        public static bool IsValueType(this Type propertyType)
        {
#if NETSTANDARD1_6
            return propertyType.GetTypeInfo().IsValueType;
#else
            return propertyType.IsValueType;
#endif
        }
    }
}