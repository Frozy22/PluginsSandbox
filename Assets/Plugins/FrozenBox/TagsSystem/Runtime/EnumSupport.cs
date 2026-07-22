using System;
using System.Collections.Frozen;
using System.Linq;
using Unity.Scripting.LifecycleManagement;
using UnityEngine;

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
            if (!_cachedSources?.TryGetValue(typeof(T), out var source) ?? true)
                return null;

            var intValue = Convert.ToInt32(value);
            return source.EnumConvertType switch
            {
                TagSourceEnum.ConvertType.FLAGS => new TagHandle(source, source.IndexOfName(value.ToString())),
                TagSourceEnum.ConvertType.DIRECT => new TagHandle(source, intValue),
                TagSourceEnum.ConvertType.SEQUENCE => new TagHandle(source, source.IndexOfName(value.ToString())),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public static T? AsEnum<T>(this TagHandle tagHandle) where T : struct, Enum
        {
            if (tagHandle.Source is not TagSourceEnum source) return null;
            
            return source.EnumConvertType switch
            {
                TagSourceEnum.ConvertType.FLAGS => Enum.Parse<T>(source.NameOfTag(tagHandle), true),
                TagSourceEnum.ConvertType.DIRECT => (T)(object)tagHandle.Index,
                TagSourceEnum.ConvertType.SEQUENCE => Enum.Parse<T>(source.NameOfTag(tagHandle), true),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}