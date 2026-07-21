using System;
using System.Diagnostics;

namespace FrozenBox.TagsSystem
{
    internal static class EnumHelper
    {
        [Conditional("UNITY_EDITOR")]
        public static void CheckDefinedFlags<T>(T value) where T : Enum
        {
            if (!IsDefinedFlags<T>()) return;
            
            var valueInt = Convert.ToInt32(value);
            if (valueInt > 0 && (valueInt & (valueInt - 1)) == 0) 
                UnityEngine.Debug.LogWarning($"Enum {typeof(T).Name} value is Flags, but is 2^n; value = {value}:{valueInt}");
            else 
                UnityEngine.Debug.LogError($"Enum {typeof(T).Name} value is Flags and not is 2^n; value = {value}:{valueInt}");
        }
        
        [Conditional("UNITY_EDITOR")]
        public static void CheckDefinedFlags<T>() where T : Enum
        {
            if (!IsDefinedFlags<T>()) return;
            UnityEngine.Debug.LogWarning($"Enum {typeof(T).Name} is Flags");
        }
        
        public static bool IsDefinedFlags<T>() where T : Enum 
            => typeof(T).IsDefined(typeof(FlagsAttribute), false);
    }
}