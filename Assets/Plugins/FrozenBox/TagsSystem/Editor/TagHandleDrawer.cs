using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace FrozenBox.TagsSystem.Editor
{
    [CustomPropertyDrawer(typeof(TagHandle))]
    internal class TagHandleDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var root = new Foldout { text = "" };
            root.Q<VisualElement>(null, "unity-toggle").style.marginBottom = 0;

            var sourceProperty = property.FindPropertyRelative("_source");
            var flagProperty = property.FindPropertyRelative("_flag");

            var sourceField = new PropertyField(sourceProperty);
            sourceField.BindProperty(sourceProperty);

            var dropDownField = new DropdownField(property.displayName) 
            { style = { flexGrow = 1, marginLeft = 0, marginRight = 0 } };
            dropDownField.labelElement.style.marginLeft = 0;
            dropDownField.AddToClassList(BaseField<object>.alignedFieldUssClassName);
            
            dropDownField.TrackPropertyValue(sourceProperty, FillChoices);
            dropDownField.TrackPropertyValue(flagProperty, HandleFlagChanged);
            dropDownField.RegisterValueChangedCallback(HandleFlagValueChanged);
            FillChoices(null!);
            
            var foldoutToggle = root.Q<Toggle>(className: "unity-foldout__toggle");
            
            foldoutToggle.Q<VisualElement>(className: BaseField<object>.inputUssClassName)
                .RemoveFromClassList(BaseField<object>.inputUssClassName);
            
            foldoutToggle.Add(dropDownField);
            root.contentContainer.Add(sourceField);
            return root;

            void FillChoices(SerializedProperty _)
            {
                if (sourceProperty.boxedValue is not TagSource source || !source.IsValid) {
                    dropDownField.formatSelectedValueCallback = null;
                    dropDownField.choices = new List<string>();
                    return;
                }

                var prefix = source.GetShortName();
                dropDownField.formatSelectedValueCallback = value => $"{prefix}.{value}";
                dropDownField.choices = source.GetNames().ToList();
                HandleFlagChanged(flagProperty);
            }
            
            void HandleFlagChanged(SerializedProperty serializedProperty)
            {
                if (sourceProperty.boxedValue is not TagSource source || !source.IsValid) {
                    dropDownField.value = "";
                    return;
                }
                dropDownField.value = source.NameOfIndex(serializedProperty.intValue);
            }
            
            void HandleFlagValueChanged(ChangeEvent<string> evt)
            {
                if (sourceProperty.boxedValue is not TagSource source || !source.IsValid) return;
                if (!source.TryGetByName(evt.newValue, out var handle)) return;
                flagProperty.intValue = handle.Flag;
                flagProperty.serializedObject.ApplyModifiedProperties();
            }
        }
    }
}