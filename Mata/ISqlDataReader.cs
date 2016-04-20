// <copyright file="ISqlDataReader.cs" company="Palador Open Source">
// Copyright (c) Palador Open Source. All rights reserved. See License.txt in the project root for license information.
// </copyright>

namespace Mata
{
    using System.Data;
    using System.Threading.Tasks;

    public interface ISqlDataReader : IDataReader
    {
        Task<bool> ReadAsync();
    }
}
