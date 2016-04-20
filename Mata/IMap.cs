// <copyright file="IMap.cs" company="Palador Open Source">
// Copyright (c) Palador Open Source. All rights reserved. See License.txt in the project root for license information.
// </copyright>

namespace Mata
{
    using System.Data;

    public interface IMap<in T>
    {
        void LoadOrdinals(IDataRecord reader);

        void Load(T model, IDataRecord reader);

        void LoadParameters(IDbCommand command, T model);
    }
}