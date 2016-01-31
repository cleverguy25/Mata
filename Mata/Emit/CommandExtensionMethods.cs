// <copyright file="CommandExtensionMethods.cs" company="Palador Open Source">
//   Copyright (c) Palador Open Source. All rights reserved. See License.txt in the project root for license information.
// </copyright>

namespace Mata.Emit
{
    using System.Reflection;

    public class CommandExtensionMethods
    {
        public CommandExtensionMethods()
        {
            var type = typeof(DbCommandExtensions);
            this.DeriveParameters = type.GetMethod("DeriveParameters");
            this.SetParameter = type.GetMethod("SetParameter");
            this.SetParameterOrDbNull = type.GetMethod("SetParameterOrDbNull");
            this.AddParameter = type.GetMethod("AddParameter");
            this.AddParameterOrDbNull = type.GetMethod("AddParameterOrDbNull");
        }

        public MethodInfo DeriveParameters { get; }

        public MethodInfo SetParameter { get; }

        public MethodInfo SetParameterOrDbNull { get; }

        public MethodInfo AddParameter { get; }

        public MethodInfo AddParameterOrDbNull { get; }
    }
}