//// <copyright file="MapEmitDebugInfo.cs" company="Palador Open Source">
//// Copyright (c) Palador Open Source. All rights reserved. See License.txt in the project root for license information.
//// </copyright>

namespace MataCore.Emit
{
#if (alreadySupportCoreFramework)
    using System.Diagnostics.SymbolStore;
#endif
    using System.IO;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Text;

    public class MapEmitDebugInfo<T>
    {
        private readonly string className;

        private readonly string typeName;

        private readonly string typeNamespace;

        private StringBuilder debugBuilder;

        private int line = 1;

#if (alreadySupportCoreFramework)
        private ISymbolDocumentWriter symbolDocumentWriter;
#endif
        public MapEmitDebugInfo(string typeNamespace, string typeName, string className)
        {
            this.typeNamespace = typeNamespace;
            this.typeName = typeName;
            this.className = className;
        }

        public void CreateDebugSymbols()
        {
#if (alreadySupportCoreFramework)
            this.symbolDocumentWriter = MapEmitAssembly.CreateDocumentWriter(this.GetDebugSymbolName());
#endif
            this.debugBuilder = new StringBuilder();
        }

        public void AddDebugEndMethod(ILGenerator code)
        {
            this.AddDebugSymbolLine(code, "        }");
        }

        public void AddDebugEndFile()
        {
            this.AddDebugLine("    }");
            this.AddDebugLine("}");
            var debugFile = this.debugBuilder.ToString();
            File.WriteAllText(this.GetDebugSymbolName(), debugFile);
        }

        public void AddDebugConstructorDeclaration()
        {
            this.AddDebugLine($"        public {this.className}()");
            this.AddDebugLine("        {");
        }

        public void AddDebugTypeDeclaration()
        {
            this.AddDebugLine($"namespace {this.typeNamespace}");
            this.AddDebugLine("{");
            this.AddDebugLine($"    public class {this.className} : IMap<{typeof(T).Name}>");
            this.AddDebugLine("    {");
        }

        public void AddDebugTypeForISqlMapDeclaration()
        {
            this.AddDebugLine($"namespace {this.typeNamespace}");
            this.AddDebugLine("{");
            this.AddDebugLine($"    public class {this.className} : ISqlMap<{typeof(T).Name}>");
            this.AddDebugLine("    {");
        }

        public void AddDebugLoadDeclaration()
        {
            this.AddDebugLine(string.Empty);
            this.AddDebugLine($"        public void Load({typeof(T).Name} model, IDataRecord reader)");
            this.AddDebugLine("        {");
        }

        public void AddDebugLoadForSqlDataReaderDeclaration()
        {
            this.AddDebugLine(string.Empty);
            this.AddDebugLine($"        public void LoadSqlDataReader({typeof(T).Name} model, ISqlDataRecord reader)");
            this.AddDebugLine("        {");
        }

        public void AddDebugLoadParametersDeclaration()
        {
            this.AddDebugLine(string.Empty);
            this.AddDebugLine($"        public void LoadParameters(IDbCommand command, {typeof(T).Name} model)");
            this.AddDebugLine("        {");
        }

        public void AddDebugLoadOrdinalsDeclaration()
        {
            this.AddDebugLine(string.Empty);
            this.AddDebugLine("        public void LoadOrdinals(IDataRecord reader)");
            this.AddDebugLine("        {");
        }

        public void AddDebugGetOrdinal(ILGenerator code, FieldInfo ordinalProperty, string sourceColumn)
        {
            this.AddDebugSymbolLine(
                code,
                $"            this.{ordinalProperty.Name} = reader.GetOrdinal(\"{sourceColumn}\");");
        }

        public void AddDebugGetValue(
                                     ILGenerator code,
                                     FieldMapDefinition field,
                                     MethodInfo getMethod,
                                     FieldInfo ordinalField)
        {
            var defaultValue = GetDefaultValue(field);

            this.AddDebugSymbolLine(
                code,
                $"            model.{field.DestinationProperty.Name} = reader.{getMethod.Name}({ordinalField.Name}{defaultValue});");
        }

        public void AddDebugDeriveParameters(ILGenerator code)
        {
            this.AddDebugSymbolLine(
                code,
                "            var parameters = command.DeriveParameters();");
        }

        public void AddDebugSetParameter(ILGenerator code, FieldMapDefinition field, MethodInfo setParameterMethod, string sourceColumn)
        {
            this.AddDebugSymbolLine(
                code,
                $"            command.{setParameterMethod.Name}(parameters, \"{sourceColumn}\", model.{field.DestinationProperty.Name});");
        }

        public void AddDebugAddParameter(ILGenerator code, FieldMapDefinition field, MethodInfo addParameterMethod, string sourceColumn)
        {
            this.AddDebugSymbolLine(
                code,
                $"            command.{addParameterMethod.Name}(\"{sourceColumn}\", model.{field.DestinationProperty.Name});");
        }

        public void SetLocalVariableName(LocalBuilder parametersLocal)
        {
#if (alreadySupportCoreFramework)
            parametersLocal.SetLocalSymInfo("parameters");
#endif
        }

        private static string GetDefaultValue(FieldMapDefinition field)
        {
            var value = field.DefaultValue;
            if (value is bool)
            {
                value = value.ToString().ToLower();
            }

            if (value is string)
            {
                value = $"\"{value}\"";
            }

            if (value is char)
            {
                value = $"'{value}'";
            }

            var defaultValue = value == null ? string.Empty : $", {value}";
            return defaultValue;
        }

        private string GetDebugSymbolName()
        {
            return this.typeName + ".cs";
        }

        private void AddDebugSymbolLine(ILGenerator code, string lineText)
        {
#if (alreadySupportCoreFramework)
            if (this.symbolDocumentWriter == null)
            {
                return;
            }

            code.MarkSequencePoint(this.symbolDocumentWriter, this.line, 1, this.line, lineText.Length);
#endif
            this.AddDebugLine(lineText);
        }

        private void AddDebugLine(string lineText)
        {
            this.debugBuilder.AppendLine(lineText);
            this.line++;
        }
    }
}