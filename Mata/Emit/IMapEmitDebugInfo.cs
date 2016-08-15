namespace Mata.Emit
{
    using System.Reflection;
    using System.Reflection.Emit;

    public interface IMapEmitDebugInfo<T>
    {
        void CreateDebugSymbols();

        void AddDebugEndMethod(ILGenerator code);

        void AddDebugEndFile();

        void AddDebugConstructorDeclaration();

        void AddDebugTypeDeclaration();

        void AddDebugTypeForISqlMapDeclaration();

        void AddDebugLoadDeclaration();

        void AddDebugLoadForSqlDataReaderDeclaration();

        void AddDebugLoadParametersDeclaration();

        void AddDebugLoadOrdinalsDeclaration();

        void AddDebugGetOrdinal(
            ILGenerator code,
            FieldInfo ordinalProperty,
            string sourceColumn);

        void AddDebugGetValue(
            ILGenerator code,
            FieldMapDefinition field,
            MethodInfo getMethod,
            FieldInfo ordinalField);
        
        void AddDebugDeriveParameters(ILGenerator code);

        void AddDebugSetParameter(
            ILGenerator code,
            FieldMapDefinition field,
            MethodInfo setParameterMethod,
            string sourceColumn);

        void AddDebugAddParameter(
            ILGenerator code,
            FieldMapDefinition field,
            MethodInfo addParameterMethod,
            string sourceColumn);

        void SetLocalVariableName(LocalBuilder parametersLocal);
    }
}
