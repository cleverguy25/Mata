// <copyright file="DbCommandExtensions.cs" company="Palador Open Source">
//   Copyright (c) Palador Open Source. All rights reserved. See License.txt in the project root for license information.
// </copyright>

namespace Mata
{
    using System;
    using System.Data;
    using System.Data.Common;

    public static class DbCommandExtensions
    {
        private static readonly Lazy<ParameterCache> ParameterCacheInstance = new Lazy<ParameterCache>();

        public static ParameterCache ParameterCache => ParameterCacheInstance.Value;

        public static ParameterSet DeriveParameters(this DbCommand command)
        {
            return ParameterCache.DeriveParameters(command);
        }

        public static void SetParameter(this IDbCommand command, ParameterSet parameters, string parameterName, object value)
        {
            parameters.SetParameter(command, parameterName, value);
        }

        public static void SetParameterOrDbNull(this IDbCommand command, ParameterSet parameters, string parameterName, object value)
        {
            command.SetParameter(parameters, parameterName, value ?? DBNull.Value);
        }

        public static void AddParameter(this IDbCommand command, string parameterName, object value)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = parameterName;
            parameter.Value = value;
            command.Parameters.Add(parameter);
        }

        public static void AddParameterOrDbNull(this IDbCommand command, string parameterName, object value)
        {
            command.AddParameter(parameterName, value ?? DBNull.Value);
        }
    }
}