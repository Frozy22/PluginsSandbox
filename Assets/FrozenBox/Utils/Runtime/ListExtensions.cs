using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrozenBox.Utils
{
    public static class ListExtensions
    {
        public static bool IsEmpty(this ICollection collection) => collection.Count == 0;
        public static bool IsNotEmpty(this ICollection collection) => collection.Count > 0;
        
        public static void Shuffle<T>(this IList<T> list)
        {
            for (var i = 0; i < list.Count; i++)
            {
                var newIdx = Random.Range(0, list.Count);
                (list[i], list[newIdx]) = (list[newIdx], list[i]);
            }
        }
    }
}