using System;
using System.Collections.Generic;
using System.Linq;

namespace FrozenBox.Utils
{
    public static class LinqExtensions
    {
        public static bool OnlyOne<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            var actuallyFound = false;
            
            foreach (var value in source)
            {
                if (predicate(value))
                {
                    if (actuallyFound)
                        return false;
                    
                    actuallyFound = true;
                }
            }

            return actuallyFound;
        }

        public static IEnumerable<T> NotNull<T>(this IEnumerable<T> source)
            => source.Where(element => element != null);
    }
}