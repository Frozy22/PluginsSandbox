using System.Linq;
using FrozenBox.Utils;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

namespace FrozenBox.Serialization.Editor
{
    [CustomPropertyDrawer(typeof(SerializableDictionary<,,>))]
    [CustomPropertyDrawer(typeof(SerializableDictionary<,>))]
    [CustomPropertyDrawer(typeof(SerializableRefDictionary<,>))]
    public class SerializableDictionaryDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var root = new VisualElement();

            var helpBox = new HelpBox("", HelpBoxMessageType.Error);
            root.Add(helpBox);
            
            var arrayProperty = property.FindPropertyRelative("_valuePairs");
            var arrayField = new PropertyField(arrayProperty, property.displayName);
            arrayField.BindProperty(arrayProperty);
            root.Add(arrayField);
            
            arrayField.TrackPropertyValue(arrayProperty, HandlePropertyChanged);
            HandlePropertyChanged(arrayProperty);
            
            return root;

            void HandlePropertyChanged(SerializedProperty changedProperty)
            {
                var keys = changedProperty.GetArrayElements()
                    .Select(element => element.FindPropertyRelative("_key")).ToList();

                if (keys.Any(key => key == null))
                {
                    Debug.LogError("Couldn't find property '_key' in pairProperty");
                    return;
                }

                for (var i = 0; i < keys.Count; i++)
                {
                    for (var j = i + 1; j < keys.Count; j++)
                    {
                        var firstKey = keys[i];
                        var secondKey = keys[j];
                        
                        if (IsEquals(firstKey, secondKey, out var firstString, out var secondString))
                        {
                            helpBox.text = $"Element {i}:[{firstString}] is equals of Element {j}:[{secondString}]";
                            helpBox.style.display = DisplayStyle.Flex;
                            helpBox.style.position = Position.Relative;
                            return;
                        }
                    }
                }
                
                helpBox.style.display = DisplayStyle.None;
                helpBox.style.position = Position.Absolute;
            }
        }

        private static bool IsEquals(SerializedProperty firstProperty, SerializedProperty secondProperty,
            out string firstString, out string secondString)
        {
            Assert.AreEqual(firstProperty.propertyType, secondProperty.propertyType);

            switch (firstProperty.propertyType)
            {
                case SerializedPropertyType.Enum: {
                    if (firstProperty.enumValueIndex == secondProperty.enumValueIndex)
                    {
                        var displayNames = firstProperty.enumDisplayNames;
                        firstString = displayNames[firstProperty.enumValueIndex];
                        secondString = displayNames[secondProperty.enumValueIndex];
                        return true;
                    }

                    break;
                }
                default:
                {
                    var firstValue = firstProperty.boxedValue;
                    var secondValue = secondProperty.boxedValue;
                    
                    if (firstValue?.Equals(secondValue) ?? secondValue == null)
                    {
                        firstString = firstValue?.ToString() ?? "null";
                        secondString = secondValue?.ToString() ?? "null";
                        return true;
                    }

                    break;
                }
            }

            firstString = "";
            secondString = "";
            return false;
        }
    }
}