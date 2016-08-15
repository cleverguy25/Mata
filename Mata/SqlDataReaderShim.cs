// <copyright file="SqlDataReaderShim.cs" company="Palador Open Source">
//   Copyright (c) Palador Open Source. All rights reserved. See License.txt in the project root for license information.
// </copyright>

namespace Mata
{
    using System;
    using System.Data;
    using System.Data.SqlClient;
    using System.Threading.Tasks;

    public class SqlDataReaderShim : ISqlDataReader, ISqlDataRecord
    {
        private readonly SqlDataReader reader;

        public SqlDataReaderShim(SqlDataReader reader)
        {
            this.reader = reader;
        }

        public int FieldCount => this.reader.FieldCount;

        public int Depth => this.reader.Depth;

        public bool IsClosed => this.reader.IsClosed;

        public int RecordsAffected => this.reader.RecordsAffected;

        public object this[int i] => this.reader[i];

        public object this[string name] => this.reader[name];

        public DateTimeOffset GetDateTimeOffset(int i)
        {
            return this.reader.GetDateTimeOffset(i);
        }

        public IDataReader GetData(int i)
        {
            return this.reader.GetData(i);
        }

        public bool IsDBNull(int i)
        {
            return this.reader.IsDBNull(i);
        }

        public string GetName(int i)
        {
            return this.reader.GetName(i);
        }

        public string GetDataTypeName(int i)
        {
            return this.reader.GetDataTypeName(i);
        }

        public Type GetFieldType(int i)
        {
            return this.reader.GetFieldType(i);
        }

        public object GetValue(int i)
        {
            return this.reader.GetValue(i);
        }

        public int GetValues(object[] values)
        {
            return this.reader.GetValues(values);
        }

        public int GetOrdinal(string name)
        {
            return this.reader.GetOrdinal(name);
        }

        public bool GetBoolean(int i)
        {
            return this.reader.GetBoolean(i);
        }

        public byte GetByte(int i)
        {
            return this.reader.GetByte(i);
        }

        public long GetBytes(
            int i,
            long fieldOffset,
            byte[] buffer,
            int bufferoffset,
            int length)
        {
            return this.reader.GetBytes(
                i,
                fieldOffset,
                buffer,
                bufferoffset,
                length);
        }

        public char GetChar(int i)
        {
            return this.reader.GetChar(i);
        }

        public long GetChars(
            int i,
            long fieldoffset,
            char[] buffer,
            int bufferoffset,
            int length)
        {
            return this.reader.GetChars(
                i,
                fieldoffset,
                buffer,
                bufferoffset,
                length);
        }

        public Guid GetGuid(int i)
        {
            return this.reader.GetGuid(i);
        }

        public short GetInt16(int i)
        {
            return this.reader.GetInt16(i);
        }

        public int GetInt32(int i)
        {
            return this.reader.GetInt32(i);
        }

        public long GetInt64(int i)
        {
            return this.reader.GetInt64(i);
        }

        public float GetFloat(int i)
        {
            return this.reader.GetFloat(i);
        }

        public double GetDouble(int i)
        {
            return this.reader.GetDouble(i);
        }

        public string GetString(int i)
        {
            return this.reader.GetString(i);
        }

        public decimal GetDecimal(int i)
        {
            return this.reader.GetDecimal(i);
        }

        public DateTime GetDateTime(int i)
        {
            return this.reader.GetDateTime(i);
        }

        public Task<bool> ReadAsync()
        {
            return this.reader.ReadAsync();
        }

        public void Close()
        {
#if !NETSTANDARD1_6
            this.reader.Close();
#endif
        }

        public DataTable GetSchemaTable()
        {
#if !NETSTANDARD1_6
            return this.reader.GetSchemaTable();
#else
            return null;
#endif
        }

        public bool NextResult()
        {
            return this.reader.NextResult();
        }

        public bool Read()
        {
            return this.reader.Read();
        }

        public void Dispose()
        {
            this.reader.Dispose();
        }

        // TODO: implement an Implicit Cast Operator
    }
}
