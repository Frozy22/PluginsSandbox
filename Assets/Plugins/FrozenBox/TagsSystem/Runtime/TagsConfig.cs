using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using FrozenBox.Utils;
using UnityEngine;
using UnityEngine.Assertions;

namespace FrozenBox.TagsSystem
{
    public class TagsConfig : ScriptableObject
    {
        [SerializeField] private TagsSource[] _sources = Array.Empty<TagsSource>();
        
        private static FrozenDictionary<int, TagsSource>? _hashedAssets;
        public static IReadOnlyDictionary<int, TagsSource> Assets => _hashedAssets!;

        internal static int AssetToHash(TagsSource asset) => Animator.StringToHash(asset.name);
        private static int AssetToHash(string assetName) => Animator.StringToHash(assetName);
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            var assets = Resources.FindObjectsOfTypeAll<TagsConfig>();
            FrAssert.IsNotNull(assets);
            Assert.IsTrue(assets.Length == 1);
            var asset = assets[0];
            _hashedAssets = asset._sources.ToFrozenDictionary(AssetToHash, source => source);
        }
    }
}