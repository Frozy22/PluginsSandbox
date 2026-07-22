using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace FrozenBox.TagsSystem
{
    [Serializable]
    public struct FlagsHandle : IEquatable<FlagsHandle>
    {
        [SerializeField] internal TagSource _source;
        [SerializeField] internal int _flags;

        public static FlagsHandle EmptyOf(TagSource source) => new(source, 0);

        internal FlagsHandle(TagSource source, int flags) {
            _source = source;
            _flags = flags;
        }

        public bool HasTag(TagHandle tag) {
            Assert.AreEqual(_source, tag._source);
            return (_flags & tag.Flag) != 0;
        }

        public bool HasAny(FlagsHandle flags) {
            Assert.AreEqual(_source, flags._source);
            return (_flags & flags._flags) != 0;
        }
        
        public bool HasAny() => _flags != 0;

        public bool HasAll(FlagsHandle flags) {
            Assert.AreEqual(_source, flags._source);
            return (_flags & flags._flags) == flags._flags;
        }

        private static FlagsHandle TryCombine(FlagsHandle a, FlagsHandle b, int flags) {
            Assert.AreEqual(a._source, b._source);
            return new FlagsHandle {_source = a._source, _flags = flags};
        }
        
        private static FlagsHandle TryCombine(FlagsHandle a, TagHandle b, int flags) {
            Assert.AreEqual(a._source, b._source);
            return new FlagsHandle(a._source, flags);
        }
        
        private static FlagsHandle TryCombine(TagHandle a, TagHandle b, int flags) {
            Assert.AreEqual(a._source, b._source);
            return new FlagsHandle {_source = a._source, _flags = flags};
        }

        public bool Equals(FlagsHandle other) => _source.Equals(other._source) && _flags == other._flags;
        public override bool Equals(object? obj) => obj is FlagsHandle other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(_source, _flags);

        public static bool operator ==(FlagsHandle left, FlagsHandle right) => left.Equals(right);
        public static bool operator !=(FlagsHandle left, FlagsHandle right) => !left.Equals(right);

        public static FlagsHandle operator ~(FlagsHandle a) => new() { _source = a._source, _flags = ~a._flags };
        
        public static FlagsHandle operator &(FlagsHandle a, FlagsHandle b) => TryCombine(a, b, a._flags & b._flags);
        public static FlagsHandle operator |(FlagsHandle a, FlagsHandle b) => TryCombine(a, b, a._flags | b._flags);
        public static FlagsHandle operator ^(FlagsHandle a, FlagsHandle b) => TryCombine(a, b, a._flags ^ b._flags);
        
        public static FlagsHandle operator &(FlagsHandle a, TagHandle b) => TryCombine(a, b, a._flags & b.Flag);
        public static FlagsHandle operator |(FlagsHandle a, TagHandle b) => TryCombine(a, b, a._flags | b.Flag);
        public static FlagsHandle operator ^(FlagsHandle a, TagHandle b) => TryCombine(a, b, a._flags ^ b.Flag);
    }
}