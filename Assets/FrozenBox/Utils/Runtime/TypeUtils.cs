using System;
using System.Collections.Generic;
using System.Reflection;

namespace FrozenBox.Utils.Runtime
{
    public class TypeUtils
    {
        public static List<Type> GetAllDerivedTypes(Type baseType)
        {
            var result = new List<Type>();
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in assemblies) {
                Type[]? types = null;

                try {
                    types = assembly.GetTypes();
                }catch (ReflectionTypeLoadException e) {
                    types = e.Types;
                }catch (Exception) {
                    continue;
                }

                if (types == null) continue;

                foreach (var type in types) {
                    if (!type.IsClass || type.IsAbstract) continue;
                    if (baseType.IsAssignableFrom(type) && type != baseType) result.Add(type);
                }
            }

            return result;
        }
    }
}