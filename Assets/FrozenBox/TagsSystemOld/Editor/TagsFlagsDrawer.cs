using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace FrozenBox.TagsSystem.Editor
{
    [CustomPropertyDrawer(typeof(TagsFlags))]
    public class TagsFlagsDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var root = new VisualElement {
                style = {
                    flexDirection = FlexDirection.Row 
                }
            };
            
            var groupProperty = property.FindPropertyRelative("_group");
            var groupField = new PropertyField(groupProperty) {
                label = property.displayName
            };
            groupField.BindProperty(groupProperty);
            root.Add(groupField);

            var flagsProperty = property.FindPropertyRelative("_flags");
            var flagsField = new MaskField {
                style = {
                    flexGrow = 1,
                    minWidth = new Length(30, LengthUnit.Percent)
                }
            };
            root.Add(flagsField);
            
            flagsField.TrackPropertyValue(groupProperty, HandleGroupChanged);
            flagsField.TrackPropertyValue(flagsProperty, HandleFlagsChanged);
            flagsField.RegisterValueChangedCallback(HandleValueChanged);
            HandleGroupChanged(groupProperty);
            HandleFlagsChanged(flagsProperty);
            
            return root;

            void HandleGroupChanged(SerializedProperty inProperty)
            {
                var group = inProperty.objectReferenceValue as TagsGroup;
                
                if (group == null) {
                    flagsField.choices = new List<string>();
                    flagsField.enabledSelf = false;
                    return;
                }

                flagsField.choices = new List<string>(group.GetRawNames());
                flagsField.enabledSelf = true;
            }

            void HandleFlagsChanged(SerializedProperty inProperty)
            {
                flagsField.value = inProperty.intValue;
            }

            void HandleValueChanged(ChangeEvent<int> changeEvent)
            {
                var group = groupProperty.objectReferenceValue as TagsGroup;
                if (group == null) return;
                
                flagsProperty.intValue = changeEvent.newValue;
                flagsProperty.serializedObject.ApplyModifiedProperties();
            }
        }
    }
}