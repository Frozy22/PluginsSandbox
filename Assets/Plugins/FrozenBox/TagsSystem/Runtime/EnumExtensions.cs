using System;
using System.Diagnostics;
using System.Linq;
using UnityEngine.Assertions;
using Debug = UnityEngine.Debug;

namespace FrozenBox.TagsSystem
{
    public static class EnumExtensions
    {
        public static TagHandle AsTag<T>(this T value) where T : Enum
        {
            CheckEnumSupported<T>();
            var valueInt = Convert.ToInt32(value);
            return new TagHandle(TagSource.From<T>(), valueInt);
        }

        [Conditional("UNITY_EDITOR")]
        private static void CheckEnumSupported<T>() where T : Enum
        {
            Assert.IsFalse(EnumHelper.IsDefinedFlags<T>(), "Tags not supported flags enum");
            Assert.IsFalse(Enum.GetValues(typeof(T)).Cast<T>().Any(value => Convert.ToInt64(value) > 32));
        }
    }
}