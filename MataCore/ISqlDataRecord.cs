// <copyright file="ISqlDataRecord.cs" company="Palador Open Source">
// Copyright (c) Palador Open Source. All rights reserved. See License.txt in the project root for license information.
// </copyright>

namespace MataCore
{
    using System;
    using System.Data;

    public interface ISqlDataRecord : IDataRecord
    {
        DateTimeOffset GetDateTimeOffset(int i);
    }
}
