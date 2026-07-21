using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine.UIElements;

namespace FrozenBox.Inspector.Editor.ComponentsManager
{
    internal sealed class ComponentsManagerElement : VisualElement
    {
        private const string BUTTON_TOGGLED_CLASS = "fr--go-inspector-context__toggled_button";

        private readonly Dictionary<Type, Button> _componentButtons = new();
        private readonly Dictionary<Type, List<VisualElement>> _inspectorComponents = new();

        public IEnumerable<Type> HiddenComponents => _componentButtons
            .Where(pair => pair.Value.ClassListContains(BUTTON_TOGGLED_CLASS))
            .Select(pair => pair.Key);
        
        public ComponentsManagerElement(VisualElement inspectorRoot, HashSet<Type> commonTypes)
        {
            var inspectorContainer = inspectorRoot.Q(null, "unity-inspector-editors-list");
            AddToClassList("fr--go-inspector-context__component_selection");

            foreach (var componentType in commonTypes)
            {
                var image = Background.FromTexture2D(AssetPreview.GetMiniTypeThumbnail(componentType));

                var button = new Button
                {
                    text = componentType.Name,
                    iconImage = image
                };
                button.AddToClassList("fr--go-inspector-context__button");
                button.clicked += () => HandleButtonClicked(componentType);
                button.RegisterCallback<ClickEvent>(evt => HandleButtonClicked(evt, componentType));

                var isHidden = button.ClassListContains(BUTTON_TOGGLED_CLASS);
                var elements = inspectorContainer.Query<VisualElement>()
                    .Where(element => element.name.Contains($"_{componentType.Name}_")).Build().ToList();
                
                foreach (var element in elements)
                {
                    if (isHidden)
                        element.AddToClassList(StyleConstants.CLASS_HIDDEN_ELEMENT);
                    else
                        element.RemoveFromClassList(StyleConstants.CLASS_HIDDEN_ELEMENT);
                }

                _inspectorComponents.Add(componentType, elements);
                _componentButtons.Add(componentType, button);
                Add(button);
            }
        }

        public void SetHiddenComponents(IEnumerable<Type> hiddenTypes)
        {
            var hiddenTypesSet = new HashSet<Type>(hiddenTypes);
            
            foreach (var (type, _) in _componentButtons) 
                ChangeButtonState(type, hiddenTypesSet.Contains(type));
        }

        private void HandleButtonClicked(ClickEvent evt, Type componentType)
        {
            if (!evt.shiftKey)
                return;

            var componentButton = _componentButtons[componentType];
            var isComponentHidden = componentButton.ClassListContains(BUTTON_TOGGLED_CLASS);
            ChangeButtonState(componentType, !isComponentHidden);
        }

        private void HandleButtonClicked(Type componentType)
        {
            foreach (var (type, _) in _componentButtons)
            {
                if (type == componentType)
                {
                    ChangeButtonState(componentType, false);
                    continue;
                }

                ChangeButtonState(type, true);
            }
        }

        private void ChangeButtonState(Type componentType, bool isHidden)
        {
            var button = _componentButtons[componentType];
            var isComponentHidden = button.ClassListContains(BUTTON_TOGGLED_CLASS);

            if (isComponentHidden == isHidden)
                return;

            if (isHidden)
            {
                button.AddToClassList(BUTTON_TOGGLED_CLASS);
                _inspectorComponents[componentType].ForEach(element => element.AddToClassList(StyleConstants.CLASS_HIDDEN_ELEMENT));
            }
            else
            {
                button.RemoveFromClassList(BUTTON_TOGGLED_CLASS);
                _inspectorComponents[componentType]
                    .ForEach(element => element.RemoveFromClassList(StyleConstants.CLASS_HIDDEN_ELEMENT));
            }
        }
    }
}