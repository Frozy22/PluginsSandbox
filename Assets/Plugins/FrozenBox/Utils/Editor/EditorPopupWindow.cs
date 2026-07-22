using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace FrozenBox.Utils.Editor
{
    public abstract class EditorPopupWindow<T> : EditorWindow where T : EditorPopupWindow<T>
    {
        [SerializeField]
        private VisualTreeAsset _popupAsset = null!;

        protected string? ContextFolder;

        protected static T OpenPopup(Vector2 size)
        {
            var window = CreateInstance<T>();
            
            if (!MenuItemUtils.TryGetContextPath(out window.ContextFolder))
                throw new Exception("No context path found.");
            
            window.position = new Rect(0, 0, size.x, size.y);
            window.CenterOnMainWindow();
            window.ShowPopup();
            
            return window;
        }

        private void CreateGUI()
        {
            var container = _popupAsset.CloneTree();
            container.style.flexGrow = 1;
            
            CreateGuiInner(container);
            
            rootVisualElement.Add(container);
        }

        protected abstract void CreateGuiInner(TemplateContainer container);
    }
}