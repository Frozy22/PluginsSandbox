using System;
using UnityEngine.UIElements;

namespace FrozenBox.Inspector.Editor
{
    internal abstract class GameObjectInspectorContext : UnityEditor.Editor
    {
        public event Action? OnVisibilityChanged;

        public VisualElement InspectorRoot { get; private set; } = null!;
        public VisualElement VisualElement { get; private set; } = null!;

        public bool IsVisible
        {
            get => !VisualElement.ClassListContains(StyleConstants.CLASS_HIDDEN_ELEMENT);
            private set
            {
                if (value == IsVisible)
                    return;
                
                if (value)
                    VisualElement.RemoveFromClassList(StyleConstants.CLASS_HIDDEN_ELEMENT);
                else 
                    VisualElement.AddToClassList(StyleConstants.CLASS_HIDDEN_ELEMENT);

                OnVisibilityChanged?.Invoke();
            }
        }

        public virtual bool CanShow() => true;

        public void Setup(VisualElement inspectorRoot)
        {
            InspectorRoot = inspectorRoot;
            VisualElement = CreateInspectorGUI();
            VisualElement.AddToClassList("fr--go-inspector-context__container");
            IsVisible = CanShow();
        }
    }
}