using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace FrozenBox.TagsSystem.Editor
{
    [CustomPropertyDrawer(typeof(FlagsHandle))]
    public class TagsFlagsDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            List<(int model, int view)>? viewModelCache = null;
            var root = new VisualElement {
                style = {
                    flexDirection = FlexDirection.Row 
                }
            };
            
            var sourceProperty = property.FindPropertyRelative("_source");
            var sourceField = new PropertyField(sourceProperty) {
                label = property.displayName
            };
            sourceField.BindProperty(sourceProperty);
            root.Add(sourceField);

            var flagsProperty = property.FindPropertyRelative("_flags");
            var flagsField = new MaskField {
                style = {
                    flexGrow = 1,
                    minWidth = new Length(30, LengthUnit.Percent)
                }
            };
            root.Add(flagsField);
            
            root.TrackPropertyValue(sourceProperty, HandleSourceChanged);
            HandleSourceChanged(sourceProperty);
            
            flagsField.TrackPropertyValue(flagsProperty, HandleFlagsChanged);
            flagsField.RegisterValueChangedCallback(HandleValueChanged);
            
            return root;

            void HandleSourceChanged(SerializedProperty inProperty)
            {
                var source = inProperty.objectReferenceValue as TagsSource;
                
                if (source == null) {
                    viewModelCache = null;
                    flagsField.choices = new List<string>();
                    flagsField.enabledSelf = false;
                    return;
                }

                viewModelCache = source.GetRawNames().Select((tagName, index) => (tagName, index))
                    .Where(pair => !string.IsNullOrWhiteSpace(pair.tagName))
                    .Select((pair, index) => (pair.index, index)).ToList();
                
                flagsField.choices = source.GetNames().ToList();
                flagsField.enabledSelf = true;
                HandleFlagsChanged(flagsProperty);
            }

            void HandleFlagsChanged(SerializedProperty inProperty)
            {
                if (viewModelCache == null) return;
                var fromValue = inProperty.intValue;
                var resultValue = 0;
                foreach (var (model, view) in viewModelCache) {
                    if ((fromValue & (1 << model)) != 0)
                        resultValue |= 1 << view;
                }
                flagsField.value = resultValue;
            }

            void HandleValueChanged(ChangeEvent<int> changeEvent)
            {
                if (viewModelCache == null) return;
                var fromValue = changeEvent.newValue;
                var resultValue = 0;
                foreach (var (model, view) in viewModelCache) {
                    if ((fromValue & (1 << view)) != 0)
                        resultValue |= 1 << model;
                }
                flagsProperty.intValue = resultValue;
                property.serializedObject.ApplyModifiedProperties();
            }
        }
    }
}