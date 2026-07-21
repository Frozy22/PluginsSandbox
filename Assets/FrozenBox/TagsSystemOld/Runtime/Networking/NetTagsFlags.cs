#if HAS_NETCODE_GO
using System;
using Unity.Netcode;

namespace FrozenBox.TagsSystem.Networking
{
    public struct NetTagsFlags : INetworkSerializable, IEquatable<NetTagsFlags>
    {
        private TagsGroupsLibrary.Reference _group;
        private int _flags;
        
        internal NetTagsFlags(TagsGroup group, int flags) {
            _group = group;
            _flags = flags;
        }
        
        public static implicit operator NetTagsFlags(TagsFlags flags) => new(flags._group, flags._flags);
        public static implicit operator TagsFlags(NetTagsFlags flags) => new(flags._group.Get(), flags._flags);
        
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref _group);
            serializer.SerializeValue(ref _flags);
        }

        public bool Equals(NetTagsFlags other) => _group.Equals(other._group) && _flags == other._flags;
        public override bool Equals(object? obj) => obj is NetTag other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(_group, _flags);

        public static bool operator ==(NetTagsFlags left, NetTagsFlags right) => left.Equals(right);
        public static bool operator !=(NetTagsFlags left, NetTagsFlags right) => !left.Equals(right);
    }
}
#endif