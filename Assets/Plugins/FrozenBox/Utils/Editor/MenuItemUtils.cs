using System.Diagnostics.CodeAnalysis;
using UnityEditor;

namespace FrozenBox.Utils.Editor
{
    public static class MenuItemUtils
    {
        public static bool TryGetContextPath([NotNullWhen(true)] out string? path)
        {
            foreach(var guid in Selection.assetGUIDs)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                path = EditorUtils.GetFolderByPath(assetPath);
                return true;
            }

            path = null;
            return false;
        }
    }
}