using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Assertions;

namespace FrozenBox.Utils
{
    public static class TransformUtilities
    {
        public static Transform? FindChildByName(this Transform transform, string name)
        {
            return transform.GetComponentsInChildren<Transform>().FirstOrDefault(child => child.name == name);
        }
        
        public static bool FindParentInHierarchy(this Transform transform, Transform target)
        {
            FrAssert.IsNotNull(target);
            return transform.GetParents().Any(parent => parent == target);
        }

        public static IEnumerable<Transform> GetParents(this Transform transform)
        {
            var current = transform;
            
            while (current.parent != null)
            {
                yield return current;
                current = current.parent;
            }
        }
        
        public static bool IsUnitScaled(this Transform transform)
        {
            return transform.lossyScale.IsUnitScaled();
        }
    }
}