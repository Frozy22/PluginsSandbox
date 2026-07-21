using System;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Assertions;

namespace FrozenBox.Utils.Editor
{
    internal abstract class EditorSingletonData<T> : ScriptableObject where T : EditorSingletonData<T>
    {   
        private static T? _instance;
        public static T Instance
        {
            get
            {
                _instance ??= FindInstance();
                return _instance;
            }
        }
        
        private static T FindInstance()
        {
            var guid = AssetDatabase.FindAssets($"t:{typeof(T).Name}").FirstOrDefault();
            
            if (!string.IsNullOrEmpty(guid))
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var instance = AssetDatabase.LoadAssetAtPath<T>(path);
                var defaultPath = instance.GetDefaultAssetPath();

                if (path != defaultPath)
                    AssetDatabase.MoveAsset(path, defaultPath);
                
                return instance;
            }
            else
            {
                var instance = CreateInstance<T>();
                var defaultPath = instance.GetDefaultAssetPath();
                AssetDatabase.CreateAsset(instance, defaultPath);

                if (EditorUtility.DisplayDialog("Created new Singleton Instance.",
                        $"Failed to found Editor Singleton of {typeof(T).Name} - Created new instance.",
                        "Select new Asset", "OK"))
                {
                    Selection.activeObject = instance;
                }
                
                return instance;
            }
        }
        
        protected static string GetAssemblyAssetPath()
        {
            var location = typeof(T).Assembly.Location;
            var name = location[(location.LastIndexOf('\\') + 1)..location.LastIndexOf(".dll", StringComparison.Ordinal)];
            
            var assetGuid = AssetDatabase.FindAssets($"t:{nameof(AssemblyDefinitionAsset)} {name}").FirstOrDefault();
            Assert.IsNotNull(assetGuid, "Failed to find assembly asset.");
            
            return AssetDatabase.GUIDToAssetPath(assetGuid);
        }

        protected virtual string GetDefaultAssetPath() 
            => $"{EditorUtils.GetFolderByPath(GetAssemblyAssetPath())}/Resources/{typeof(T).Name}.asset";
    }
}