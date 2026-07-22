#if HAS_NETCODE_GO
using System;
using Unity.Netcode;

namespace FrozenBox.TagsSystem.Networking
{
    public struct NetFlagsHandle : INetworkSerializable, IEquatable<NetFlagsHandle>
    {
        private TagsGroupsLibrary.Reference _source;
        private int _flags;
        
        internal NetFlagsHandle(TagsSource source, int flags) {
            _source = source;
            _flags = flags;
        }
        
        public static implicit operator NetFlagsHandle(FlagsHandle flags) => new(flags.Source, flags.Flags);
        public static implicit operator FlagsHandle(NetFlagsHandle flagsHandle) => new(flagsHandle._source.Get(), flagsHandle._flags);
        
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref _source);
            serializer.SerializeValue(ref _flags);
        }

        public bool Equals(NetFlagsHandle other) => _source.Equals(other._source) && _flags == other._flags;
        public override bool Equals(object? obj) => obj is NetTagHandle other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(_source, _flags);

        public static bool operator ==(NetFlagsHandle left, NetFlagsHandle right) => left.Equals(right);
        public static bool operator !=(NetFlagsHandle left, NetFlagsHandle right) => !left.Equals(right);
    }
}
#endif