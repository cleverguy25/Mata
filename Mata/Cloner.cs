namespace Mata
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    public static class Cloner
    {
        private static Dictionary<Type, Delegate> cachedIL = new Dictionary<Type, Delegate>();

        public static T CloneWithIL<T>(T source)
        {
            Delegate executeDelegate = null;
            if (cachedIL.TryGetValue(typeof(T), out executeDelegate) == false)
            {
                var dynamicMethod = new DynamicMethod("CloneWithIL", typeof(T), new Type[] { typeof(T) }, true);
                var generator = GetILGenerator(source, dynamicMethod);
                SetupCloneMethod(source, generator);
                executeDelegate = dynamicMethod.CreateDelegate(typeof(Func<T, T>));
                cachedIL.Add(typeof(T), executeDelegate);
            }
            return ((Func<T, T>)executeDelegate)(source);
        }

        private static ILGenerator GetILGenerator<T>(T source, DynamicMethod dynamicMethod)
        {
            ConstructorInfo constructorInfo = source.GetType().GetConstructor(new Type[]
                                                                                  {
                                                                                  });
            var generator = dynamicMethod.GetILGenerator();
            generator.DeclareLocal(typeof(T));
            generator.Emit(OpCodes.Newobj, constructorInfo);
            return generator;
        }

        private static void SetupCloneMethod<T>(T source, ILGenerator generator)
        {
            
            generator.Emit(OpCodes.Stloc_0);
            foreach (
                FieldInfo field in
                    source.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                generator.Emit(OpCodes.Ldloc_0);
                generator.Emit(OpCodes.Ldarg_0);
                generator.Emit(OpCodes.Ldfld, field);
                generator.Emit(OpCodes.Stfld, field);
            }

            generator.Emit(OpCodes.Ldloc_0);
            generator.Emit(OpCodes.Ret);
        }
    }
}
