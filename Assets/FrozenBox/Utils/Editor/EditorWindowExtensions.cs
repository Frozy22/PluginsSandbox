using FrozenBox.Utils;
using UnityEditor;

namespace FrozenBox.Utils.Editor
{
    public static class EditorWindowExtensions
    {
        public static void CenterOnMainWindow(this EditorWindow editorWindow)
        {
            var mainWindow = EditorGUIUtility.GetMainWindowPosition();
            var center = mainWindow.position + mainWindow.size / 2f;
    
            editorWindow.position = editorWindow.position.WithCenter(center);
        }
    }
}