using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.Scripting.LifecycleManagement;
using UnityEngine;
using Object = UnityEngine.Object;

[assembly: InternalsVisibleTo("FrozenBox.TagsSystem.Editor")]

namespace FrozenBox.TagsSystem
{
    public static partial class EnumExtensions
    {
        private static FrozenDictionary<Type, TagSourceEnum> _cachedStaticSources = null!;
        private static readonly Dictionary<Type, TagSourceEnum> CachedDynamicSources = new();
        
        public static TagHandle AsTag<T>(this T value) where T : Enum
        {
            if (_cachedStaticSources.TryGetValue(typeof(T), out var source))
                return source.CreateTagFrom_Internal(value);
            
            if (!CachedDynamicSources.TryGetValue(typeof(T), out source)) {
                source = ScriptableObject.CreateInstance<TagSourceEnumDynamic<T>>();
                source.name = typeof(T).Name;
                CachedDynamicSources.Add(typeof(T), source);
            }
            
            return source.CreateTagFrom_Internal(value);
        }
        
        internal static bool IsPowerOfTwo<T>(T x) where T : Enum
            => IsPowerOfTwo(Convert.ToInt32(x));
        
        internal static bool IsPowerOfTwo(int x) 
            => x > 0 && (x & (x - 1)) == 0;

        [OnCodeInitializing]
        private static void InitCache()
        {
            _cachedStaticSources = Resources.LoadAll<TagSourceEnum>("")
                .ToFrozenDictionary(source => source.GetType().GetGenericArguments().First(), source => source);
            foreach (var (_, source) in _cachedStaticSources) source.UpdateCache_Internal();
        }

        [OnCodeDeinitializing]
        private static void DisposeCache()
        {
            foreach (var (_, source) in CachedDynamicSources)
            {
                #if UNITY_EDITOR
                if (UnityEditor.EditorUtility.IsPersistent(source)) continue;
                #endif
                Object.Destroy(source);
            }
        }
    }
}