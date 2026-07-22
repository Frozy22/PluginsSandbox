using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace FrozenBox.TagsSystem.Editor
{
    [CustomPropertyDrawer(typeof(FlagsHandle))]
    public class FlagsHandleDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            List<(int model, int view)>? modelViewCache = null;
            
            var root = new VisualElement
                { style = { flexDirection = FlexDirection.Row } };
            
            var sourceProperty = property.FindPropertyRelative("_source");
            var valueProperty = property.FindPropertyRelative("_flags");
            
            var sourceField = new PropertyField(sourceProperty)
            {
                label = property.displayName,
                style = { flexShrink = 1 }
            };
            sourceField.BindProperty(sourceProperty);

            var valueField = new MaskField() { style = { flexGrow = 1 } };
            valueField.RegisterValueChangedCallback(HandleValueChanged);
            valueField.TrackPropertyValue(valueProperty, HandleValuePropertyChanged);
            
            var warningLabel = new Label("NOT SUPPORTED FOR FLAGS") {
                style = {
                    flexGrow = 1,
                    visibility = Visibility.Hidden,
                    position = Position.Absolute
                }
            };
            
            root.Add(sourceField);
            root.Add(valueField);
            root.Add(warningLabel);
            
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

                if (!source.CanBeFlag) {
                    valueField.choices = new List<string>();
                    
                    valueField.style.visibility = Visibility.Hidden;
                    valueField.style.position = Position.Absolute;
                    
                    warningLabel.style.visibility = Visibility.Visible;
                    warningLabel.style.position = Position.Relative;
                    return;
                }

                warningLabel.style.visibility = Visibility.Hidden;
                warningLabel.style.position = Position.Absolute;
                    
                valueField.style.visibility = Visibility.Visible;
                valueField.style.position = Position.Relative;
                
                valueField.choices = source.NameToTagHash.OrderBy(pair => pair.Value._value).Select(pair => pair.Key).ToList();
                modelViewCache = valueField.choices.Select((tagName, index) => (source.NameToTagHash[tagName]._value, index)).ToList();
                HandleValuePropertyChanged(valueProperty);
            }

            void HandleValueChanged(ChangeEvent<int> evt)
            {
                if (modelViewCache == null) return;
                
                var fromValue = evt.newValue;
                var resultValue = 0;
                foreach (var (model, view) in modelViewCache) {
                    if ((fromValue & (1 << view)) != 0)
                        resultValue |= 1 << model;
                }
                valueProperty.intValue = resultValue;
                property.serializedObject.ApplyModifiedProperties();
            }

            void HandleValuePropertyChanged(SerializedProperty inValueProperty)
            {
                if (modelViewCache == null) return;

                var fromValue = inValueProperty.intValue;
                var resultValue = 0;
                foreach (var (model, view) in modelViewCache) {
                    if ((fromValue & (1 << model)) != 0)
                        resultValue |= 1 << view;
                }
                valueField.value = resultValue;
            }
        }
    }
}