#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FrozenBox.Utils.Editor
{
    public static class AssetDatabaseUtils
    {
        public struct AssetContext<T> where T : Object
        {
            public GUID Guid;
            public string Path;
            public T Asset;
        }

        public static int AssetsCount<T>(string filter = "", string[]? findInFolders = null) where T : Object
        {
            var assetsGuids = findInFolders != null 
                ? AssetDatabase.FindAssetGUIDs($"{filter} t:{typeof(T).Name}") 
                : AssetDatabase.FindAssetGUIDs($"{filter} t:{typeof(T).Name}", findInFolders);
            
            return assetsGuids.Length;
        }
        
        public static IEnumerable<T> FindAssets<T>(string filter = "", string[]? findInFolders = null) where T : Object
        {
            var assetsGuids = findInFolders != null 
                ? AssetDatabase.FindAssetGUIDs($"{filter} t:{typeof(T).Name}") 
                : AssetDatabase.FindAssetGUIDs($"{filter} t:{typeof(T).Name}", findInFolders);
            
            foreach (var assetGuid in assetsGuids)
                yield return AssetDatabase.LoadAssetByGUID<T>(assetGuid);
        }
        
        public static IEnumerable<AssetContext<T>> FindAssetsWithContext<T>(string filter = "", string[]? findInFolders = null) where T : Object
        {
            var assetsGuids = findInFolders != null 
                ? AssetDatabase.FindAssetGUIDs($"{filter} t:{typeof(T).Name}") 
                : AssetDatabase.FindAssetGUIDs($"{filter} t:{typeof(T).Name}", findInFolders);
            
            foreach (var assetGuid in assetsGuids)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(assetGuid);
                var asset = AssetDatabase.LoadAssetByGUID<T>(assetGuid);
                
                yield return new AssetContext<T>()
                {
                    Guid = assetGuid,
                    Path = assetPath,
                    Asset = asset
                };
            }
        }
    }
}

#endif