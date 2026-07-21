using System.Collections.Generic;
using System.Linq;
using FrozenBox.Utils;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace FrozenBox.TagsSystem.Editor
{
    [CustomPropertyDrawer(typeof(Tag))]
    public class TagDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            var groupProperty = property.FindPropertyRelative("_group");
            var tagProperty = property.FindPropertyRelative("_index");

            var groupRect = position.WithWidth(position.width * 0.67f, Alignment.CenterLeft);
            groupProperty.objectReferenceValue = EditorGUI.ObjectField(groupRect, label, groupProperty.objectReferenceValue, typeof(TagsGroup), false);
            var group = groupProperty.objectReferenceValue as TagsGroup;
            
            var tagRect = position.WithWidth(position.width - groupRect.width, Alignment.CenterRight);
            
            if (group != null) {
                var choices = group.GetNames().ToArray();
                var value = ArrayUtility.IndexOf(choices, group.NameOfIndex(tagProperty.intValue));
                tagProperty.intValue = group.IndexOfName(choices[EditorGUI.Popup(tagRect, value, choices)]);
            }
            
            EditorGUI.EndProperty();
        }
        
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var root = new VisualElement {
                style = {
                    flexDirection = FlexDirection.Row 
                }
            };
            
            var groupProperty = property.FindPropertyRelative("_group");
            var tagProperty = property.FindPropertyRelative("_index");
            
            var groupField = new PropertyField(groupProperty) {
                label = property.displayName
            };
            groupField.BindProperty(groupProperty);
            root.Add(groupField);

            var tagField = new DropdownField {
                style = {
                    flexGrow = 1,
                    minWidth = new Length(30, LengthUnit.Percent)
                }
            };
            root.Add(tagField);
            
            tagField.TrackPropertyValue(groupProperty, HandleGroupChanged);
            tagField.TrackPropertyValue(tagProperty, HandleTagChanged);
            tagField.RegisterValueChangedCallback(HandleValueChanged);
            HandleGroupChanged(groupProperty);
            HandleTagChanged(tagProperty);
            
            return root;

            void HandleGroupChanged(SerializedProperty inProperty)
            {
                var group = inProperty.objectReferenceValue as TagsGroup;
                
                if (group == null) {
                    tagField.choices = new List<string>();
                    tagField.enabledSelf = false;
                    return;
                }

                tagField.choices = new List<string>(group.GetNames());
                tagField.enabledSelf = true;
            }

            void HandleTagChanged(SerializedProperty inProperty)
            {
                var group = groupProperty.objectReferenceValue as TagsGroup;
                tagField.value = group?.NameOfIndex(inProperty.intValue) ?? tagField.value;
            }

            void HandleValueChanged(ChangeEvent<string> changeEvent)
            {
                var group = groupProperty.objectReferenceValue as TagsGroup;
                if (group == null) return;
                if (!group.HasName(changeEvent.newValue)) return;
                
                tagProperty.intValue = group.IndexOfName(changeEvent.newValue);
                tagProperty.serializedObject.ApplyModifiedProperties();
            }
        }
    }
}