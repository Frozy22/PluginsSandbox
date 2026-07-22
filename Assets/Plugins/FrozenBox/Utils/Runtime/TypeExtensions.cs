using System;

namespace FrozenBox.Utils
{
    public static class TypeExtensions
    {
        public static bool IsSubclassOf<TBase>(this Type type) 
            => type.IsSubclassOf(typeof(TBase));
        
        public static bool IsAssignableTo<TBase>(this Type type) 
            => typeof(TBase).IsAssignableFrom(type);
    }
}