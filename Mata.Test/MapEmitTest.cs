// <copyright file="MapEmitTest.cs" company="Palador Open Source">
//   Copyright (c) Palador Open Source. All rights reserved. See License.txt in the project root for license information.
// </copyright>

namespace Mata.Test
{
    using System;
    using System.Data;
    using Emit;
    using NSubstitute;
    using Xunit;

    public class MapEmitTest
    {
        public MapEmitTest()
        {
            MapEmitAssembly.EmitDebugSymbols = true;
        }

        [Fact]
        public void MapGetBool()
        {
            // Arrange
            var mapDefinition = new MapDefinition<TestModel>();
            mapDefinition.Map(model => model.BoolProperty);
            var map = mapDefinition.CreateMap();
            var reader = SetupGetBoolFromReader("BoolProperty", true);

            // Act
            var item = new TestModel();
            map.LoadOrdinals(reader);
            map.Load(item, reader);

            // Assert
            Assert.Equal(true, item.BoolProperty);
        }

        [Fact]
        public void MapGetByte()
        {
            // Arrange
            var mapDefinition = new MapDefinition<TestModel>();
            mapDefinition.Map(model => model.ByteProperty);
            var map = mapDefinition.CreateMap();
            var reader = SetupGetByteFromReader("ByteProperty", (byte)8);

            // Act
            var item = new TestModel();
            map.LoadOrdinals(reader);
            map.Load(item, reader);

            // Assert
            Assert.Equal(8, item.ByteProperty);
        }

        [Fact]
        public void MapGetShort()
        {
            // Arrange
            var mapDefinition = new MapDefinition<TestModel>();
            mapDefinition.Map(model => model.ShortProperty);
            var map = mapDefinition.CreateMap();
            var reader = SetupGetShortFromReader("ShortProperty", (short)16);

            // Act
            var item = new TestModel();
            map.LoadOrdinals(reader);
            map.Load(item, reader);

            // Assert
            Assert.Equal(16, item.ShortProperty);
        }

        [Fact]
        public void MapGetInt()
        {
            // Arrange
            var mapDefinition = new MapDefinition<TestModel>();
            mapDefinition.Map(model => model.IntProperty);
            var map = mapDefinition.CreateMap();
            var reader = SetupGetIntFromReader("IntProperty", 32);

            // Act
            var item = new TestModel();
            map.LoadOrdinals(reader);
            map.Load(item, reader);

            // Assert
            Assert.Equal(32, item.IntProperty);
        }

        [Fact]
        public void MapGetLong()
        {
            // Arrange
            var mapDefinition = new MapDefinition<TestModel>();
            mapDefinition.Map(model => model.LongProperty);
            var map = mapDefinition.CreateMap();
            var reader = SetupGetLongFromReader("LongProperty", 64);

            // Act
            var item = new TestModel();
            map.LoadOrdinals(reader);
            map.Load(item, reader);

            // Assert
            Assert.Equal(64, item.LongProperty);
        }

        [Fact]
        public void MapGetChar()
        {
            // Arrange
            var mapDefinition = new MapDefinition<TestModel>();
            mapDefinition.Map(model => model.CharProperty);
            var map = mapDefinition.CreateMap();
            var reader = SetupGetCharFromReader("CharProperty", 'c');

            // Act
            var item = new TestModel();
            map.LoadOrdinals(reader);
            map.Load(item, reader);

            // Assert
            Assert.Equal('c', item.CharProperty);
        }

        [Fact]
        public void MapGetString()
        {
            // Arrange
            var mapDefinition = new MapDefinition<TestModel>();
            mapDefinition.Map(model => model.StringProperty, false);
            var map = mapDefinition.CreateMap();
            var reader = SetupGetStringFromReader("StringProperty", "This is a test.");

            // Act
            var item = new TestModel();
            map.LoadOrdinals(reader);
            map.Load(item, reader);

            // Assert
            Assert.Equal("This is a test.", item.StringProperty);
        }

        [Fact]
        public void MapGetDateTime()
        {
            // Arrange
            var mapDefinition = new MapDefinition<TestModel>();
            mapDefinition.Map(model => model.DateTimeProperty);
            var map = mapDefinition.CreateMap();
            var reader = SetupGetDateTimeFromReader("DateTimeProperty", DateTime.Today);

            // Act
            var item = new TestModel();
            map.LoadOrdinals(reader);
            map.Load(item, reader);

            // Assert
            Assert.Equal(DateTime.Today, item.DateTimeProperty);
        }

        [Fact]
        public void MapGetDecimal()
        {
            // Arrange
            var mapDefinition = new MapDefinition<TestModel>();
            mapDefinition.Map(model => model.DecimalProperty);
            var map = mapDefinition.CreateMap();
            var reader = SetupGetDecimalFromReader("DecimalProperty", (decimal)3.141529);

            // Act
            var item = new TestModel();
            map.LoadOrdinals(reader);
            map.Load(item, reader);

            // Assert
            Assert.Equal((decimal)3.141529, item.DecimalProperty);
        }

        [Fact]
        public void MapGetFloat()
        {
            // Arrange
            var mapDefinition = new MapDefinition<TestModel>();
            mapDefinition.Map(model => model.FloatProperty);
            var map = mapDefinition.CreateMap();
            var reader = SetupGetFloatFromReader("FloatProperty", (float)3.141);

            // Act
            var item = new TestModel();
            map.LoadOrdinals(reader);
            map.Load(item, reader);

            // Assert
            Assert.Equal((float)3.141, item.FloatProperty);
        }

        [Fact]
        public void MapGetDouble()
        {
            // Arrange
            var mapDefinition = new MapDefinition<TestModel>();
            mapDefinition.Map(model => model.DoubleProperty);
            var map = mapDefinition.CreateMap();
            var reader = SetupGetDoubleFromReader("DoubleProperty", 6.022140857);

            // Act
            var item = new TestModel();
            map.LoadOrdinals(reader);
            map.Load(item, reader);

            // Assert
            Assert.Equal(6.022140857, item.DoubleProperty);
        }

        [Fact]
        public void MapGetGuid()
        {
            // Arrange
            var mapDefinition = new MapDefinition<TestModel>();
            mapDefinition.Map(model => model.GuidProperty);
            var map = mapDefinition.CreateMap();
            var guid = Guid.NewGuid();
            var reader = SetupGetGuidFromReader("GuidProperty", guid);

            // Act
            var item = new TestModel();
            map.LoadOrdinals(reader);
            map.Load(item, reader);

            // Assert
            Assert.Equal(guid, item.GuidProperty);
        }

        [Fact]
        public void MapGetNullableBool()
        {
            // Arrange
            var mapDefinition = new MapDefinition<TestModel>();
            mapDefinition.Map(model => model.NullableBoolProperty);
            var map = mapDefinition.CreateMap();
            var reader = SetupGetBoolFromReader("NullableBoolProperty", true);

            // Act
            var item = new TestModel();
            map.LoadOrdinals(reader);
            map.Load(item, reader);

            // Assert
            Assert.NotNull(item.NullableBoolProperty);
            Assert.Equal(true, item.NullableBoolProperty.Value);
        }

        [Fact]
        public void MapGetNullableByte()
        {
            // Arrange
            var mapDefinition = new MapDefinition<TestModel>();
            mapDefinition.Map(model => model.NullableByteProperty);
            var map = mapDefinition.CreateMap();
            var reader = SetupGetByteFromReader("NullableByteProperty", (byte)8);

            // Act
            var item = new TestModel();
            map.LoadOrdinals(reader);
            map.Load(item, reader);

            // Assert
            Assert.NotNull(item.NullableByteProperty);
            Assert.Equal(8, item.NullableByteProperty.Value);
        }

        [Fact]
        public void MapGetNullableShort()
        {
            // Arrange
            var mapDefinition = new MapDefinition<TestModel>();
            mapDefinition.Map(model => model.NullableShortProperty);
            var map = mapDefinition.CreateMap();
            var reader = SetupGetShortFromReader("NullableShortProperty", (short)16);

            // Act
            var item = new TestModel();
            map.LoadOrdinals(reader);
            map.Load(item, reader);

            // Assert
            Assert.NotNull(item.NullableShortProperty);
            Assert.Equal(16, item.NullableShortProperty.Value);
        }

        [Fact]
        public void MapGetNullableInt()
        {
            // Arrange
            var mapDefinition = new MapDefinition<TestModel>();
            mapDefinition.Map(model => model.NullableIntProperty);
            var map = mapDefinition.CreateMap();
            var reader = SetupGetIntFromReader("NullableIntProperty", 32);

            // Act
            var item = new TestModel();
            map.LoadOrdinals(reader);
            map.Load(item, reader);

            // Assert
            Assert.NotNull(item.NullableIntProperty);
            Assert.Equal(32, item.NullableIntProperty.Value);
        }

        [Fact]
        public void MapGetNullableLong()
        {
            // Arrange
            var mapDefinition = new MapDefinition<TestModel>();
            mapDefinition.Map(model => model.NullableLongProperty);
            var map = mapDefinition.CreateMap();
            var reader = SetupGetLongFromReader("NullableLongProperty", 64);

            // Act
            var item = new TestModel();
            map.LoadOrdinals(reader);
            map.Load(item, reader);

            // Assert
            Assert.NotNull(item.NullableLongProperty);
            Assert.Equal(64, item.NullableLongProperty.Value);
        }

        [Fact]
        public void MapGetNullableChar()
        {
            // Arrange
            var mapDefinition = new MapDefinition<TestModel>();
            mapDefinition.Map(model => model.NullableCharProperty);
            var map = mapDefinition.CreateMap();
            var reader = SetupGetCharFromReader("NullableCharProperty", 'c');

            // Act
            var item = new TestModel();
            map.LoadOrdinals(reader);
            map.Load(item, reader);

            // Assert
            Assert.NotNull(item.NullableCharProperty);
            Assert.Equal('c', item.NullableCharProperty.Value);
        }

        [Fact]
        public void MapGetNullableString()
        {
            // Arrange
            var mapDefinition = new MapDefinition<TestModel>();
            mapDefinition.Map(model => model.StringProperty);
            var map = mapDefinition.CreateMap();
            var reader = SetupGetStringFromReader("StringProperty", "This is a test.");

            // Act
            var item = new TestModel();
            map.LoadOrdinals(reader);
            map.Load(item, reader);

            // Assert
            Assert.Equal("This is a test.", item.StringProperty);
        }

        [Fact]
        public void MapGetNullableDateTime()
        {
            // Arrange
            var mapDefinition = new MapDefinition<TestModel>();
            mapDefinition.Map(model => model.NullableDateTimeProperty);
            var map = mapDefinition.CreateMap();
            var reader = SetupGetDateTimeFromReader("NullableDateTimeProperty", DateTime.Today);

            // Act
            var item = new TestModel();
            map.LoadOrdinals(reader);
            map.Load(item, reader);

            // Assert
            Assert.NotNull(item.NullableDateTimeProperty);
            Assert.Equal(DateTime.Today, item.NullableDateTimeProperty.Value);
        }

        [Fact]
        public void MapGetNullableDecimal()
        {
            // Arrange
            var mapDefinition = new MapDefinition<TestModel>();
            mapDefinition.Map(model => model.NullableDecimalProperty);
            var map = mapDefinition.CreateMap();
            var reader = SetupGetDecimalFromReader("NullableDecimalProperty", (decimal)3.141529);

            // Act
            var item = new TestModel();
            map.LoadOrdinals(reader);
            map.Load(item, reader);

            // Assert
            Assert.NotNull(item.NullableDecimalProperty);
            Assert.Equal((decimal)3.141529, item.NullableDecimalProperty.Value);
        }

        [Fact]
        public void MapGetNullableFloat()
        {
            // Arrange
            var mapDefinition = new MapDefinition<TestModel>();
            mapDefinition.Map(model => model.NullableFloatProperty);
            var map = mapDefinition.CreateMap();
            var reader = SetupGetFloatFromReader("NullableFloatProperty", (float)3.141);

            // Act
            var item = new TestModel();
            map.LoadOrdinals(reader);
            map.Load(item, reader);

            // Assert
            Assert.NotNull(item.NullableFloatProperty);
            Assert.Equal((float)3.141, item.NullableFloatProperty.Value);
        }

        [Fact]
        public void MapGetNullableDouble()
        {
            // Arrange
            var mapDefinition = new MapDefinition<TestModel>();
            mapDefinition.Map(model => model.NullableDoubleProperty);
            var map = mapDefinition.CreateMap();
            var reader = SetupGetDoubleFromReader("NullableDoubleProperty", 6.022140857);

            // Act
            var item = new TestModel();
            map.LoadOrdinals(reader);
            map.Load(item, reader);

            // Assert
            Assert.NotNull(item.NullableDoubleProperty);
            Assert.Equal(6.022140857, item.NullableDoubleProperty.Value);
        }

        [Fact]
        public void MapGetNullableGuid()
        {
            // Arrange
            var mapDefinition = new MapDefinition<TestModel>();
            mapDefinition.Map(model => model.NullableGuidProperty);
            var map = mapDefinition.CreateMap();
            var guid = Guid.NewGuid();
            var reader = SetupGetGuidFromReader("NullableGuidProperty", guid);

            // Act
            var item = new TestModel();
            map.LoadOrdinals(reader);
            map.Load(item, reader);

            // Assert
            Assert.NotNull(item.NullableGuidProperty);
            Assert.Equal(guid, item.NullableGuidProperty.Value);
        }

        [Fact]
        public void MapGetNullableBoolWithNull()
        {
            // Arrange
            var mapDefinition = new MapDefinition<TestModel>();
            mapDefinition.Map(model => model.NullableBoolProperty);
            var map = mapDefinition.CreateMap();
            var reader = GetDataRecordWithNullColumn("NullableBoolProperty");

            // Act
            var item = new TestModel();
            map.LoadOrdinals(reader);
            map.Load(item, reader);

            // Assert
            Assert.Null(item.NullableBoolProperty);
        }

        [Fact]
        public void MapGetNullableByteWithNull()
        {
            // Arrange
            var mapDefinition = new MapDefinition<TestModel>();
            mapDefinition.Map(model => model.NullableByteProperty);
            var map = mapDefinition.CreateMap();
            var reader = GetDataRecordWithNullColumn("NullableByteProperty");

            // Act
            var item = new TestModel();
            map.LoadOrdinals(reader);
            map.Load(item, reader);

            // Assert
            Assert.Null(item.NullableByteProperty);
        }

        [Fact]
        public void MapGetNullableShortWithNull()
        {
            // Arrange
            var mapDefinition = new MapDefinition<TestModel>();
            mapDefinition.Map(model => model.NullableShortProperty);
            var map = mapDefinition.CreateMap();
            var reader = GetDataRecordWithNullColumn("NullableShortProperty");

            // Act
            var item = new TestModel();
            map.LoadOrdinals(reader);
            map.Load(item, reader);

            // Assert
            Assert.Null(item.NullableShortProperty);
        }

        [Fact]
        public void MapGetNullableIntWithNull()
        {
            // Arrange
            var mapDefinition = new MapDefinition<TestModel>();
            mapDefinition.Map(model => model.NullableIntProperty);
            var map = mapDefinition.CreateMap();
            var reader = GetDataRecordWithNullColumn("NullableIntProperty");

            // Act
            var item = new TestModel();
            map.LoadOrdinals(reader);
            map.Load(item, reader);

            // Assert
            Assert.Null(item.NullableIntProperty);
        }

        [Fact]
        public void MapGetNullableLongWithNull()
        {
            // Arrange
            var mapDefinition = new MapDefinition<TestModel>();
            mapDefinition.Map(model => model.NullableLongProperty);
            var map = mapDefinition.CreateMap();
            var reader = GetDataRecordWithNullColumn("NullableLongProperty");

            // Act
            var item = new TestModel();
            map.LoadOrdinals(reader);
            map.Load(item, reader);

            // Assert
            Assert.Null(item.NullableLongProperty);
        }

        [Fact]
        public void MapGetNullableCharWithNull()
        {
            // Arrange
            var mapDefinition = new MapDefinition<TestModel>();
            mapDefinition.Map(model => model.NullableCharProperty);
            var map = mapDefinition.CreateMap();
            var reader = GetDataRecordWithNullColumn("NullableCharProperty");

            // Act
            var item = new TestModel();
            map.LoadOrdinals(reader);
            map.Load(item, reader);

            // Assert
            Assert.Null(item.NullableCharProperty);
        }

        [Fact]
        public void MapGetNullableStringWithNull()
        {
            // Arrange
            var mapDefinition = new MapDefinition<TestModel>();
            mapDefinition.Map(model => model.StringProperty, true);
            var map = mapDefinition.CreateMap();
            var reader = GetDataRecordWithNullColumn("StringProperty");

            // Act
            var item = new TestModel();
            map.LoadOrdinals(reader);
            map.Load(item, reader);

            // Assert
            Assert.Null(item.StringProperty);
        }

        [Fact]
        public void MapGetNullableDateTimeWithNull()
        {
            // Arrange
            var mapDefinition = new MapDefinition<TestModel>();
            mapDefinition.Map(model => model.NullableDateTimeProperty);
            var map = mapDefinition.CreateMap();
            var reader = GetDataRecordWithNullColumn("NullableDateTimeProperty");

            // Act
            var item = new TestModel();
            map.LoadOrdinals(reader);
            map.Load(item, reader);

            // Assert
            Assert.Null(item.NullableDateTimeProperty);
        }

        [Fact]
        public void MapGetNullableDecimalWithNull()
        {
            // Arrange
            var mapDefinition = new MapDefinition<TestModel>();
            mapDefinition.Map(model => model.NullableDecimalProperty);
            var map = mapDefinition.CreateMap();
            var reader = GetDataRecordWithNullColumn("NullableDecimalProperty");

            // Act
            var item = new TestModel();
            map.LoadOrdinals(reader);
            map.Load(item, reader);

            // Assert
            Assert.Null(item.NullableDecimalProperty);
        }

        [Fact]
        public void MapGetNullableFloatWithNull()
        {
            // Arrange
            var mapDefinition = new MapDefinition<TestModel>();
            mapDefinition.Map(model => model.NullableFloatProperty);
            var map = mapDefinition.CreateMap();
            var reader = GetDataRecordWithNullColumn("NullableFloatProperty");

            // Act
            var item = new TestModel();
            map.LoadOrdinals(reader);
            map.Load(item, reader);

            // Assert
            Assert.Null(item.NullableFloatProperty);
        }

        [Fact]
        public void MapGetNullableDoubleWithNull()
        {
            // Arrange
            var mapDefinition = new MapDefinition<TestModel>();
            mapDefinition.Map(model => model.NullableDoubleProperty);
            var map = mapDefinition.CreateMap();
            var reader = GetDataRecordWithNullColumn("NullableDoubleProperty");

            // Act
            var item = new TestModel();
            map.LoadOrdinals(reader);
            map.Load(item, reader);

            // Assert
            Assert.Null(item.NullableDoubleProperty);
        }

        [Fact]
        public void MapGetNullableGuidWithNull()
        {
            // Arrange
            var mapDefinition = new MapDefinition<TestModel>();
            mapDefinition.Map(model => model.NullableGuidProperty);
            var map = mapDefinition.CreateMap();
            var reader = GetDataRecordWithNullColumn("NullableGuidProperty");

            // Act
            var item = new TestModel();
            map.LoadOrdinals(reader);
            map.Load(item, reader);

            // Assert
            Assert.Null(item.NullableGuidProperty);
        }

        [Fact]
        public void MapGetBoolWithDefaultNull()
        {
            // Arrange
            var mapDefinition = new MapDefinition<TestModel>();
            mapDefinition.Map(model => model.BoolProperty, true, true);
            var map = mapDefinition.CreateMap();
            var reader = GetDataRecordWithNullColumn("BoolProperty");

            // Act
            var item = new TestModel();
            map.LoadOrdinals(reader);
            map.Load(item, reader);

            // Assert
            Assert.Equal(true, item.BoolProperty);
        }

        [Fact]
        public void MapGetByteWithDefaultNull()
        {
            // Arrange
            var mapDefinition = new MapDefinition<TestModel>();
            mapDefinition.Map(model => model.ByteProperty, true, byte.MaxValue);
            var map = mapDefinition.CreateMap();
            var reader = GetDataRecordWithNullColumn("ByteProperty");

            // Act
            var item = new TestModel();
            map.LoadOrdinals(reader);
            map.Load(item, reader);

            // Assert
            Assert.Equal(byte.MaxValue, item.ByteProperty);
        }

        [Fact]
        public void MapGetShortWithDefaultNull()
        {
            // Arrange
            var mapDefinition = new MapDefinition<TestModel>();
            mapDefinition.Map(model => model.ShortProperty, true, short.MaxValue);
            var map = mapDefinition.CreateMap();
            var reader = GetDataRecordWithNullColumn("ShortProperty");

            // Act
            var item = new TestModel();
            map.LoadOrdinals(reader);
            map.Load(item, reader);

            // Assert
            Assert.Equal(short.MaxValue, item.ShortProperty);
        }

        [Fact]
        public void MapGetIntWithDefaultNull()
        {
            // Arrange
            var mapDefinition = new MapDefinition<TestModel>();
            mapDefinition.Map(model => model.IntProperty, true, int.MaxValue);
            var map = mapDefinition.CreateMap();
            var reader = GetDataRecordWithNullColumn("IntProperty");

            // Act
            var item = new TestModel();
            map.LoadOrdinals(reader);
            map.Load(item, reader);

            // Assert
            Assert.Equal(int.MaxValue, item.IntProperty);
        }

        [Fact]
        public void MapGetLongWithDefaultNull()
        {
            // Arrange
            var mapDefinition = new MapDefinition<TestModel>();
            mapDefinition.Map(model => model.LongProperty, true, long.MaxValue);
            var map = mapDefinition.CreateMap();
            var reader = GetDataRecordWithNullColumn("LongProperty");

            // Act
            var item = new TestModel();
            map.LoadOrdinals(reader);
            map.Load(item, reader);

            // Assert
            Assert.Equal(long.MaxValue, item.LongProperty);
        }

        [Fact]
        public void MapGetCharWithDefaultNull()
        {
            // Arrange
            var mapDefinition = new MapDefinition<TestModel>();
            mapDefinition.Map(model => model.CharProperty, true, 'd');
            var map = mapDefinition.CreateMap();
            var reader = GetDataRecordWithNullColumn("CharProperty");

            // Act
            var item = new TestModel();
            map.LoadOrdinals(reader);
            map.Load(item, reader);

            // Assert
            Assert.Equal('d', item.CharProperty);
        }

        [Fact]
        public void MapGetStringWithDefaultNull()
        {
            // Arrange
            var mapDefinition = new MapDefinition<TestModel>();
            mapDefinition.Map(model => model.StringProperty, true, "default");
            var map = mapDefinition.CreateMap();
            var reader = GetDataRecordWithNullColumn("StringProperty");

            // Act
            var item = new TestModel();
            map.LoadOrdinals(reader);
            map.Load(item, reader);

            // Assert
            Assert.Equal("default", item.StringProperty);
        }

        [Fact]
        public void MapGetFloatWithDefaultNull()
        {
            // Arrange
            var mapDefinition = new MapDefinition<TestModel>();
            mapDefinition.Map(model => model.FloatProperty, true, (float)3.14);
            var map = mapDefinition.CreateMap();
            var reader = GetDataRecordWithNullColumn("FloatProperty");

            // Act
            var item = new TestModel();
            map.LoadOrdinals(reader);
            map.Load(item, reader);

            // Assert
            Assert.Equal((float)3.14, item.FloatProperty);
        }

        [Fact]
        public void MapGetDoubleWithDefaultNull()
        {
            // Arrange
            var mapDefinition = new MapDefinition<TestModel>();
            mapDefinition.Map(model => model.DoubleProperty, true, 6.022140857);
            var map = mapDefinition.CreateMap();
            var reader = GetDataRecordWithNullColumn("DoubleProperty");

            // Act
            var item = new TestModel();
            map.LoadOrdinals(reader);
            map.Load(item, reader);

            // Assert
            Assert.Equal(6.022140857, item.DoubleProperty);
        }

        [Fact]
        public void MapGetBoolWithDefaultToNullableField()
        {
            // Arrange
            var mapDefinition = new MapDefinition<TestModel>();
            mapDefinition.Map(model => model.NullableBoolProperty, true, false);
            var map = mapDefinition.CreateMap();
            var reader = SetupGetBoolFromReader("NullableBoolProperty", true);

            // Act
            var item = new TestModel();
            map.LoadOrdinals(reader);
            map.Load(item, reader);

            // Assert
            Assert.NotNull(item.NullableBoolProperty);
            Assert.Equal(true, item.NullableBoolProperty.Value);
        }

        [Fact]
        public void MapGetByteWithDefaultToNullableField()
        {
            // Arrange
            var mapDefinition = new MapDefinition<TestModel>();
            mapDefinition.Map(model => model.NullableByteProperty, true, byte.MinValue);
            var map = mapDefinition.CreateMap();
            var reader = SetupGetByteFromReader("NullableByteProperty", (byte)8);

            // Act
            var item = new TestModel();
            map.LoadOrdinals(reader);
            map.Load(item, reader);

            // Assert
            Assert.NotNull(item.NullableByteProperty);
            Assert.Equal(8, item.NullableByteProperty.Value);
        }

        [Fact]
        public void MapGetShortWithDefaultToNullableField()
        {
            // Arrange
            var mapDefinition = new MapDefinition<TestModel>();
            mapDefinition.Map(model => model.NullableShortProperty, true, short.MinValue);
            var map = mapDefinition.CreateMap();
            var reader = SetupGetShortFromReader("NullableShortProperty", (short)16);

            // Act
            var item = new TestModel();
            map.LoadOrdinals(reader);
            map.Load(item, reader);

            // Assert
            Assert.NotNull(item.NullableShortProperty);
            Assert.Equal(16, item.NullableShortProperty.Value);
        }

        [Fact]
        public void MapGetIntWithDefaultToNullableField()
        {
            // Arrange
            var mapDefinition = new MapDefinition<TestModel>();
            mapDefinition.Map(model => model.NullableIntProperty, true, int.MinValue);
            var map = mapDefinition.CreateMap();
            var reader = SetupGetIntFromReader("NullableIntProperty", 32);

            // Act
            var item = new TestModel();
            map.LoadOrdinals(reader);
            map.Load(item, reader);

            // Assert
            Assert.NotNull(item.NullableIntProperty);
            Assert.Equal(32, item.NullableIntProperty.Value);
        }

        [Fact]
        public void MapGetLongWithDefaultToNullableField()
        {
            // Arrange
            var mapDefinition = new MapDefinition<TestModel>();
            mapDefinition.Map(model => model.NullableLongProperty, true, long.MinValue);
            var map = mapDefinition.CreateMap();
            var reader = SetupGetLongFromReader("NullableLongProperty", 64);

            // Act
            var item = new TestModel();
            map.LoadOrdinals(reader);
            map.Load(item, reader);

            // Assert
            Assert.NotNull(item.NullableLongProperty);
            Assert.Equal(64, item.NullableLongProperty.Value);
        }

        [Fact]
        public void MapGetCharWithDefaultToNullableField()
        {
            // Arrange
            var mapDefinition = new MapDefinition<TestModel>();
            mapDefinition.Map(model => model.NullableCharProperty, true, 'e');
            var map = mapDefinition.CreateMap();
            var reader = SetupGetCharFromReader("NullableCharProperty", 'c');

            // Act
            var item = new TestModel();
            map.LoadOrdinals(reader);
            map.Load(item, reader);

            // Assert
            Assert.NotNull(item.NullableCharProperty);
            Assert.Equal('c', item.NullableCharProperty.Value);
        }

        [Fact]
        public void MapGetStringWithDefault()
        {
            // Arrange
            var mapDefinition = new MapDefinition<TestModel>();
            mapDefinition.Map(model => model.StringProperty, true, "foo");
            var map = mapDefinition.CreateMap();
            var reader = SetupGetStringFromReader("StringProperty", "This is a test.");

            // Act
            var item = new TestModel();
            map.LoadOrdinals(reader);
            map.Load(item, reader);

            // Assert
            Assert.Equal("This is a test.", item.StringProperty);
        }

        [Fact]
        public void MapGetFloatWithDefaultToNullableField()
        {
            // Arrange
            var mapDefinition = new MapDefinition<TestModel>();
            mapDefinition.Map(model => model.NullableFloatProperty, true, (float)1.71);
            var map = mapDefinition.CreateMap();
            var reader = SetupGetFloatFromReader("NullableFloatProperty", (float)3.141);

            // Act
            var item = new TestModel();
            map.LoadOrdinals(reader);
            map.Load(item, reader);

            // Assert
            Assert.NotNull(item.NullableFloatProperty);
            Assert.Equal((float)3.141, item.NullableFloatProperty.Value);
        }

        [Fact]
        public void MapGetDoubleWithDefaultToNullableField()
        {
            // Arrange
            var mapDefinition = new MapDefinition<TestModel>();
            mapDefinition.Map(model => model.NullableDoubleProperty, true, 12.00001234);
            var map = mapDefinition.CreateMap();
            var reader = SetupGetDoubleFromReader("NullableDoubleProperty", 6.022140857);

            // Act
            var item = new TestModel();
            map.LoadOrdinals(reader);
            map.Load(item, reader);

            // Assert
            Assert.NotNull(item.NullableDoubleProperty);
            Assert.Equal(6.022140857, item.NullableDoubleProperty.Value);
        }

        [Fact]
        public void MapGetBoolWithDefault()
        {
            // Arrange
            var mapDefinition = new MapDefinition<TestModel>();
            mapDefinition.Map(model => model.BoolProperty, true, false);
            var map = mapDefinition.CreateMap();
            var reader = SetupGetBoolFromReader("BoolProperty", true);

            // Act
            var item = new TestModel();
            map.LoadOrdinals(reader);
            map.Load(item, reader);

            // Assert
            Assert.Equal(true, item.BoolProperty);
        }

        [Fact]
        public void MapGetByteWithDefault()
        {
            // Arrange
            var mapDefinition = new MapDefinition<TestModel>();
            mapDefinition.Map(model => model.ByteProperty, true, byte.MinValue);
            var map = mapDefinition.CreateMap();
            var reader = SetupGetByteFromReader("ByteProperty", (byte)8);

            // Act
            var item = new TestModel();
            map.LoadOrdinals(reader);
            map.Load(item, reader);

            // Assert
            Assert.Equal(8, item.ByteProperty);
        }

        [Fact]
        public void MapGetShortWithDefault()
        {
            // Arrange
            var mapDefinition = new MapDefinition<TestModel>();
            mapDefinition.Map(model => model.ShortProperty, true, short.MinValue);
            var map = mapDefinition.CreateMap();
            var reader = SetupGetShortFromReader("ShortProperty", (byte)16);

            // Act
            var item = new TestModel();
            map.LoadOrdinals(reader);
            map.Load(item, reader);

            // Assert
            Assert.Equal(16, item.ShortProperty);
        }

        [Fact]
        public void MapGetIntWithDefault()
        {
            // Arrange
            var mapDefinition = new MapDefinition<TestModel>();
            mapDefinition.Map(model => model.IntProperty, true, int.MinValue);
            var map = mapDefinition.CreateMap();
            var reader = SetupGetIntFromReader("IntProperty", 32);

            // Act
            var item = new TestModel();
            map.LoadOrdinals(reader);
            map.Load(item, reader);

            // Assert
            Assert.Equal(32, item.IntProperty);
        }

        [Fact]
        public void MapGetLongWithDefault()
        {
            // Arrange
            var mapDefinition = new MapDefinition<TestModel>();
            mapDefinition.Map(model => model.LongProperty, true, long.MinValue);
            var map = mapDefinition.CreateMap();
            var reader = SetupGetLongFromReader("LongProperty", 64);

            // Act
            var item = new TestModel();
            map.LoadOrdinals(reader);
            map.Load(item, reader);

            // Assert
            Assert.Equal(64, item.LongProperty);
        }

        [Fact]
        public void MapGetCharWithDefault()
        {
            // Arrange
            var mapDefinition = new MapDefinition<TestModel>();
            mapDefinition.Map(model => model.CharProperty, true, 'e');
            var map = mapDefinition.CreateMap();
            var reader = SetupGetCharFromReader("CharProperty", 'c');

            // Act
            var item = new TestModel();
            map.LoadOrdinals(reader);
            map.Load(item, reader);

            // Assert
            Assert.Equal('c', item.CharProperty);
        }

        [Fact]
        public void MapGetFloatWithDefault()
        {
            // Arrange
            var mapDefinition = new MapDefinition<TestModel>();
            mapDefinition.Map(model => model.FloatProperty, true, (float)1.71);
            var map = mapDefinition.CreateMap();
            var reader = SetupGetFloatFromReader("FloatProperty", (float)3.141);

            // Act
            var item = new TestModel();
            map.LoadOrdinals(reader);
            map.Load(item, reader);

            // Assert
            Assert.Equal((float)3.141, item.FloatProperty);
        }

        [Fact]
        public void MapGetDoubleWithDefault()
        {
            // Arrange
            var mapDefinition = new MapDefinition<TestModel>();
            mapDefinition.Map(model => model.DoubleProperty, true, 12.00001234);
            var map = mapDefinition.CreateMap();
            var reader = SetupGetDoubleFromReader("DoubleProperty", 6.022140857);

            // Act
            var item = new TestModel();
            map.LoadOrdinals(reader);
            map.Load(item, reader);

            // Assert
            Assert.Equal(6.022140857, item.DoubleProperty);
        }

        private static IDataRecord GetDataRecordWithNullColumn(string property)
        {
            var ordinal = new Random().Next();
            var reader = Substitute.For<IDataRecord>();
            reader.GetOrdinal(property).Returns(ordinal);
            reader.IsDBNull(ordinal).Returns(true);
            return reader;
        }

        private static IDataRecord SetupGetBoolFromReader(string propertyName, bool returnValue)
        {
            var ordinal = new Random().Next();
            var reader = Substitute.For<IDataRecord>();
            reader.GetOrdinal(propertyName).Returns(ordinal);
            reader.GetBoolean(ordinal).Returns(returnValue);
            return reader;
        }

        private static IDataRecord SetupGetByteFromReader(string propertyName, byte returnValue)
        {
            var ordinal = new Random().Next();
            var reader = Substitute.For<IDataRecord>();
            reader.GetOrdinal(propertyName).Returns(ordinal);
            reader.GetByte(ordinal).Returns(returnValue);
            return reader;
        }

        private static IDataRecord SetupGetShortFromReader(string propertyName, short returnValue)
        {
            var ordinal = new Random().Next();
            var reader = Substitute.For<IDataRecord>();
            reader.GetOrdinal(propertyName).Returns(ordinal);
            reader.GetInt16(ordinal).Returns(returnValue);
            return reader;
        }

        private static IDataRecord SetupGetIntFromReader(string propertyName, int returnValue)
        {
            var ordinal = new Random().Next();
            var reader = Substitute.For<IDataRecord>();
            reader.GetOrdinal(propertyName).Returns(ordinal);
            reader.GetInt32(ordinal).Returns(returnValue);
            return reader;
        }

        private static IDataRecord SetupGetLongFromReader(string propertyName, long returnValue)
        {
            var ordinal = new Random().Next();
            var reader = Substitute.For<IDataRecord>();
            reader.GetOrdinal(propertyName).Returns(ordinal);
            reader.GetInt64(ordinal).Returns(returnValue);
            return reader;
        }

        private static IDataRecord SetupGetCharFromReader(string propertyName, char returnValue)
        {
            var ordinal = new Random().Next();
            var reader = Substitute.For<IDataRecord>();
            reader.GetOrdinal(propertyName).Returns(ordinal);
            reader.GetChar(ordinal).Returns(returnValue);
            return reader;
        }

        private static IDataRecord SetupGetStringFromReader(string propertyName, string returnValue)
        {
            var ordinal = new Random().Next();
            var reader = Substitute.For<IDataRecord>();
            reader.GetOrdinal(propertyName).Returns(ordinal);
            reader.GetString(ordinal).Returns(returnValue);
            return reader;
        }

        private static IDataRecord SetupGetDateTimeFromReader(string propertyName, DateTime returnValue)
        {
            var ordinal = new Random().Next();
            var reader = Substitute.For<IDataRecord>();
            reader.GetOrdinal(propertyName).Returns(ordinal);
            reader.GetDateTime(ordinal).Returns(returnValue);
            return reader;
        }

        private static IDataRecord SetupGetDecimalFromReader(string propertyName, decimal returnValue)
        {
            var ordinal = new Random().Next();
            var reader = Substitute.For<IDataRecord>();
            reader.GetOrdinal(propertyName).Returns(ordinal);
            reader.GetDecimal(ordinal).Returns(returnValue);
            return reader;
        }

        private static IDataRecord SetupGetFloatFromReader(string propertyName, float returnValue)
        {
            var ordinal = new Random().Next();
            var reader = Substitute.For<IDataRecord>();
            reader.GetOrdinal(propertyName).Returns(ordinal);
            reader.GetFloat(ordinal).Returns(returnValue);
            return reader;
        }

        private static IDataRecord SetupGetDoubleFromReader(string propertyName, double returnValue)
        {
            var ordinal = new Random().Next();
            var reader = Substitute.For<IDataRecord>();
            reader.GetOrdinal(propertyName).Returns(ordinal);
            reader.GetDouble(ordinal).Returns(returnValue);
            return reader;
        }

        private static IDataRecord SetupGetGuidFromReader(string propertyName, Guid returnValue)
        {
            var ordinal = new Random().Next();
            var reader = Substitute.For<IDataRecord>();
            reader.GetOrdinal(propertyName).Returns(ordinal);
            reader.GetGuid(ordinal).Returns(returnValue);
            return reader;
        }
    }
}