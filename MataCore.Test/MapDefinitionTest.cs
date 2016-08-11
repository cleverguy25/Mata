// <copyright file="MapDefinitionTest.cs" company="Palador Open Source">
// Copyright (c) Palador Open Source. All rights reserved. See License.txt in the project root for license information.
// </copyright>

namespace MataCore.Test
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Reflection;
    using Emit;
    using NSubstitute;
    using Xunit;

    public class MapDefinitionTest
    {
        public MapDefinitionTest()
        {
            MapEmitAssembly.EmitDebugSymbols = true;
        }

        [Fact]
        public void MapProperty()
        {
            // Arrange
            var mapDefinition = new MapDefinition<SampleModel>();

            // Act
            mapDefinition.Map(model => model.Test1);

            // Assert
            var fields = mapDefinition.FieldMapDefinitions;
            AssertFieldMap(fields, "Test1", "Test1", true);
        }

        [Fact]
        public void MapPropertySetAllowNulls()
        {
            // Arrange
            var mapDefinition = new MapDefinition<SampleModel>();

            // Act
            mapDefinition.Map(model => model.Test1, false);

            // Assert
            var fields = mapDefinition.FieldMapDefinitions;
            AssertFieldMap(fields, "Test1", "Test1", false);
        }

        [Fact]
        public void MapPropertySetAllowNullsWithDefault()
        {
            // Arrange
            var mapDefinition = new MapDefinition<SampleModel>();

            // Act
            mapDefinition.Map(model => model.Test1, true, "default");

            // Assert
            var fields = mapDefinition.FieldMapDefinitions;
            AssertFieldMap(fields, "Test1", "Test1", true, "default");
        }

        [Fact]
        public void MapPropertySetSourceColumn()
        {
            // Arrange
            var mapDefinition = new MapDefinition<SampleModel>();

            // Act
            mapDefinition.Map(model => model.Test2, "Bar");

            // Assert
            var fields = mapDefinition.FieldMapDefinitions;
            AssertFieldMap(fields, "Test2", "Bar", false);
        }

        [Fact]
        public void MapPropertySetSourceColumnAndAllowNulls()
        {
            // Arrange
            var mapDefinition = new MapDefinition<SampleModel>();

            // Act
            mapDefinition.Map(model => model.Test3, "Source", true);

            // Assert
            var fields = mapDefinition.FieldMapDefinitions;
            AssertFieldMap(fields, "Test3", "Source", true);
        }

        [Fact]
        public void MapPropertyExplicit()
        {
            // Arrange
            var mapDefinition = new MapDefinition<SampleModel>();

            // Act
            mapDefinition.Map(model => model.Test3, "Explicit", true, int.MaxValue);

            // Assert
            var fields = mapDefinition.FieldMapDefinitions;
            AssertFieldMap(fields, "Test3", "Explicit", true, int.MaxValue);
        }

        [Fact]
        public void MapType()
        {
            // Arrange
            var mapDefinition = new MapDefinition<SampleModel>();

            // Act
            mapDefinition.MapType();

            // Assert
            var fields = mapDefinition.FieldMapDefinitions;
            Assert.Equal(3, fields.Count);
            AssertFieldMap(fields, "Test1", "Test1", true);
            AssertFieldMap(fields, "Test2", "Test2", false);
            AssertFieldMap(fields, "Test3", "Test3", true);
        }

        [Fact]
        public void MapTypeThenOverride()
        {
            // Arrange
            var mapDefinition = new MapDefinition<SampleModel>();

            // Act
            mapDefinition.MapType();
            mapDefinition.Map(model => model.Test2, "Foo", true);
            mapDefinition.Map(model => model.Test3, false);

            // Assert
            var fields = mapDefinition.FieldMapDefinitions;
            Assert.Equal(3, fields.Count);
            AssertFieldMap(fields, "Test1", "Test1", true);
            AssertFieldMap(fields, "Test2", "Foo", true);
            AssertFieldMap(fields, "Test3", "Test3", false);
        }

        [Fact]
        public void CreateMap()
        {
            // Arrange
            var mapDefinition = new MapDefinition<SampleModel>();
            mapDefinition.Map(model => model.Test1, false);
            var dataRecord = Substitute.For<IDataRecord>();
            dataRecord.GetOrdinal("Test1").Returns(1);
            dataRecord.GetString(1).Returns("DB Value");

            // Act
            var map = mapDefinition.CreateMap();
            map.LoadOrdinals(dataRecord);
            var item = new SampleModel();
            map.Load(item, dataRecord);

            // Assert
            Assert.Equal("DB Value", item.Test1);
        }

        [Fact]
        public void TryToMapMethodFails()
        {
            // Arrange
            var mapDefinition = new MapDefinition<SampleModel>();

            // Act, Assert
            Assert.Throws<InvalidOperationException>(
                () => mapDefinition.Map(model => model.TestMethod("Test"), false));
        }

        [Fact]
        public void TryToMapInvalidFails()
        {
            // Arrange
            var mapDefinition = new MapDefinition<InvalidModel>();

            // Act, Assert
            Assert.Throws<ArgumentOutOfRangeException>(
                () => mapDefinition.Map(model => model.Data));
        }

        [Fact]
        public void DefaultValueNotSetWithAllowNullsFails()
        {
            // Arrange
            var mapDefinition = new MapDefinition<TestModel>();

            // Act, Assert
            Assert.Throws<InvalidOperationException>(
                () => mapDefinition.Map(model => model.StringProperty, false, "default"));
        }

        [Fact]
        public void DefaultValueIncompatibleTypesFail()
        {
            // Arrange
            var mapDefinition = new MapDefinition<TestModel>();

            // Act, Assert
            Assert.Throws<ArgumentException>(
                () => mapDefinition.Map(model => model.StringProperty, true, 5));
        }

        [Fact]
        public void DefaultValueDoesNotSupportDecimal()
        {
            // Arrange
            var mapDefinition = new MapDefinition<TestModel>();

            // Act, Assert
            Assert.Throws<InvalidOperationException>(
                () => mapDefinition.Map(model => model.DecimalProperty, true, (decimal)3.14));
        }

        [Fact]
        public void DefaultValueDoesNotSupportGuid()
        {
            // Arrange
            var mapDefinition = new MapDefinition<TestModel>();

            // Act, Assert
            Assert.Throws<InvalidOperationException>(
                () => mapDefinition.Map(model => model.GuidProperty, true, Guid.NewGuid()));
        }

        [Fact]
        public void DefaultValueDoesNotSupportDateTime()
        {
            // Arrange
            var mapDefinition = new MapDefinition<TestModel>();

            // Act, Assert
            Assert.Throws<InvalidOperationException>(
                () => mapDefinition.Map(model => model.DateTimeProperty, true, DateTime.Today));
        }

        private static void AssertFieldMap(
                                           Dictionary<MemberInfo, FieldMapDefinition> fields,
                                           string expectedPropertyName,
                                           string expectedSourceColumn,
                                           bool expectedAllowNulls,
                                           object expectedDefaultValue = null)
        {
            var field = fields.Single(item => item.Key.Name == expectedPropertyName).Value;
            Assert.Equal(expectedSourceColumn, field.SourceColumn);
            Assert.Equal(expectedPropertyName, field.DestinationProperty.Name);
            Assert.Equal(expectedAllowNulls, field.AllowNulls);
            Assert.Equal(expectedDefaultValue, field.DefaultValue);
        }

        public struct Data
        {
        }

        public class InvalidModel
        {
            public Data Data { get; set; }
        }

        public class SampleModel
        {
            public string Test1 { get; set; }

            public bool Test2 { get; set; }

            public int? Test3 { get; set; }

            public List<string> MyStrings { get; set; }

            private long Test4 { get; set; }

            public object TestMethod(string value)
            {
                return null;
            }
        }
    }
}