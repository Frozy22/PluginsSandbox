using System;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace FrozenBox.TagsSystem.Editor
{
    [CustomEditor(typeof(TagSourceEnum))]
    public class TagSourceEnumEditor : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            var enumTypeNameProperty = serializedObject.FindProperty("_enumTypeName");
            
            var dropdownField = new DropdownField(enumTypeNameProperty.displayName);
            dropdownField.BindProperty(enumTypeNameProperty);
            dropdownField.RegisterValueChangedCallback(HandleTypeNameChanged);

            var searchBarField = new ToolbarSearchField();
            searchBarField.RegisterValueChangedCallback(HandleSearchBarChanged);
            HandleSearchBarChanged(null!);

            var tagsProperty = serializedObject.FindProperty("_tags");
            var tagsField = new PropertyField(tagsProperty);
            tagsField.BindProperty(tagsProperty);
            tagsField.enabledSelf = false;
            
            root.Add(searchBarField);
            root.Add(dropdownField);
            root.Add(tagsField);
            root.Bind(serializedObject);
            return root;

            void HandleSearchBarChanged(ChangeEvent<string> _)
            {
                var searchText = searchBarField.value;
                var value = dropdownField.value;
                if (string.IsNullOrWhiteSpace(searchText)) {
                    dropdownField.choices = TypeCache.GetTypesDerivedFrom<Enum>()
                        .Select(type => type.AssemblyQualifiedName).ToList();
                }else {
                    dropdownField.choices = TypeCache.GetTypesDerivedFrom<Enum>()
                        .Select(type => type.AssemblyQualifiedName)
                        .Where(typeName => typeName.ContainsInvariantCultureIgnoreCase(searchText)).ToList();
                }
                dropdownField.value = value;
            }
            
            void HandleTypeNameChanged(ChangeEvent<string> evt)
            {
                var tagsSource = serializedObject.targetObject as TagSourceEnum;
                if (tagsSource == null) return;
                tagsSource.TryInitialize();
            }
        }

    }
}