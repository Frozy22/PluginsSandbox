using System;
using UnityEditor;

namespace FrozenBox.Utils.Editor
{
    public static class EditorUtils
    {
        public static UnityEditor.Editor CreateEditor(SerializedObject serializedObject, Type editorType)
        {
            return serializedObject.isEditingMultipleObjects
                ? UnityEditor.Editor.CreateEditor(serializedObject.targetObjects, editorType)
                : UnityEditor.Editor.CreateEditor(serializedObject.targetObject, editorType);
        }

        public static string GetFolderByPath(string assetPath)
        {
            return AssetDatabase.IsValidFolder(assetPath) 
                ? assetPath 
                : assetPath[..assetPath.LastIndexOf('/')];
        }
    }
}