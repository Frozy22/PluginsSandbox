using UnityEngine.Assertions;

#if HAS_NETCODE_GO
using Unity.Netcode;
#endif

namespace FrozenBox.TagsSystem
{
    public partial class TagsSource
    {
#if HAS_NETCODE_GO
        public struct Reference : INetworkSerializeByMemcpy, System.IEquatable<Reference>
        {
            public static Reference Empty = new();
            public readonly bool IsEmpty => this == Empty;
            public readonly bool IsValid => !IsEmpty && TagsConfig.Assets.ContainsKey(Hash);

            public int Hash;
            
            public readonly TagsSource Get()
            {
                Assert.IsTrue(IsValid, $"Reference[{Hash}] of type {nameof(TagsSource)} is not valid.");
                return TagsConfig.Assets[Hash];
            }

            public readonly bool TryGet(out TagsSource asset) => TagsConfig.Assets.TryGetValue(Hash, out asset);
            public void Set(TagsSource asset) => Hash = TagsConfig.AssetToHash(asset);

            public Reference(TagsSource asset) {
                Hash = TagsConfig.AssetToHash(asset);
            }

            public Reference(int hash) {
                Hash = hash;
            }
            
            public bool Equals(Reference other) => Hash == other.Hash;
            public override bool Equals(object? obj) => obj is Reference other && Equals(other);
            public override int GetHashCode() => Hash;

            public static bool operator ==(Reference left, Reference right) => left.Equals(right);
            public static bool operator !=(Reference left, Reference right) => !left.Equals(right);

            public static implicit operator Reference(TagsSource asset) => new(asset);
        }
#endif
    }
}