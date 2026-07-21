using UnityEngine.Assertions;

#if HAS_NETCODE_GO
using Unity.Netcode;
#endif

namespace FrozenBox.Utils
{
    public partial class AssetsLibrary<TAsset>
    {
#if HAS_NETCODE_GO
        public struct Reference : INetworkSerializeByMemcpy, System.IEquatable<Reference>
        {
            public static Reference Empty = new();
            public readonly bool IsEmpty => this == Empty;
            public readonly bool IsValid => !IsEmpty && HashedAssets.ContainsKey(Hash);

            public int Hash;
            
            public readonly TAsset Get()
            {
                Assert.IsTrue(IsValid, $"Reference[{Hash}] of type {typeof(TAsset).Name} is not valid.");
                return HashedAssets[Hash];
            }

            public readonly bool TryGet(out TAsset asset) => HashedAssets.TryGetValue(Hash, out asset);
            public void Set(TAsset asset) => Hash = AssetToHash(asset);

            public Reference(TAsset asset) {
                Hash = AssetToHash(asset);
            }

            public Reference(int hash) {
                Hash = hash;
            }
            
            public bool Equals(Reference other) => Hash == other.Hash;
            public override bool Equals(object? obj) => obj is Reference other && Equals(other);
            public override int GetHashCode() => Hash;

            public static bool operator ==(Reference left, Reference right) => left.Equals(right);
            public static bool operator !=(Reference left, Reference right) => !left.Equals(right);

            public static implicit operator Reference(TAsset asset) => new(asset);
        }
#endif
    }
}