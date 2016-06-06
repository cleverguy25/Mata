// <copyright file="TestModel.cs" company="Palador Open Source">
// Copyright (c) Palador Open Source. All rights reserved. See License.txt in the project root for license information.
// </copyright>

namespace Mata.Test
{
    using System;

    public class TestModel
    {
        public string StringProperty { get; set; }

        public bool BoolProperty { get; set; }

        public byte ByteProperty { get; set; }

        public short ShortProperty { get; set; }

        public int IntProperty { get; set; }

        public long LongProperty { get; set; }

        public char CharProperty { get; set; }

        public DateTime DateTimeProperty { get; set; }

        public decimal DecimalProperty { get; set; }

        public float FloatProperty { get; set; }

        public double DoubleProperty { get; set; }

        public Guid GuidProperty { get; set; }

        public bool? NullableBoolProperty { get; set; }

        public byte? NullableByteProperty { get; set; }

        public short? NullableShortProperty { get; set; }

        public int? NullableIntProperty { get; set; }

        public long? NullableLongProperty { get; set; }

        public char? NullableCharProperty { get; set; }

        public DateTime? NullableDateTimeProperty { get; set; }

        public decimal? NullableDecimalProperty { get; set; }

        public float? NullableFloatProperty { get; set; }

        public double? NullableDoubleProperty { get; set; }

        public Guid? NullableGuidProperty { get; set; }

        public DateTimeOffset DateTimeOffsetProperty { get; set; }

        public DateTimeOffset? NullableDateTimeOffsetProperty { get; set; }

        // TypeConveters
        public ShortGuid ShortGuidProperty { get; set; }

        public ShortGuid? NullableShortGuidProperty { get; set; }
    }
}