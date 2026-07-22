using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace FrozenBox.TagsSystem
{
    [Serializable]
    public struct FlagsHandle : IEquatable<FlagsHandle>
    {
        [SerializeField] private TagsSource _source;
        [SerializeField] private int _flags;
        
        internal TagsSource Source => _source;
        internal int Flags => _flags;
        
        public static FlagsHandle EmptyOf(TagsSource source) => new(source, 0);

        internal FlagsHandle(TagsSource source, int flags) {
            _source = source;
            _flags = flags;
        }

        public bool HasTag(TagHandle tagHandle) {
            Assert.AreEqual(_source, tagHandle.Source);
            return (_flags & tagHandle.Flag) != 0;
        }

        public bool HasAny(FlagsHandle flagsHandle) {
            Assert.AreEqual(_source, flagsHandle._source);
            return (_flags & flagsHandle._flags) != 0;
        }
        
        public bool HasAny() => _flags != 0;

        public bool HasAll(FlagsHandle flagsHandle) {
            Assert.AreEqual(_source, flagsHandle._source);
            return (_flags & flagsHandle._flags) == flagsHandle._flags;
        }

        private static FlagsHandle TryCombine(FlagsHandle a, FlagsHandle b, int flags) {
            Assert.AreEqual(a._source, b._source);
            return new FlagsHandle {_source = a._source, _flags = flags};
        }
        
        private static FlagsHandle TryCombine(FlagsHandle a, TagHandle b, int flags) {
            Assert.AreEqual(a._source, b.Source);
            return new FlagsHandle(a._source, flags);
        }
        
        private static FlagsHandle TryCombine(TagHandle a, TagHandle b, int flags) {
            Assert.AreEqual(a.Source, b.Source);
            return new FlagsHandle {_source = a.Source, _flags = flags};
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