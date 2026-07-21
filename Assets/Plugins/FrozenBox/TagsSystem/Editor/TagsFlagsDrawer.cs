using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace FrozenBox.TagsSystem.Editor
{
    [CustomPropertyDrawer(typeof(TagsFlags))]
    internal class TagsFlagsDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var root = new Foldout { text = "" };
            root.Q<VisualElement>(null, "unity-toggle").style.marginBottom = 0;
            
            var sourceProperty = property.FindPropertyRelative("_source");
            var sourceTypeProperty = sourceProperty.FindPropertyRelative("_sourceType");
            var enumTypeNameProperty = sourceProperty.FindPropertyRelative("_enumTypeName");
            var tagsGroupProperty = sourceProperty.FindPropertyRelative("_tagsGroup");
            var flagsProperty = property.FindPropertyRelative("_flags");

            var sourceField = new PropertyField(sourceProperty);
            sourceField.BindProperty(sourceProperty);

            EnumFlagsField? enumFlagsField = null;
            MaskField? maskField = null;
            Dictionary<int, int>? modelToView, viewToModel;
            
            var foldoutToggle = root.Q<Toggle>(className: "unity-foldout__toggle");
            
            foldoutToggle.Q<VisualElement>(className: BaseField<object>.inputUssClassName)
                .RemoveFromClassList(BaseField<object>.inputUssClassName);
            
            root.contentContainer.Add(sourceField);
            
            root.TrackPropertyValue(sourceTypeProperty, HandleSourceTypeChanged);
            root.TrackPropertyValue(enumTypeNameProperty, HandleEnumTypeNameChanged);

            return root;

            void HandleSourceTypeChanged(SerializedProperty serializedProperty)
            {
                var type = (TagSourceType)serializedProperty.intValue;
                switch (type)
                {
                    case TagSourceType.ENUM:
                        HandleEnumTypeNameChanged(enumTypeNameProperty);
                        break;
                    
                    case TagSourceType.ASSET:
                        DisposeElement(enumFlagsField);
                        enumFlagsField = null;
                        
                        CreateMaskField();
                        FillAssetChoices(tagsGroupProperty);
                        break;
                    
                    case TagSourceType.INVALID:
                        DisposeElement(enumFlagsField);
                        enumFlagsField = null;

                        DisposeElement(maskField);
                        maskField = null;
                        break;
                    
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            
            void CreateFlagsField()
            {
                if (enumFlagsField != null) return; 
                enumFlagsField = new EnumFlagsField(property.displayName)
                    { style = { flexGrow = 1, marginLeft = 0, marginRight = 0 } };
                enumFlagsField.labelElement.style.marginLeft = 0;
                enumFlagsField.AddToClassList(BaseField<object>.alignedFieldUssClassName);
            
                maskField.TrackPropertyValue(sourceProperty, FillChoices);
                maskField.TrackPropertyValue(flagsProperty, HandleFlagsChanged);
                maskField.RegisterValueChangedCallback(HandleMaskChanged);
                FillChoices(sourceTypeProperty);
                foldoutToggle.Add(maskField);
            }

            void CreateMaskField()
            {
                if (maskField != null) return;
                maskField = new MaskField(property.displayName) 
                    { style = { flexGrow = 1, marginLeft = 0, marginRight = 0 } };
                maskField.labelElement.style.marginLeft = 0;
                maskField.AddToClassList(BaseField<object>.alignedFieldUssClassName);
            
                maskField.TrackPropertyValue(sourceProperty, FillChoices);
                maskField.TrackPropertyValue(flagsProperty, HandleFlagsChanged);
                maskField.RegisterValueChangedCallback(HandleMaskChanged);
                FillChoices(sourceTypeProperty);
                foldoutToggle.Add(maskField);
            }
            
            void HandleEnumTypeNameChanged(SerializedProperty serializedProperty)
            {
                
            }

            void FillAssetChoices(SerializedProperty serializedProperty)
            {
                if (maskField == null) return;
                
                var asset = serializedProperty.objectReferenceValue as TagsGroup;
                if (asset == null) {
                    maskField.choices = new List<string>();
                    return;
                }

                modelToView = asset.GetRawNames().Select((name, index) => (index, name))
                    .Where(tag => !string.IsNullOrWhiteSpace(tag.name))
                    .Select((tag, index) => (tag.index, index))
                    .ToDictionary(tag => tag.Item1, tag => tag.Item2);
                
                viewToModel = modelToView.ToDictionary(pair => pair.Value, pair => pair.Key);
                
                maskField.formatSelectedValueCallback = value => $"{asset.name}.{value}";
                maskField.choices = asset.GetNames().ToList();
                HandleFlagsChanged(flagsProperty);
            }
            
            void FillChoices(SerializedProperty _)
            {
                if (sourceProperty.boxedValue is not TagSource source || !source.IsValid) {
                    maskField.formatSelectedValueCallback = null;
                    maskField.choices = new List<string>();
                    return;
                }

                var prefix = source.GetShortName();
                maskField.formatSelectedValueCallback = value => $"{prefix}.{value}";
                maskField.choices = source.GetNames().ToList();
                HandleFlagsChanged(flagsProperty);
            }
            
            void HandleFlagsChanged(SerializedProperty serializedProperty)
            {
                if (modelToView == null) return;

                var fromValue = serializedProperty.intValue;
                var resultValue = 0;
                foreach (var (view, model) in modelToView) {
                    if ((fromValue & (1 << view)) != 0)
                        resultValue |= 1 << model;
                }
                maskField.value = resultValue;
            }
            
            void HandleMaskChanged(ChangeEvent<int> evt)
            {
                if (viewToModel == null) return;
                
                var resultValue = 0;
                foreach (var (view, model) in viewToModel) {
                    if ((evt.newValue & (1 << view)) != 0)
                        resultValue |= 1 << model;
                }
                flagsProperty.intValue = resultValue;
                flagsProperty.serializedObject.ApplyModifiedProperties();
            }
        }
        
        private void DisposeElement(VisualElement? visualElement)
        {
            if (visualElement == null) return;
            visualElement.Unbind();
            visualElement.RemoveFromHierarchy();
        }
    }
}