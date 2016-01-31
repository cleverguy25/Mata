// <copyright file="MapEmitParametersTest.cs" company="Palador Open Source">
//   Copyright (c) Palador Open Source. All rights reserved. See License.txt in the project root for license information.
// </copyright>

namespace Mata.Test
{
    using System;
    using System.Data;
    using System.Data.SqlClient;
    using Emit;
    using Xunit;

    public class MapEmitParametersTest
    {
        public MapEmitParametersTest()
        {
            MapEmitAssembly.EmitDebugSymbols = true;
        }

        //// bool, byte, short, int, long, char, string, datetime, decimal, float, double, guid

        [Fact]
        public void SetBoolParameter()
        {
            // Arrange
            var mapDefinition = new MapDefinition<TestModel>();
            mapDefinition.Map(model => model.BoolProperty);
            var parameterName = "@BoolProperty";
            var command = CreateCommandWithParameter("MySproc", CommandType.StoredProcedure, parameterName);
            var map = mapDefinition.CreateMap();

            // Act
            var item = new TestModel();
            item.BoolProperty = true;
            map.LoadParameters(command, item);

            // Assert
            Assert.Equal(true, command.Parameters[parameterName].Value);
        }

        [Fact]
        public void SetNullableIntParameterWithNull()
        {
            // Arrange
            var mapDefinition = new MapDefinition<TestModel>();
            mapDefinition.Map(model => model.NullableIntProperty);
            var parameterName = "@NullableIntProperty";
            var command = CreateCommandWithParameter("MySproc", CommandType.StoredProcedure, parameterName);
            var map = mapDefinition.CreateMap();

            // Act
            var item = new TestModel();
            item.NullableIntProperty = null;
            map.LoadParameters(command, item);

            // Assert
            Assert.Equal(DBNull.Value, command.Parameters[parameterName].Value);
        }

        [Fact]
        public void SetNullableIntParameter()
        {
            // Arrange
            var mapDefinition = new MapDefinition<TestModel>();
            mapDefinition.Map(model => model.NullableIntProperty);
            var parameterName = "@NullableIntProperty";
            var command = CreateCommandWithParameter("MySproc", CommandType.StoredProcedure, parameterName);
            var map = mapDefinition.CreateMap();

            // Act
            var item = new TestModel();
            item.NullableIntProperty = 32;
            map.LoadParameters(command, item);

            // Assert
            Assert.Equal(32, command.Parameters[parameterName].Value);
        }

        [Fact]
        public void AddDateTimeParameter()
        {
            // Arrange
            var mapDefinition = new MapDefinition<TestModel>(false);
            mapDefinition.Map(model => model.DateTimeProperty);
            var parameterName = "@DateTimeProperty";
            var command = new SqlCommand();
            var map = mapDefinition.CreateMap();

            // Act
            var item = new TestModel();
            item.DateTimeProperty = DateTime.Today;
            map.LoadParameters(command, item);

            // Assert
            Assert.Equal(DateTime.Today, command.Parameters[parameterName].Value);
        }

        [Fact]
        public void AddNullableFloatParameterWithNull()
        {
            // Arrange
            var mapDefinition = new MapDefinition<TestModel>(false);
            mapDefinition.Map(model => model.NullableFloatProperty);
            var parameterName = "@NullableFloatProperty";
            var command = new SqlCommand();
            var map = mapDefinition.CreateMap();

            // Act
            var item = new TestModel();
            item.NullableFloatProperty = null;
            map.LoadParameters(command, item);

            // Assert
            Assert.Equal(DBNull.Value, command.Parameters[parameterName].Value);
        }

        [Fact]
        public void AddNullableGuidParameter()
        {
            // Arrange
            var mapDefinition = new MapDefinition<TestModel>(false);
            mapDefinition.Map(model => model.NullableGuidProperty);
            var parameterName = "@NullableGuidProperty";
            var command = new SqlCommand();
            var map = mapDefinition.CreateMap();

            // Act
            var item = new TestModel();
            item.NullableGuidProperty = Guid.Empty;
            map.LoadParameters(command, item);

            // Assert
            Assert.Equal(Guid.Empty, command.Parameters[parameterName].Value);
        }

        [Fact]
        public void SetStringParameter()
        {
            // Arrange
            var mapDefinition = new MapDefinition<TestModel>();
            mapDefinition.Map(model => model.StringProperty, false);
            var parameterName = "@StringProperty";
            var command = CreateCommandWithParameter("MySproc", CommandType.StoredProcedure, parameterName);
            var map = mapDefinition.CreateMap();

            // Act
            var item = new TestModel();
            item.StringProperty = "Foo";
            map.LoadParameters(command, item);

            // Assert
            Assert.Equal("Foo", command.Parameters[parameterName].Value);
        }

        [Fact]
        public void SetStringParameterWithNull()
        {
            // Arrange
            var mapDefinition = new MapDefinition<TestModel>();
            mapDefinition.Map(model => model.StringProperty, true);
            var parameterName = "@StringProperty";
            var command = CreateCommandWithParameter("MySproc", CommandType.StoredProcedure, parameterName);
            var map = mapDefinition.CreateMap();

            // Act
            var item = new TestModel();
            item.StringProperty = null;
            map.LoadParameters(command, item);

            // Assert
            Assert.Equal(DBNull.Value, command.Parameters[parameterName].Value);
        }

        private static SqlCommand CreateCommandWithParameter(string commandText, CommandType commandType, string parameterName)
        {
            var command = CreateCommand(commandText, commandType);
            command.AddParameter(parameterName, string.Empty);
            DbCommandExtensions.ParameterCache.AddParametersToCache(command);
            return command;
        }

        private static SqlCommand CreateCommand(string commandText, CommandType commandType)
        {
            var connection = new SqlConnection($"Server=\"{Guid.NewGuid()}\"");
            var command = new SqlCommand(commandText, connection)
                              {
                                  CommandType = commandType
                              };
            return command;
        }
    }
}