// <copyright file="DataReaderExtensions.cs" company="Palador Open Source">
//   Copyright (c) Palador Open Source. All rights reserved. See License.txt in the project root for license information.
// </copyright>

namespace Mata
{
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.SqlClient;
    using System.Threading.Tasks;

    public static class DataReaderExtensions
    {
        public static async Task<List<T>> LoadListAsync<T>(
            this DbDataReader reader,
            MapDefinition<T> mapDefinition)
            where T : new()
        {
            var list = new List<T>();
            if (reader.HasRows == false)
            {
                return list;
            }

            var map = CreateMap(reader, mapDefinition);
            while (await reader.ReadAsync())
            {
                var item = LoadItem(reader, map);
                list.Add(item);
            }

            return list;
        }

        public static async Task<T> LoadSingleAsync<T>(
            this DbDataReader reader,
            MapDefinition<T> mapDefinition)
            where T : new()
        {
            if (await reader.ReadAsync() == false)
            {
                return default(T);
            }

            var map = CreateMap(reader, mapDefinition);
            return LoadItem(reader, map);
        }

        private static T LoadItem<T>(IDataRecord reader, IMap<T> map)
            where T : new()
        {
            var item = new T();
            map.Load(item, reader);
            return item;
        }

        private static IMap<T> CreateMap<T>(IDataRecord reader, MapDefinition<T> mapDefinition)
            where T : new()
        {
            var map = mapDefinition.CreateMap();
            map.LoadOrdinals(reader);
            return map;
        }
    }
}