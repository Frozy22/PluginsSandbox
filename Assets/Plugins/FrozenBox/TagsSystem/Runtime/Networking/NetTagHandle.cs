#if HAS_NETCODE_GO
using System;
using Unity.Netcode;

namespace FrozenBox.TagsSystem.Networking
{
    public struct NetTagHandle : INetworkSerializable, IEquatable<NetTagHandle>
    {
        private TagsSource.Reference _source;
        private int _index;
        
        internal NetTagHandle(TagsSource source, int index) {
            _source = source;
            _index = index;
        }
        
        public static implicit operator NetTagHandle(TagHandle tag) => new(tag.Source, tag.Index);
        public static implicit operator TagHandle(NetTagHandle tagHandle) => new(tagHandle._source.Get(), tagHandle._index);
        
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref _source);
            serializer.SerializeValue(ref _index);
        }

        public bool Equals(NetTagHandle other) => _source.Equals(other._source) && _index == other._index;
        public override bool Equals(object? obj) => obj is NetTagHandle other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(_source, _index);

        public static bool operator ==(NetTagHandle left, NetTagHandle right) => left.Equals(right);
        public static bool operator !=(NetTagHandle left, NetTagHandle right) => !left.Equals(right);
    }
}
#endif