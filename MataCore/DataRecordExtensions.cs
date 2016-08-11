// <copyright file="DataRecordExtensions.cs" company="Palador Open Source">
// Copyright (c) Palador Open Source. All rights reserved. See License.txt in the project root for license information.
// </copyright>

namespace MataCore
{
    using System;
    using System.Data;

    public static class DataRecordExtensions
    {
        public static bool? GetNullableBoolean(this IDataRecord reader, int ordinal)
        {
            return reader.IsDBNull(ordinal) ? null : (bool?)reader.GetBoolean(ordinal);
        }

        public static bool GetBoolean(this IDataRecord reader, int ordinal, bool defaultValue)
        {
            return reader.IsDBNull(ordinal) ? defaultValue : reader.GetBoolean(ordinal);
        }

        public static byte? GetNullableByte(this IDataRecord reader, int ordinal)
        {
            return reader.IsDBNull(ordinal) ? null : (byte?)reader.GetByte(ordinal);
        }

        public static byte GetByte(this IDataRecord reader, int ordinal, byte defaultValue)
        {
            return reader.IsDBNull(ordinal) ? defaultValue : reader.GetByte(ordinal);
        }

        public static short? GetNullableInt16(this IDataRecord reader, int ordinal)
        {
            return reader.IsDBNull(ordinal) ? null : (short?)reader.GetInt16(ordinal);
        }

        public static short GetInt16(this IDataRecord reader, int ordinal, short defaultValue)
        {
            return reader.IsDBNull(ordinal) ? defaultValue : reader.GetInt16(ordinal);
        }

        public static int? GetNullableInt32(this IDataRecord reader, int ordinal)
        {
            return reader.IsDBNull(ordinal) ? null : (int?)reader.GetInt32(ordinal);
        }

        public static int GetInt32(this IDataRecord reader, int ordinal, int defaultValue)
        {
            return reader.IsDBNull(ordinal) ? defaultValue : reader.GetInt32(ordinal);
        }

        public static long? GetNullableInt64(this IDataRecord reader, int ordinal)
        {
            return reader.IsDBNull(ordinal) ? null : (long?)reader.GetInt64(ordinal);
        }

        public static long GetInt64(this IDataRecord reader, int ordinal, long defaultValue)
        {
            return reader.IsDBNull(ordinal) ? defaultValue : reader.GetInt64(ordinal);
        }

        public static char? GetNullableChar(this IDataRecord reader, int ordinal)
        {
            return reader.IsDBNull(ordinal) ? null : (char?)reader.GetChar(ordinal);
        }

        public static char GetChar(this IDataRecord reader, int ordinal, char defaultValue)
        {
            return reader.IsDBNull(ordinal) ? defaultValue : reader.GetChar(ordinal);
        }

        public static string GetNullableString(this IDataRecord reader, int ordinal)
        {
            return reader.IsDBNull(ordinal) ? null : reader.GetString(ordinal);
        }

        public static string GetString(this IDataRecord reader, int ordinal, string defaultValue)
        {
            return reader.IsDBNull(ordinal) ? defaultValue : reader.GetString(ordinal);
        }

        public static DateTime? GetNullableDateTime(this IDataRecord reader, int ordinal)
        {
            return reader.IsDBNull(ordinal) ? null : (DateTime?)reader.GetDateTime(ordinal);
        }

        public static DateTime GetDateTime(this IDataRecord reader, int ordinal, DateTime defaultValue)
        {
            return reader.IsDBNull(ordinal) ? defaultValue : reader.GetDateTime(ordinal);
        }

        public static decimal? GetNullableDecimal(this IDataRecord reader, int ordinal)
        {
            return reader.IsDBNull(ordinal) ? null : (decimal?)reader.GetDecimal(ordinal);
        }

        public static decimal GetDecimal(this IDataRecord reader, int ordinal, decimal defaultValue)
        {
            return reader.IsDBNull(ordinal) ? defaultValue : reader.GetDecimal(ordinal);
        }

        public static float? GetNullableFloat(this IDataRecord reader, int ordinal)
        {
            return reader.IsDBNull(ordinal) ? null : (float?)reader.GetFloat(ordinal);
        }

        public static float GetFloat(this IDataRecord reader, int ordinal, float defaultValue)
        {
            return reader.IsDBNull(ordinal) ? defaultValue : reader.GetFloat(ordinal);
        }

        public static double? GetNullableDouble(this IDataRecord reader, int ordinal)
        {
            return reader.IsDBNull(ordinal) ? null : (double?)reader.GetDouble(ordinal);
        }

        public static double GetDouble(this IDataRecord reader, int ordinal, double defaultValue)
        {
            return reader.IsDBNull(ordinal) ? defaultValue : reader.GetDouble(ordinal);
        }

        public static Guid? GetNullableGuid(this IDataRecord reader, int ordinal)
        {
            return reader.IsDBNull(ordinal) ? null : (Guid?)reader.GetGuid(ordinal);
        }

        public static Guid GetGuid(this IDataRecord reader, int ordinal, Guid defaultValue)
        {
            return reader.IsDBNull(ordinal) ? defaultValue : reader.GetGuid(ordinal);
        }

        public static DateTimeOffset? GetNullableDateTimeOffset(this ISqlDataRecord reader, int ordinal)
        {
            return reader.IsDBNull(ordinal) ? null : (DateTimeOffset?)reader.GetDateTimeOffset(ordinal);
        }

        public static DateTimeOffset GetDateTimeOffset(ISqlDataRecord reader, int ordinal)
        {
            return reader.GetDateTimeOffset(ordinal);
        }
    }
}