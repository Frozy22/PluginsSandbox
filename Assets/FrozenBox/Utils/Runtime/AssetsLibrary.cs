using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

#if HAS_NETCODE_GO
using Unity.Netcode;
#endif

namespace FrozenBox.Utils
{
    public partial class AssetsLibrary<TAsset> where TAsset : Object
    {
        private static readonly Dictionary<int, TAsset> HashedAssets = new();
        public static IReadOnlyDictionary<int, TAsset> Assets => HashedAssets;

        private static int AssetToHash(TAsset asset) => Animator.StringToHash(asset.name);
        private static int AssetToHash(string assetName) => Animator.StringToHash(assetName);
        
        static AssetsLibrary()
        {
            HashedAssets.Clear();
            var assets = Resources.FindObjectsOfTypeAll<TAsset>();
            
            foreach (var asset in assets) {
                var hash = AssetToHash(asset);
                Assert.IsFalse(HashedAssets.ContainsKey(hash), $"Hash conflict for {asset.name} and {HashedAssets.GetValueOrDefault(hash)?.name ?? "null"}. Required to rename one of this.");
                HashedAssets.Add(hash, asset);
            }
        }
    }
}