using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace FrozenBox.TagsSystem
{
    [Serializable]
    public struct TagsFlags : IEquatable<TagsFlags>
    {
        [SerializeField] internal TagSource _source;
        [SerializeField] internal int _flags;

        public static TagsFlags EmptyOf(TagSource source) => new(source, 0);

        internal TagsFlags(TagSource source, int flags) {
            _source = source;
            _flags = flags;
        }

        public bool HasTag(TagHandle tag) {
            Assert.AreEqual(_source, tag.Source);
            return (_flags & tag.Flag) != 0;
        }

        public bool HasAny(TagsFlags flags) {
            Assert.AreEqual(_source, flags._source);
            return (_flags & flags._flags) != 0;
        }
        
        public bool HasAny() => _flags != 0;

        public bool HasAll(TagsFlags flags) {
            Assert.AreEqual(_source, flags._source);
            return (_flags & flags._flags) == flags._flags;
        }

        private static TagsFlags TryCombine(TagsFlags a, TagsFlags b, int flags) {
            Assert.AreEqual(a._source, b._source);
            return new TagsFlags {_source = a._source, _flags = flags};
        }
        
        private static TagsFlags TryCombine(TagsFlags a, TagHandle b, int flags) {
            Assert.AreEqual(a._source, b.Source);
            return new TagsFlags(a._source, flags);
        }
        
        private static TagsFlags TryCombine(TagHandle a, TagHandle b, int flags) {
            Assert.AreEqual(a.Source, b.Source);
            return new TagsFlags {_source = a.Source, _flags = flags};
        }

        public bool Equals(TagsFlags other) => _source.Equals(other._source) && _flags == other._flags;
        public override bool Equals(object? obj) => obj is TagsFlags other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(_source, _flags);

        public static bool operator ==(TagsFlags left, TagsFlags right) => left.Equals(right);
        public static bool operator !=(TagsFlags left, TagsFlags right) => !left.Equals(right);

        public static TagsFlags operator ~(TagsFlags a) => new() { _source = a._source, _flags = ~a._flags };
        
        public static TagsFlags operator &(TagsFlags a, TagsFlags b) => TryCombine(a, b, a._flags & b._flags);
        public static TagsFlags operator |(TagsFlags a, TagsFlags b) => TryCombine(a, b, a._flags | b._flags);
        public static TagsFlags operator ^(TagsFlags a, TagsFlags b) => TryCombine(a, b, a._flags ^ b._flags);
        
        public static TagsFlags operator &(TagsFlags a, TagHandle b) => TryCombine(a, b, a._flags & b.Flag);
        public static TagsFlags operator |(TagsFlags a, TagHandle b) => TryCombine(a, b, a._flags | b.Flag);
        public static TagsFlags operator ^(TagsFlags a, TagHandle b) => TryCombine(a, b, a._flags ^ b.Flag);
    }
}