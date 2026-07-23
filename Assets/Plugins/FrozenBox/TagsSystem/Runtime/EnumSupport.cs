using System;
using System.Collections.Frozen;
using System.Linq;
using Unity.Scripting.LifecycleManagement;
using UnityEngine;
using UnityEngine.Assertions;

namespace FrozenBox.TagsSystem
{
    public static partial class EnumSupport
    {
        [AutoStaticsCleanup]
        private static FrozenDictionary<Type, TagSourceEnum>? _cachedSources;

        [OnCodeInitializing]
        private static void InitializeCache()
        {
            _cachedSources = Resources.LoadAll<TagSourceEnum>("")
                .Where(source => source.EnumType != null)
                .ToFrozenDictionary(source => source.EnumType!, source => source);
        }

        public static TagHandle? AsTag<T>(this T value) where T : Enum
        {
            if (_cachedSources == null) return null;
            if (!_cachedSources.TryGetValue(typeof(T), out var source)) return null;
            
            Assert.IsNotNull(source.toTagHash);
            var tag = source.toTagHash![value];
            Assert.AreEqual(value.ToString(), source._tags[tag.Index]);
            return tag;
        }

        public static T? AsEnum<T>(this TagHandle tagHandle) where T : struct, Enum
        {
            if (tagHandle.Source is not TagSourceEnum source) return null;
            
            var value= source.toEnumHash[tagHandle.Index];
            Assert.AreEqual(value.ToString(), source._tags[tagHandle.Index]);
            return (T)value;
        }

        internal static bool IsPowerOfTwo<T>(T value) where T : Enum
        {
            var intValue = Convert.ToInt32(value);
            return intValue > 0 && (intValue & (intValue - 1)) == 0;
        }
    }
}