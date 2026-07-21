using System;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace FrozenBox.TagsSystem
{
    public static class EnumExtensions
    {
        public static TagHandle AsTag<T>(this T value) where T : Enum
        {
            EnumHelper.CheckDefinedFlags(value);
            var valueInt = Convert.ToInt32(value);
            return new TagHandle(TagSource.From<T>(), valueInt);
        }
    }
}