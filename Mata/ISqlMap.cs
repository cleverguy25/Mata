// <copyright file="ISqlMap.cs" company="Palador Open Source">
// Copyright (c) Palador Open Source. All rights reserved. See License.txt in the project root for license information.
// </copyright>

namespace Mata
{
    public interface ISqlMap<in T> : IMap<T>
    {
        void LoadSqlDataReader(T model, ISqlDataRecord reader);
    }
}