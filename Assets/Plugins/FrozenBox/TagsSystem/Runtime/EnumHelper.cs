using System;
using System.Diagnostics;

namespace FrozenBox.TagsSystem
{
    internal static class EnumHelper
    {
        public static bool IsDefinedFlags<T>() where T : Enum 
            => typeof(T).IsDefined(typeof(FlagsAttribute), false);
    }
}