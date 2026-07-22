using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace FrozenBox.TagsSystem.Editor
{
    [CustomPropertyDrawer(typeof(TagHandle))]
    public class TagHandleDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var root = new VisualElement
                { style = { flexDirection = FlexDirection.Row } };
            
            var sourceProperty = property.FindPropertyRelative("_source");
            var valueProperty = property.FindPropertyRelative("_value");
            
            var sourceField = new PropertyField(sourceProperty)
            {
                label = property.displayName,
                style = { flexShrink = 1 }
            };
            sourceField.BindProperty(sourceProperty);

            var valueField = new DropdownField() { style = { flexGrow = 1 } };
            valueField.RegisterValueChangedCallback(HandleValueChanged);
            valueField.TrackPropertyValue(valueProperty, HandleValuePropertyChanged);

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
                HandleValuePropertyChanged(valueProperty);
            }

            void HandleValueChanged(ChangeEvent<string> evt)
            {
                var source = sourceProperty.objectReferenceValue as TagSource;
                if (source != null && source.NameToTagHash.TryGetValue(evt.newValue, out var tag))
                {
                    valueProperty.intValue = tag._value;
                    property.serializedObject.ApplyModifiedProperties();
                }
            }

            void HandleValuePropertyChanged(SerializedProperty inValueProperty)
            {
                var source = sourceProperty.objectReferenceValue as TagSource;
                if (source == null) return;

                var tag = source.CreateTag_Internal(inValueProperty.intValue);
                if (source.TagToNameHash.TryGetValue(tag, out var tagName))
                    valueField.value = tagName;
            }
        }
    }
}