using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FrozenBox.Utils
{
    public static class TransformUtilities
    {
        public static bool FindParentInHierarchy(this Transform transform, Transform target)
        {
            FrAssert.IsNotNull(target);
            return transform.GetParents().Any(parent => parent == target);
        }

        private static IEnumerable<Transform> GetParents(this Transform transform)
        {
            var current = transform;
            while (current.parent != null) {
                yield return current;
                current = current.parent;
            }
        }
        
        public static bool IsUnitScaled(this Transform transform) 
            => transform.lossyScale.IsUnitScaled();
    }
}