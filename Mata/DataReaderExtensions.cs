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

            if (mapDefinition.HasSqlServerSpecificFields == false)
            {
                return await ReadListAsync(reader, map, list);
            }

            var sqlReader = new SqlDataReaderShim((SqlDataReader)reader);
            var sqlMap = (ISqlMap<T>)map;
            return await ReadSqlServerSpecificListAsync(sqlReader, sqlMap, list);
        }

        public static async Task<Dictionary<TKey, List<T>>> LoadDictionaryAsync<T, TKey>(
            this DbDataReader reader,
            MapDefinition<T> mapDefinition,
            string keyColumn)
            where T : new()
        {
            var result = new Dictionary<TKey, List<T>>();
            if (reader.HasRows == false)
            {
                return result;
            }

            var keyOrdinal = reader.GetOrdinal(keyColumn);
            var map = CreateMap(reader, mapDefinition);

            if (mapDefinition.HasSqlServerSpecificFields == false)
            {
                return await ReadDictionaryAsync(reader, keyOrdinal, result, map);
            }

            var sqlReader = new SqlDataReaderShim((SqlDataReader)reader);
            var sqlMap = (ISqlMap<T>)map;
            return await ReadSqlServerSpecificDictionaryAsync(sqlReader, keyOrdinal, result, sqlMap);
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

            if (mapDefinition.HasSqlServerSpecificFields == false)
            {
                return LoadItem(reader, map);
            }

            var sqlReader = new SqlDataReaderShim((SqlDataReader)reader);
            var sqlMap = (ISqlMap<T>)map;
            return LoadItemWithSqlServerSpecificField(sqlReader, sqlMap);
        }

        private static IMap<T> CreateMap<T>(IDataRecord reader, MapDefinition<T> mapDefinition)
            where T : new()
        {
            var map = mapDefinition.CreateMap();
            map.LoadOrdinals(reader);
            return map;
        }

        private static async Task<List<T>> ReadListAsync<T>(
            DbDataReader reader,
            IMap<T> map,
            List<T> list)
            where T : new()
        {
            while (await reader.ReadAsync())
            {
                var item = LoadItem(reader, map);
                list.Add(item);
            }

            return list;
        }

        private static async Task<List<T>> ReadSqlServerSpecificListAsync<T>(
            SqlDataReaderShim sqlReader,
            ISqlMap<T> sqlMap,
            List<T> list)
            where T : new()
        {
            while (await sqlReader.ReadAsync())
            {
                var item = LoadItemWithSqlServerSpecificField(sqlReader, sqlMap);
                list.Add(item);
            }

            return list;
        }

        private static async Task<Dictionary<TKey, List<T>>> ReadDictionaryAsync<T, TKey>(
            DbDataReader reader,
            int keyOrdinal,
            Dictionary<TKey, List<T>> result,
            IMap<T> map)
            where T : new()
        {
            while (await reader.ReadAsync())
            {
                var key = (TKey)reader.GetValue(keyOrdinal);
                List<T> list;
                if (result.TryGetValue(key, out list) == false)
                {
                    list = new List<T>();
                    result[key] = list;
                }

                var item = LoadItem(reader, map);
                list.Add(item);
            }

            return result;
        }

        private static async Task<Dictionary<TKey, List<T>>> ReadSqlServerSpecificDictionaryAsync<T, TKey>(
            ISqlDataReader sqlReader,
            int keyOrdinal,
            Dictionary<TKey, List<T>> result,
            ISqlMap<T> sqlMap)
            where T : new()
        {
            while (await sqlReader.ReadAsync())
            {
                var key = (TKey)sqlReader.GetValue(keyOrdinal);
                List<T> list;
                if (result.TryGetValue(key, out list) == false)
                {
                    list = new List<T>();
                    result[key] = list;
                }

                var item = LoadItem(sqlReader, sqlMap);
                list.Add(item);
            }

            return result;
        }

        private static T LoadItem<T>(IDataRecord reader, IMap<T> map)
            where T : new()
        {
            var item = new T();
            map.Load(item, reader);
            return item;
        }

        private static T LoadItemWithSqlServerSpecificField<T>(ISqlDataRecord sqlReader, ISqlMap<T> sqlMap)
            where T : new()
        {
            var item = new T();
            sqlMap.Load(item, sqlReader);
            sqlMap.LoadSqlDataReader(item, sqlReader);
            return item;
        }
    }
}