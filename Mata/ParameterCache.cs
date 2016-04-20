// <copyright file="ParameterCache.cs" company="Palador Open Source">
//   Copyright (c) Palador Open Source. All rights reserved. See License.txt in the project root for license information.
// </copyright>

namespace Mata
{
    using System;
    using System.Collections.Concurrent;
    using System.Data;
    using System.Data.OleDb;
    using System.Data.SqlClient;

    public class ParameterCache
    {
        private readonly ConcurrentDictionary<string, ParameterSet> parameterCache =
            new ConcurrentDictionary<string, ParameterSet>();

        public void Clear()
        {
            this.parameterCache.Clear();
        }

        public ParameterSet AddParametersToCache(IDbCommand command)
        {
            var key = CreateHashKey(command);
            var parameters = new ParameterSet(command.Parameters);
            this.parameterCache[key] = parameters;
            command.Parameters.Clear();
            return parameters;
        }

        public ParameterSet DeriveParameters(IDbCommand command)
        {
            var key = CreateHashKey(command);
            ParameterSet parameters;
            if (this.parameterCache.TryGetValue(key, out parameters))
            {
                return parameters;
            }

            DeriveParametersFromConnection(command);

            return this.AddParametersToCache(command);
        }

        private static string CreateHashKey(IDbCommand command)
        {
            var connectionString = command.Connection.ConnectionString;
            return $"{connectionString}:{command.CommandText}";
        }

        private static void DeriveParametersFromConnection(IDbCommand command)
        {
            var sqlCommand = command as SqlCommand;
            if (sqlCommand != null)
            {
                SqlCommandBuilder.DeriveParameters(sqlCommand);
                return;
            }

            var oleDbCommand = command as OleDbCommand;
            if (oleDbCommand != null)
            {
                OleDbCommandBuilder.DeriveParameters(oleDbCommand);
                return;
            }

            throw new ArgumentOutOfRangeException(
                nameof(command),
                $"Cannot derive parameters for command type [{command.GetType().Name}].");
        }
    }
}