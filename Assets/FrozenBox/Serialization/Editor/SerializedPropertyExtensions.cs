using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine.Assertions;

namespace FrozenBox.Serialization.Editor
{
    public static class SerializedPropertyExtensions
    {
        public static bool IsValueAssigned(this SerializedProperty property)
        {
            return property.propertyType switch
            {
                SerializedPropertyType.ObjectReference or SerializedPropertyType.ExposedReference 
                    => property.objectReferenceValue != null,
                SerializedPropertyType.ManagedReference => property.managedReferenceValue != null,
                SerializedPropertyType.String => !string.IsNullOrWhiteSpace(property.stringValue),
                _ => throw new NotImplementedException()
            };
        }

        public static SerializedProperty? GetParent(this SerializedProperty property)
        {
            var index = property.propertyPath.LastIndexOf('.');
            return index >= 0 ? property.serializedObject.FindProperty(property.propertyPath[..index]) : null;
        }

        public static SerializedProperty AppendElement(this SerializedProperty property)
        {
            Assert.IsTrue(property.isArray, "Property is not an array.");
            property.arraySize++;
            return property.GetArrayElementAtIndex(property.arraySize - 1);
        }
        
        public static IEnumerable<SerializedProperty> GetArrayElements(this SerializedProperty property)
        {
            Assert.IsTrue(property.isArray, "Property is not an array.");
            
            for (var i = 0; i < property.arraySize; i++)
                yield return property.GetArrayElementAtIndex(i);
        }
        
        public static IEnumerable<SerializedProperty> FindPropertyRelative(this IEnumerable<SerializedProperty> properties, string childPath) 
            => properties.Select(property => property.FindPropertyRelative(childPath));
        
        public static IEnumerable<SerializedProperty> GetAllChildren(this SerializedProperty property, bool useEndProperty)
        {
            var iterator = property.Copy();
            var endProperty = useEndProperty ? iterator.GetEndProperty() : null;
            var canEnterChildren = true;

            while (iterator.Next(canEnterChildren) && !SerializedProperty.EqualContents(iterator, endProperty))
            {
                yield return iterator.Copy();
                canEnterChildren = false;
            }
        }

        public static IEnumerable<SerializedProperty> GetVisibleChildren(this SerializedProperty property, bool useEndProperty)
        {
            var iterator = property.Copy();
            var endProperty = useEndProperty ? iterator.GetEndProperty() : null;
            var canEnterChildren = true;

            while (iterator.NextVisible(canEnterChildren) && !SerializedProperty.EqualContents(iterator, endProperty))
            {
                yield return iterator.Copy();
                canEnterChildren = false;
            }
        }
    }
}