using System;
using UnityEngine.UIElements;

namespace FrozenBox.Utils
{
    public static class VisualElementExtensions
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

        public static VisualElement? QAncestor<T>(this VisualElement visualElement, 
            string? name = null, string? className = null) where T : VisualElement
        {
            var iterator = visualElement.parent;
            
            while (iterator.parent != null)
            {
                if (iterator is T element && (name == null || iterator.name == name) 
                                          && (className == null || iterator.ClassListContains(className)))
                    return element;
                
                iterator = iterator.parent;
            }

            return null;
        }

        public static void SetClassPresence(this VisualElement visualElement, string className, bool isPresence)
        { 
            if (isPresence)
                visualElement.AddToClassList(className);
            else 
                visualElement.RemoveFromClassList(className);
        }
    }
}