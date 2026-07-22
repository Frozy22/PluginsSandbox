using FrozenBox.Utils;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace FrozenBox.Utils.Editor
{
    internal class CreateAssemblyPopup : EditorPopupWindow<CreateAssemblyPopup>
    {
        [MenuItem("Assets/Create/FrozenBox/Scripting/Create Assembly")]
        private static void OpenPopup()
        {
            OpenPopup(new Vector2(396, 96));
        }
        
        protected override void CreateGuiInner(TemplateContainer container)
        {
            var nameField = container.Q<TextField>("NameField");
            var isEditorToggle = rootVisualElement.Q<Toggle>("IsEditorToggle");
            var createButton = container.Q<Button>("CreateButton");
            var cancelButton = container.Q<Button>("CancelButton");
            
            nameField.RegisterCallback<KeyUpEvent>(evt =>
            {
                switch (evt.keyCode)
                {
                    case KeyCode.Return:
                        createButton.Focus();
                        evt.StopImmediatePropagation();
                        break;
                    
                    case KeyCode.Escape:
                        HandleCancelButtonClicked();
                        evt.StopImmediatePropagation();
                        break;
                }
            });

            FrAssert.IsNotNull(ContextFolder, "ContextFolder is null.");
            
            switch (ContextFolder[ContextFolder.LastIndexOf('/')..])
            {
                case "/Editor":
                    isEditorToggle.value = true;
                    break;
                case "/Runtime":
                    ContextFolder = ContextFolder[..^"/Runtime".Length];
                    goto default;
                default:
                    isEditorToggle.value = false;
                    break;
            }
            
            nameField.value = ContextFolder[(ContextFolder.IndexOf('/') + 1)..].Replace('/', '.');

            createButton.clicked += HandleCreateButtonClicked;
            createButton.RegisterCallback<KeyDownEvent>(evt =>
            {
                if (evt.keyCode == KeyCode.Escape)
                {
                    HandleCancelButtonClicked();
                    evt.StopImmediatePropagation();
                }
            });
            
            cancelButton.clicked += HandleCancelButtonClicked;
            cancelButton.RegisterCallback<KeyDownEvent>(evt =>
            {
                if (evt.keyCode == KeyCode.Escape)
                {
                    HandleCancelButtonClicked();
                    evt.StopImmediatePropagation();
                }
            });
            
            nameField.Focus();
        }

        private void HandleCreateButtonClicked()
        {
            var nameField = rootVisualElement.Q<TextField>("NameField");
            var isEditorToggle = rootVisualElement.Q<Toggle>("IsEditorToggle");
            var assemblyName = nameField.value;

            FrAssert.IsNotNull(ContextFolder, "ContextFolder is null.");
            AssemblyUtils.CreateAssembly(ContextFolder, assemblyName, isEditorToggle.value);
            Close();
        }

        private void HandleCancelButtonClicked()
        {
            Close();
        }
    }
}