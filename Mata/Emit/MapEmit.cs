// <copyright file="MapEmit.cs" company="Palador Open Source">
// Copyright (c) Palador Open Source. All rights reserved. See License.txt in the project root for license information.
// </copyright>

namespace Mata.Emit
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Reflection.Emit;
    using Mata;

    public class MapEmit<T>
    {
        private const string TypeNamespace = "SqlDataReaderMap";

        private readonly MapDefinition<T> mapDefinition;

        private readonly IMapEmitDebugInfo<T> mapEmitDebugInfo;

        private ConstructorBuilder defaultConstructor;

        private Dictionary<string, FieldInfo> fieldOrdinals;

        private MethodBuilder loadMethod;

        private MethodBuilder loadSqlDataReaderMethod;

        private MethodBuilder loadOrdinalsMethod;

        private TypeBuilder typeBuilder;

        private MethodBuilder loadParametersMethod;

        public MapEmit(MapDefinition<T> mapDefinition)
        {
            this.mapDefinition = mapDefinition;

#if !NETSTANDARD1_6
            if (MapEmitAssembly.EmitDebugSymbols)
            {
                this.mapEmitDebugInfo = new MapEmitDebugInfo<T>(TypeNamespace, this.GetTypeName(), this.GetClassName());
            }
#endif
        }

        public Func<IMap<T>> Generate()
        {
            this.mapEmitDebugInfo?.CreateDebugSymbols();
            this.CreateType();
            this.CreateFieldOrdinals();
            this.CreateDefaultConstructor();
            this.CreateLoadOrdinalsMethod();
            this.CreateLoadMethod();
            this.CreateLoadParametersMethod();
            this.ImplementInterface();

            if (this.mapDefinition.HasSqlServerSpecificFields)
            {
                this.CreateLoadSqlDataReaderMethod();
                this.ImplementSqlDataReaderInterface();
            }
            
            var type = this.typeBuilder.CreateType();
            this.mapEmitDebugInfo?.AddDebugEndFile();
            return GenerateFactoryMethod(type);
        }

        private static Func<IMap<T>> GenerateFactoryMethod(Type type)
        {
            var newObject = Expression.New(type);
            var castToMap = Expression.Convert(newObject, typeof(IMap<T>));
            return Expression.Lambda<Func<IMap<T>>>(castToMap).Compile();
        }

        private static MethodInfo GetReaderGetDataMethodForField(FieldMapDefinition field)
        {
            Dictionary<Type, MethodInfo> methodDictionary;

            if (field.IsSqlServerSpecific)
            {
                methodDictionary = MapEmitAssembly.SqlSpecificGetMethods;
                return methodDictionary[field.DestinationProperty.PropertyType];
            }

            if (field.AllowNulls)
            {
                methodDictionary = field.DefaultValue == null ? MapEmitAssembly.NullableGetMethods : MapEmitAssembly.NullableGetWithDefaultMethods;
            }
            else
            {
                methodDictionary = MapEmitAssembly.DataRecordGetMethods;
            }

            return methodDictionary[field.DestinationProperty.PropertyType];
        }

        private static void EmitDefaultValueConstant(ILGenerator code, FieldMapDefinition field)
        {
            if (field.DefaultValue == null)
            {
                return;
            }

            var defaultValueType = field.DefaultValue.GetType();
            if (defaultValueType == typeof(bool))
            {
                if ((bool)field.DefaultValue)
                {
                    code.Emit(OpCodes.Ldc_I4_1);
                }
                else
                {
                    code.Emit(OpCodes.Ldc_I4_0);
                }
            }
            else if (defaultValueType == typeof(byte))
            {
                code.Emit(OpCodes.Ldc_I4, (int)(byte)field.DefaultValue);
            }
            else if (defaultValueType == typeof(short))
            {
                code.Emit(OpCodes.Ldc_I4, (int)(short)field.DefaultValue);
            }
            else if (defaultValueType == typeof(int))
            {
                code.Emit(OpCodes.Ldc_I4, (int)field.DefaultValue);
            }
            else if (defaultValueType == typeof(long))
            {
                code.Emit(OpCodes.Ldc_I8, (long)field.DefaultValue);
            }
            else if (defaultValueType == typeof(char))
            {
                code.Emit(OpCodes.Ldc_I4_S, (char)field.DefaultValue);
            }
            else if (defaultValueType == typeof(string))
            {
                code.Emit(OpCodes.Ldstr, (string)field.DefaultValue);
            }
            else if (defaultValueType == typeof(float))
            {
                code.Emit(OpCodes.Ldc_R4, (float)field.DefaultValue);
            }
            else if (defaultValueType == typeof(double))
            {
                code.Emit(OpCodes.Ldc_R8, (double)field.DefaultValue);
            }
        }

        private static void WrapWithNullableIfNeeded(ILGenerator code, FieldMapDefinition field)
        {
            var propertyType = field.DestinationProperty.PropertyType;
            if (field.DefaultValue == null || propertyType.IsNullableType() == false)
            {
                return;
            }

            var constructor = MapEmitAssembly.SupportedConstantType[propertyType.GetGenericArguments()[0]];
            code.Emit(OpCodes.Newobj, constructor);
        }

        private void CreateType()
        {
            var typeName = this.GetTypeName();
            this.typeBuilder = MapEmitAssembly.ModuleBuilder.DefineType(
                typeName,
                TypeAttributes.Public);

            if (this.mapDefinition.HasSqlServerSpecificFields)
            {
                this.typeBuilder.AddInterfaceImplementation(typeof(ISqlMap<T>));
                this.mapEmitDebugInfo?.AddDebugTypeForISqlMapDeclaration();
            }
            else
            {
                this.typeBuilder.AddInterfaceImplementation(typeof(IMap<T>));
                this.mapEmitDebugInfo?.AddDebugTypeDeclaration();
            }
        }

        private string GetTypeName()
        {
            return $"{TypeNamespace}.{this.GetClassName()}";
        }

        private string GetClassName()
        {
            return typeof(T).Name + this.mapDefinition.UniqueId.Replace("-", string.Empty);
        }

        private void CreateDefaultConstructor()
        {
            this.defaultConstructor = this.typeBuilder.DefineConstructor(
                MethodAttributes.Public,
                CallingConventions.Standard,
                Type.EmptyTypes);

            this.EmitConstructor();
        }

        private void EmitConstructor()
        {
            this.mapEmitDebugInfo?.AddDebugConstructorDeclaration();
            var code = this.defaultConstructor.GetILGenerator();

            this.mapEmitDebugInfo?.AddDebugEndMethod(code);
            code.Emit(OpCodes.Ret);
        }

        private void CreateFieldOrdinals()
        {
            this.fieldOrdinals = new Dictionary<string, FieldInfo>();
            foreach (var field in this.mapDefinition.FieldMapDefinitions)
            {
                var sourceColumn = field.Value.SourceColumn;
                var fieldInfo = this.typeBuilder.DefineField(
                    sourceColumn,
                    typeof(int),
                    FieldAttributes.Public);

                this.fieldOrdinals[sourceColumn] = fieldInfo;
            }
        }

        private void ImplementInterface()
        {
            var loadOrdinalsInterface = typeof(IMap<T>).GetMethod("LoadOrdinals");
            this.typeBuilder.DefineMethodOverride(this.loadOrdinalsMethod, loadOrdinalsInterface);

            var loadInterface = typeof(IMap<T>).GetMethod("Load");
            this.typeBuilder.DefineMethodOverride(this.loadMethod, loadInterface);

            var loadParametersInterface = typeof(IMap<T>).GetMethod("LoadParameters");
            this.typeBuilder.DefineMethodOverride(this.loadParametersMethod, loadParametersInterface);
        }

        private void ImplementSqlDataReaderInterface()
        {
            var loadSqlReaderInterface = typeof(ISqlMap<T>).GetMethod("LoadSqlDataReader");
            this.typeBuilder.DefineMethodOverride(this.loadSqlDataReaderMethod, loadSqlReaderInterface);
        }

        private void CreateLoadOrdinalsMethod()
        {
            var parameterTypes = new[] { typeof(IDataRecord) };
            this.loadOrdinalsMethod = this.DefineVoidMethod("LoadOrdinals", parameterTypes);

            this.loadOrdinalsMethod.DefineParameter(1, ParameterAttributes.None, "reader");
            this.EmitLoadOrdinals();
        }

        private void CreateLoadMethod()
        {
            var parameterTypes = new[] { typeof(T), typeof(IDataRecord) };
            this.loadMethod = this.DefineVoidMethod("Load", parameterTypes);

            this.loadMethod.DefineParameter(1, ParameterAttributes.None, "model");
            this.loadMethod.DefineParameter(2, ParameterAttributes.None, "reader");

            this.EmitLoadMethod();
        }

        private void CreateLoadParametersMethod()
        {
            var parameterTypes = new[] { typeof(IDbCommand), typeof(T) };
            this.loadParametersMethod = this.DefineVoidMethod("LoadParameters", parameterTypes);

            this.loadParametersMethod.DefineParameter(1, ParameterAttributes.None, "command");
            this.loadParametersMethod.DefineParameter(2, ParameterAttributes.None, "model");

            this.EmitLoadParametersMethod();
        }

        private void CreateLoadSqlDataReaderMethod()
        {
            var parameterTypes = new[] { typeof(T), typeof(ISqlDataRecord) };
            this.loadSqlDataReaderMethod = this.DefineVoidMethod("LoadSqlDataReader", parameterTypes);

            this.loadSqlDataReaderMethod.DefineParameter(1, ParameterAttributes.None, "model");
            this.loadSqlDataReaderMethod.DefineParameter(2, ParameterAttributes.None, "reader");

            this.EmitLoadSqlDataReaderMethod();
        }

        private MethodBuilder DefineVoidMethod(string methodName, Type[] parameterTypes)
        {
            return this.typeBuilder.DefineMethod(
                methodName,
                MethodAttributes.Public | MethodAttributes.NewSlot | MethodAttributes.Virtual | MethodAttributes.Final,
                typeof(void),
                parameterTypes);
        }

        private void EmitLoadMethod()
        {
            this.mapEmitDebugInfo?.AddDebugLoadDeclaration();
            var code = this.loadMethod.GetILGenerator();

            foreach (var field in this.mapDefinition.FieldMapDefinitions.Where(fieldDefinition =>
                fieldDefinition.Value.IsSqlServerSpecific == false))
            {
                this.EmitGetValue(code, field.Value);
            }

            this.mapEmitDebugInfo?.AddDebugEndMethod(code);
            code.Emit(OpCodes.Ret);
        }

        private void EmitLoadSqlDataReaderMethod()
        {
            this.mapEmitDebugInfo?.AddDebugLoadForSqlDataReaderDeclaration();
            var code = this.loadSqlDataReaderMethod.GetILGenerator();

            foreach (var field in this.mapDefinition.FieldMapDefinitions.Where(fieldDefinition =>
                fieldDefinition.Value.IsSqlServerSpecific == true))
            {
                this.EmitGetValue(code, field.Value);
            }

            this.mapEmitDebugInfo?.AddDebugEndMethod(code);
            code.Emit(OpCodes.Ret);
        }

        private void EmitLoadParametersMethod()
        {
            this.mapEmitDebugInfo?.AddDebugLoadParametersDeclaration();
            var code = this.loadParametersMethod.GetILGenerator();

            LocalBuilder parametersLocal = null;
            var deriveParameters = this.mapDefinition.DeriveParameters;
            if (deriveParameters)
            {
                parametersLocal = this.EmitDeriveParameters(code);
            }

            foreach (var field in this.mapDefinition.FieldMapDefinitions)
            {
                this.EmitParameter(field.Value, code, parametersLocal);
            }

            this.mapEmitDebugInfo?.AddDebugEndMethod(code);
            code.Emit(OpCodes.Ret);
        }

        private void EmitParameter(
            FieldMapDefinition fieldDefinition,
            ILGenerator code,
            LocalBuilder parametersLocal)
        {
            var sourceColumn = fieldDefinition.SourceColumn;
            sourceColumn = sourceColumn.StartsWith("@") ? sourceColumn : $"@{sourceColumn}";
            if (parametersLocal == null)
            {
                this.EmitAddParameter(code, fieldDefinition, sourceColumn);
            }
            else
            {
                this.EmitSetParameter(code, fieldDefinition, parametersLocal, sourceColumn);
            }
        }

        private LocalBuilder EmitDeriveParameters(ILGenerator code)
        {
            this.mapEmitDebugInfo?.AddDebugDeriveParameters(code);
            var parametersLocal = code.DeclareLocal(typeof(ParameterSet));

            this.mapEmitDebugInfo?.SetLocalVariableName(parametersLocal);
            code.Emit(OpCodes.Ldarg_1);
            code.Emit(OpCodes.Call, MapEmitAssembly.CommandExtensionMethods.DeriveParameters);
            code.Emit(OpCodes.Stloc, parametersLocal);
            return parametersLocal;
        }

        private void EmitGetValue(ILGenerator code, FieldMapDefinition field)
        {
            var ordinalField = this.fieldOrdinals[field.SourceColumn];
            var getMethod = GetReaderGetDataMethodForField(field);

            this.mapEmitDebugInfo?.AddDebugGetValue(code, field, getMethod, ordinalField);
            code.Emit(OpCodes.Ldarg_1);
            code.Emit(OpCodes.Ldarg_2);
            code.Emit(OpCodes.Ldarg_0);
            code.Emit(OpCodes.Ldfld, ordinalField);

            EmitDefaultValueConstant(code, field);

            ////code.EmitCall(field.AllowNulls || field.IsSqlServerSpecific ? OpCodes.Call : OpCodes.Callvirt, getMethod, null);
            code.EmitCall(getMethod.IsVirtual ? OpCodes.Callvirt : OpCodes.Call, getMethod, null);

            WrapWithNullableIfNeeded(code, field);

            code.EmitCall(OpCodes.Callvirt, field.DestinationProperty.GetSetMethod(), null);
        }

        private void EmitSetParameter(ILGenerator code, FieldMapDefinition field, LocalBuilder parametersLocal, string sourceColumn)
        {
            var destinationProperty = field.DestinationProperty;
            var propertyType = destinationProperty.PropertyType;
            var commandMethods = MapEmitAssembly.CommandExtensionMethods;
            var setParameterMethod = field.AllowNulls ? commandMethods.SetParameterOrDbNull : commandMethods.SetParameter;

            this.mapEmitDebugInfo?.AddDebugSetParameter(code, field, setParameterMethod, sourceColumn);
            code.Emit(OpCodes.Ldarg_1);
            code.Emit(OpCodes.Ldloc, parametersLocal);
            code.Emit(OpCodes.Ldstr, sourceColumn);
            code.Emit(OpCodes.Ldarg_2);
            code.Emit(OpCodes.Callvirt, destinationProperty.GetMethod);
            
            if (propertyType.IsValueType())
            {
                code.Emit(OpCodes.Box, propertyType);
            }

            code.Emit(OpCodes.Call, setParameterMethod);
        }

        private void EmitAddParameter(ILGenerator code, FieldMapDefinition field, string sourceColumn)
        {
            var destinationProperty = field.DestinationProperty;
            var propertyType = destinationProperty.PropertyType;
            var commandMethods = MapEmitAssembly.CommandExtensionMethods;
            var addParameterMethod = field.AllowNulls ? commandMethods.AddParameterOrDbNull : commandMethods.AddParameter;

            this.mapEmitDebugInfo?.AddDebugAddParameter(code, field, addParameterMethod, sourceColumn);
            code.Emit(OpCodes.Ldarg_1);
            code.Emit(OpCodes.Ldstr, sourceColumn);
            code.Emit(OpCodes.Ldarg_2);
            code.Emit(OpCodes.Callvirt, destinationProperty.GetMethod);
            
            if (propertyType.IsValueType())
            {
                code.Emit(OpCodes.Box, propertyType);
            }

            code.Emit(OpCodes.Call, addParameterMethod);
        }

        private void EmitLoadOrdinals()
        {
            this.mapEmitDebugInfo?.AddDebugLoadOrdinalsDeclaration();
            var code = this.loadOrdinalsMethod.GetILGenerator();

            foreach (var fieldOrdinal in this.fieldOrdinals)
            {
                this.EmitGetOrdinal(code, fieldOrdinal.Key, fieldOrdinal.Value);
            }

            this.mapEmitDebugInfo?.AddDebugEndMethod(code);
            code.Emit(OpCodes.Ret);
        }

        private void EmitGetOrdinal(
            ILGenerator code,
            string sourceColumn,
            FieldInfo ordinalProperty)
        {
            this.mapEmitDebugInfo?.AddDebugGetOrdinal(code, ordinalProperty, sourceColumn);
            code.Emit(OpCodes.Ldarg_0);
            code.Emit(OpCodes.Ldarg_1);
            code.Emit(OpCodes.Ldstr, sourceColumn);
            code.EmitCall(OpCodes.Callvirt, MapEmitAssembly.GetOrdinalsMethod, null);
            code.Emit(OpCodes.Stfld, ordinalProperty);
        }
    }
}