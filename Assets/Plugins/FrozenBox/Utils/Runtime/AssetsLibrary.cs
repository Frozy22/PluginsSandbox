using System.Collections.Frozen;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

#if UNITY_EDITOR
using UnityEditor;
#endif

#if HAS_NETCODE_GO
using Unity.Netcode;
#endif

namespace FrozenBox.Utils
{
    public partial class AssetsLibrary<TAsset> where TAsset : Object
    {
        private static readonly FrozenDictionary<int, TAsset> HashedAssets;
        public static IReadOnlyDictionary<int, TAsset> Assets => HashedAssets;

        internal static int AssetToHash(TAsset asset) => Animator.StringToHash(asset.name);
        private static int AssetToHash(string assetName) => Animator.StringToHash(assetName);

        static AssetsLibrary()
        {
            var assets = Resources.FindObjectsOfTypeAll<TAsset>();
            FrAssert.IsNotNull(assets);
            Assert.IsTrue(assets.Length > 0);
            HashedAssets = assets.ToFrozenDictionary(AssetToHash, asset => asset);
        }
    }
}