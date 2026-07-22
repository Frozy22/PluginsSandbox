using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace FrozenBox.TagsSystem.Editor
{
    [CustomPropertyDrawer(typeof(TagsSet))]
    public class TagsSetDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            Dictionary<int, int>? modelViewCache = null;
            
            var root = new VisualElement
                { style = { flexDirection = FlexDirection.Row } };
            
            var sourceProperty = property.FindPropertyRelative("_source");
            var valuesProperty = property.FindPropertyRelative("_tags");
            
            var sourceField = new PropertyField(sourceProperty)
            {
                label = property.displayName,
                style = { flexShrink = 1 }
            };
            sourceField.BindProperty(sourceProperty);

            var valueField = new Mask64Field() { style = { flexGrow = 1 } };
            valueField.RegisterValueChangedCallback(HandleValueChanged);
            valueField.TrackPropertyValue(valuesProperty, HandleValuePropertyChanged);

            root.Add(sourceField);
            root.Add(valueField);
            
            root.TrackPropertyValue(sourceProperty, HandleSourceChanged);
            HandleSourceChanged(sourceProperty);
            return root;

            void HandleSourceChanged(SerializedProperty inSourceProperty)
            {
                var source = inSourceProperty.objectReferenceValue as TagSource;
                if (source == null) {
                    valueField.choices = new List<string>();
                    return;
                }

                valueField.choices = source.NameToTagHash.OrderBy(pair => pair.Value._value).Select(pair => pair.Key).ToList();
                modelViewCache = valueField.choices.Select((tagName, index) => (source.NameToTagHash[tagName]._value, index))
                    .ToDictionary(pair => pair._value, pair => pair.index);
                HandleValuePropertyChanged(valuesProperty);
            }

            void HandleValueChanged(ChangeEvent<ulong> evt)
            {
                if (modelViewCache == null) return;
                
                var fromValue = evt.newValue;
                valuesProperty.ClearArray();
                foreach (var (model, view) in modelViewCache) {
                    if ((fromValue & (1u << view)) != 0) {
                        valuesProperty.arraySize++;
                        valuesProperty.GetArrayElementAtIndex(valuesProperty.arraySize - 1).intValue = model;
                    }
                }
                property.serializedObject.ApplyModifiedProperties();
            }

            void HandleValuePropertyChanged(SerializedProperty inValuesProperty)
            {
                if (modelViewCache == null) return;

                var resultValue = 0uL;
                for (var i = 0; i < inValuesProperty.arraySize; i++)
                {
                    var value = inValuesProperty.GetArrayElementAtIndex(i).intValue;
                    resultValue |= 1u << modelViewCache[value];
                }
                
                valueField.value = resultValue;
            }
        }
    }
}