// <copyright file="FieldMapDefinition.cs" company="Palador Open Source">
// Copyright (c) Palador Open Source. All rights reserved. See License.txt in the project root for license information.
// </copyright>

namespace MataCore
{
    using System.Reflection;

    public class FieldMapDefinition
    {
        public FieldMapDefinition(
            PropertyInfo destinationProperty,
            string sourceColumn,
            bool allowNulls,
            object defaultValue,
            bool isSqlServerSpecific)
        {
            this.DestinationProperty = destinationProperty;
            this.SourceColumn = sourceColumn;
            this.AllowNulls = allowNulls;
            this.DefaultValue = defaultValue;
            this.IsSqlServerSpecific = isSqlServerSpecific;
        }

        public PropertyInfo DestinationProperty { get; }

        public string SourceColumn { get; }

        public bool AllowNulls { get; }

        public object DefaultValue { get; }

        public bool IsSqlServerSpecific { get; }
    }
}