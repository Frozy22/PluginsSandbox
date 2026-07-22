using FrozenBox.Utils;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

namespace FrozenBox.Utils.Editor
{
    internal class CreatePackageAssemblyPopup : EditorPopupWindow<CreatePackageAssemblyPopup>
    {
        [MenuItem("Assets/Create/FrozenBox/Scripting/Create Assembly Package")]
        private static void OpenPopup()
        {
            OpenPopup(new Vector2(396, 96));
        }

        protected override void CreateGuiInner(TemplateContainer container)
        {
            var nameField = container.Q<TextField>("NameField");
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
            Assert.IsFalse(ContextFolder.EndsWith("/Runtime"));
            Assert.IsFalse(ContextFolder.EndsWith("/Editor"));
                
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
            
            var assemblyName = nameField.value;
            var editorAssemblyName = $"{assemblyName}.Editor";
            
            var assemblyFolder = $"{ContextFolder}/Runtime";
            var editorAssemblyFolder = $"{ContextFolder}/Editor";
            
            if (!AssetDatabase.IsValidFolder(assemblyFolder))
                AssetDatabase.CreateFolder(ContextFolder, "Runtime");
            
            if (!AssetDatabase.IsValidFolder(editorAssemblyFolder))
                AssetDatabase.CreateFolder(ContextFolder, "Editor");

            AssemblyUtils.CreateAssembly(assemblyFolder, assemblyName, false);
            AssemblyUtils.CreateAssembly(editorAssemblyFolder, editorAssemblyName, true, assemblyName);
            Close();
        }

        private void HandleCancelButtonClicked()
        {
            Close();
        }
    }
}