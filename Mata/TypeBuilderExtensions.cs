// <copyright file="TypeBuilderExtensions.cs" company="Palador Open Source">
//   Copyright (c) Palador Open Source. All rights reserved. See License.txt in the project root for license information.
// </copyright>

#if NETSTANDARD1_6
namespace Mata
{
    using System;
    using System.Reflection.Emit;

    public static class TypeBuilderExtensions
    {

        public static Type CreateType(this TypeBuilder typeBuilder)
        {
            var typeInfo = typeBuilder.CreateTypeInfo();
            return typeInfo.AsType();
        }
    }
}
#endif