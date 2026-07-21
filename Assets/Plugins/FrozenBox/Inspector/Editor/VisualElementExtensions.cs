using System;
using UnityEngine.UIElements;

namespace FrozenBox.Utils
{
    internal static class VisualElementExtensions
    {
        public static VisualElement? QAncestor(this VisualElement visualElement, 
            string? name = null, string? className = null)
        {
            var iterator = visualElement.parent;
            
            while (iterator.parent != null)
            {
                if ((name == null || iterator.name == name) && (className == null || iterator.ClassListContains(className)))
                    return iterator;
                
                iterator = iterator.parent;
            }

            return null;
        }
    }
}