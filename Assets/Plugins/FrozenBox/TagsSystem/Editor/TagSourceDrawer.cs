using System;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace FrozenBox.TagsSystem.Editor
{
    [CustomPropertyDrawer(typeof(TagSource))]
    internal class TagSourceDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var root = new VisualElement {
                style = {
                    flexDirection = FlexDirection.Row,
                }
            };
            var sourceTypeProperty = property.FindPropertyRelative("_sourceType");
            var enumTypeNameProperty = property.FindPropertyRelative("_enumTypeName");
            var tagsGroupProperty = property.FindPropertyRelative("_tagsGroup");
            
            var sourceTypeField = new PropertyField(sourceTypeProperty, property.displayName)
                { style = { flexGrow = 2, flexShrink = 1 } };
            sourceTypeField.AddToClassList("unity-base-field__aligned");
            sourceTypeField.BindProperty(sourceTypeProperty);
            
            var enumTypeNames = TypeCache.GetTypesDerivedFrom<Enum>()
                .Where(ValidateEnumType).Select(type => type.AssemblyQualifiedName).ToList();

            var enumTypeNameField = new DropdownField(enumTypeNames, enumTypeNames.FirstOrDefault(), 
                    FormatEnumType, FormatEnumType) 
                { style = { flexGrow = 1, flexShrink = 10 } };
            enumTypeNameField.BindProperty(enumTypeNameProperty);
            enumTypeNameField.Q(null, "unity-text-element")
                .style.unityTextAlign = new StyleEnum<TextAnchor>(TextAnchor.MiddleRight);
            
            var tagsGroupField = new PropertyField(tagsGroupProperty, "") 
                { style = { flexGrow = 1, flexShrink = 10 } };
            tagsGroupField.BindProperty(tagsGroupProperty);
            
            root.Add(sourceTypeField);
            root.Add(enumTypeNameField);
            root.Add(tagsGroupField);

            root.TrackPropertyValue(sourceTypeProperty, HandleSourceTypeChanged);
            HandleSourceTypeChanged(sourceTypeProperty);
            return root;

            void HandleSourceTypeChanged(SerializedProperty serializedProperty)
            {
                var sourceType = (TagSourceType)serializedProperty.intValue;
                switch (sourceType)
                {
                    case TagSourceType.ENUM:
                        enumTypeNameField.style.visibility = Visibility.Visible;
                        enumTypeNameField.style.position = Position.Relative;
                        tagsGroupField.style.visibility = Visibility.Hidden;
                        tagsGroupField.style.position = Position.Absolute;
                        break;
                    
                    case TagSourceType.ASSET:
                        enumTypeNameField.style.visibility = Visibility.Hidden;
                        enumTypeNameField.style.position = Position.Absolute;
                        tagsGroupField.style.visibility = Visibility.Visible;
                        tagsGroupField.style.position = Position.Relative;
                        break;
                    
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private string FormatEnumType(string enumTypeName)
        {
            if (string.IsNullOrWhiteSpace(enumTypeName)) return enumTypeName;
            return enumTypeName[..enumTypeName.IndexOf(',')];
        }

        private bool ValidateEnumType(Type type)
        {
            return type.IsDefined(typeof(TagEnumAttribute), false) || TagEnumsRegistry.Contains(type);
        }
    }
}