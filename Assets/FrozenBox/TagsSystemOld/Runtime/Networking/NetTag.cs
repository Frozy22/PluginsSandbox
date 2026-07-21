#if HAS_NETCODE_GO
using System;
using Unity.Netcode;

namespace FrozenBox.TagsSystem.Networking
{
    public struct NetTag : INetworkSerializable, IEquatable<NetTag>
    {
        private TagsGroupsLibrary.Reference _group;
        private int _index;
        
        internal NetTag(TagsGroup group, int index) {
            _group = group;
            _index = index;
        }
        
        public static implicit operator NetTag(Tag tag) => new(tag.Group, tag._index);
        public static implicit operator Tag(NetTag tag) => new(tag._group.Get(), tag._index);
        
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref _group);
            serializer.SerializeValue(ref _index);
        }

        public bool Equals(NetTag other) => _group.Equals(other._group) && _index == other._index;
        public override bool Equals(object? obj) => obj is NetTag other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(_group, _index);

        public static bool operator ==(NetTag left, NetTag right) => left.Equals(right);
        public static bool operator !=(NetTag left, NetTag right) => !left.Equals(right);
    }
}
#endif