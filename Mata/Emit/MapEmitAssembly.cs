// <copyright file="MapEmitAssembly.cs" company="Palador Open Source">
// Copyright (c) Palador Open Source. All rights reserved. See License.txt in the project root for license information.
// </copyright>

namespace Mata.Emit
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics;
    using System.Diagnostics.SymbolStore;
    using System.Reflection;
    using System.Reflection.Emit;

    public static class MapEmitAssembly
    {
        private static readonly AssemblyName AssemblyName = new AssemblyName("SqlDataReaderMap");

        private static readonly Lazy<ModuleBuilder> ModuleBuilderInstance = new Lazy<ModuleBuilder>(LoadModuleBuilder);

        private static readonly Lazy<MethodInfo> GetOrdinalsMethodInstance = new Lazy<MethodInfo>(() => typeof(IDataRecord).GetMethod("GetOrdinal"));

        private static readonly Lazy<Dictionary<Type, MethodInfo>> DataRecordGetMethodsData =
            new Lazy<Dictionary<Type, MethodInfo>>(LoadDataRecordGetMethods);

        private static readonly Lazy<Dictionary<Type, MethodInfo>> NullableGetMethodsData =
            new Lazy<Dictionary<Type, MethodInfo>>(LoadNullableGetMethods);

        private static readonly Lazy<Dictionary<Type, MethodInfo>> NullableGetMethodsWithDefaultData =
            new Lazy<Dictionary<Type, MethodInfo>>(LoadGetWithDefaultMethods);

        private static readonly Lazy<Dictionary<Type, ConstructorInfo>> SupportedConstantTypeData =
            new Lazy<Dictionary<Type, ConstructorInfo>>(LoadSupportedConstantType);

        private static readonly Lazy<CommandExtensionMethods> CommandExtensionMethodsInstance =
            new Lazy<CommandExtensionMethods>(() => new CommandExtensionMethods());

        private static AssemblyBuilder assemblyBuilder;

        public static bool EmitDebugSymbols { get; set; }

        public static ModuleBuilder ModuleBuilder => ModuleBuilderInstance.Value;

        public static Dictionary<Type, MethodInfo> DataRecordGetMethods => DataRecordGetMethodsData.Value;

        public static Dictionary<Type, MethodInfo> NullableGetMethods => NullableGetMethodsData.Value;

        public static Dictionary<Type, MethodInfo> NullableGetWithDefaultMethods => NullableGetMethodsWithDefaultData.Value;

        public static Dictionary<Type, ConstructorInfo> SupportedConstantType => SupportedConstantTypeData.Value;

        public static CommandExtensionMethods CommandExtensionMethods => CommandExtensionMethodsInstance.Value;

        public static MethodInfo GetOrdinalsMethod => GetOrdinalsMethodInstance.Value;

        public static ISymbolDocumentWriter CreateDocumentWriter(string name)
            => ModuleBuilder.DefineDocument(name, Guid.Empty, Guid.Empty, Guid.Empty);

        public static void CheckValidType(PropertyInfo destinationProperty)
        {
            var type = destinationProperty.PropertyType;
            if (DataRecordGetMethods.ContainsKey(type))
            {
                return;
            }

            if (NullableGetMethods.ContainsKey(type))
            {
                return;
            }

            throw new ArgumentOutOfRangeException(nameof(destinationProperty), $"Type {type.Name} for property {destinationProperty.Name} is not supported by IDataRecord.");
        }

        public static void SaveAssembly()
        {
            assemblyBuilder.Save(AssemblyName.Name + ".dll");
        }

        private static Dictionary<Type, MethodInfo> LoadDataRecordGetMethods()
        {
            var type = typeof(IDataRecord);
            var methods = new Dictionary<Type, MethodInfo>
                              {
                                  { typeof(bool), type.GetMethod("GetBoolean") },
                                  { typeof(byte), type.GetMethod("GetByte") },
                                  { typeof(short), type.GetMethod("GetInt16") },
                                  { typeof(int), type.GetMethod("GetInt32") },
                                  { typeof(long), type.GetMethod("GetInt64") },
                                  { typeof(char), type.GetMethod("GetChar") },
                                  { typeof(string), type.GetMethod("GetString") },
                                  { typeof(DateTime), type.GetMethod("GetDateTime") },
                                  { typeof(decimal), type.GetMethod("GetDecimal") },
                                  { typeof(float), type.GetMethod("GetFloat") },
                                  { typeof(double), type.GetMethod("GetDouble") },
                                  { typeof(Guid), type.GetMethod("GetGuid") }
                              };
            return methods;
        }

        private static Dictionary<Type, MethodInfo> LoadNullableGetMethods()
        {
            var type = typeof(DataRecordExtensions);
            var methods = new Dictionary<Type, MethodInfo>
                              {
                                  { typeof(bool?), type.GetMethod("GetNullableBoolean") },
                                  { typeof(byte?), type.GetMethod("GetNullableByte") },
                                  { typeof(short?), type.GetMethod("GetNullableInt16") },
                                  { typeof(int?), type.GetMethod("GetNullableInt32") },
                                  { typeof(long?), type.GetMethod("GetNullableInt64") },
                                  { typeof(char?), type.GetMethod("GetNullableChar") },
                                  { typeof(string), type.GetMethod("GetNullableString") },
                                  { typeof(DateTime?), type.GetMethod("GetNullableDateTime") },
                                  { typeof(decimal?), type.GetMethod("GetNullableDecimal") },
                                  { typeof(float?), type.GetMethod("GetNullableFloat") },
                                  { typeof(double?), type.GetMethod("GetNullableDouble") },
                                  { typeof(Guid?), type.GetMethod("GetNullableGuid") }
                              };
            return methods;
        }

        private static Dictionary<Type, MethodInfo> LoadGetWithDefaultMethods()
        {
            var type = typeof(DataRecordExtensions);
            var methods = new Dictionary<Type, MethodInfo>
                              {
                                  { typeof(bool?), type.GetMethod("GetBoolean") },
                                  { typeof(bool), type.GetMethod("GetBoolean") },
                                  { typeof(byte?), type.GetMethod("GetByte") },
                                  { typeof(byte), type.GetMethod("GetByte") },
                                  { typeof(short?), type.GetMethod("GetInt16") },
                                  { typeof(short), type.GetMethod("GetInt16") },
                                  { typeof(int?), type.GetMethod("GetInt32") },
                                  { typeof(int), type.GetMethod("GetInt32") },
                                  { typeof(long?), type.GetMethod("GetInt64") },
                                  { typeof(long), type.GetMethod("GetInt64") },
                                  { typeof(char?), type.GetMethod("GetChar") },
                                  { typeof(char), type.GetMethod("GetChar") },
                                  { typeof(string), type.GetMethod("GetString") },
                                  { typeof(float?), type.GetMethod("GetFloat") },
                                  { typeof(float), type.GetMethod("GetFloat") },
                                  { typeof(double?), type.GetMethod("GetDouble") },
                                  { typeof(double), type.GetMethod("GetDouble") }
                              };
            return methods;
        }

        private static Dictionary<Type, ConstructorInfo> LoadSupportedConstantType()
        {
            var constantTypes = new Dictionary<Type, ConstructorInfo>()
                                    {
                                        { typeof(bool), typeof(bool?).GetConstructor(new[] { typeof(bool) }) },
                                        { typeof(byte), typeof(byte?).GetConstructor(new[] { typeof(byte) }) },
                                        { typeof(short), typeof(short?).GetConstructor(new[] { typeof(short) }) },
                                        { typeof(int), typeof(int?).GetConstructor(new[] { typeof(int) }) },
                                        { typeof(long), typeof(long?).GetConstructor(new[] { typeof(long) }) },
                                        { typeof(char), typeof(char?).GetConstructor(new[] { typeof(char) }) },
                                        { typeof(string), null },
                                        { typeof(float), typeof(float?).GetConstructor(new[] { typeof(float) }) },
                                        { typeof(double), typeof(double?).GetConstructor(new[] { typeof(double) }) }
                                    };
            return constantTypes;
        }

        private static ModuleBuilder LoadModuleBuilder()
        {
            assemblyBuilder =
                AppDomain.CurrentDomain.DefineDynamicAssembly(
                    AssemblyName,
                    AssemblyBuilderAccess.RunAndSave);

            SetDebugAttributeIfNeeded();

            var moduleBuilder = assemblyBuilder.DefineDynamicModule(
                                                    AssemblyName.Name,
                                                    AssemblyName.Name + ".dll",
                                                    EmitDebugSymbols);

            return moduleBuilder;
        }

        private static void SetDebugAttributeIfNeeded()
        {
            if (EmitDebugSymbols == false)
            {
                return;
            }

            var debugAttributeType = typeof(DebuggableAttribute);
            var debugAttributeConstructor = FindDebugAttributeConstructor(debugAttributeType);
            var debugAttributeBuilder = CreateDebugAttributeBuilder(debugAttributeConstructor);
            assemblyBuilder.SetCustomAttribute(debugAttributeBuilder);
        }

        private static CustomAttributeBuilder CreateDebugAttributeBuilder(ConstructorInfo debugAttributeConstructor)
        {
            var constructorArgs = new object[]
                                      {
                                          DebuggableAttribute.DebuggingModes.DisableOptimizations |
                                          DebuggableAttribute.DebuggingModes.Default
                                      };
            var debugAttributeBuilder = new CustomAttributeBuilder(debugAttributeConstructor, constructorArgs);
            return debugAttributeBuilder;
        }

        private static ConstructorInfo FindDebugAttributeConstructor(Type debugAttributeType)
        {
            var types = new[]
                            {
                                typeof(DebuggableAttribute.DebuggingModes)
                            };
            var debugAttributeConstructor = debugAttributeType.GetConstructor(types);
            return debugAttributeConstructor;
        }
    }
}