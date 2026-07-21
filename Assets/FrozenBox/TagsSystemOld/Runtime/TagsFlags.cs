using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace FrozenBox.TagsSystem
{
    [Serializable]
    public struct TagsFlags : IEquatable<TagsFlags>
    {
        [SerializeField] internal TagsGroup _group;
        [SerializeField] internal int _flags;
        
        public TagsGroup Group => _group;

        public static TagsFlags EmptyOf(TagsGroup group) => new(group, 0);

        internal TagsFlags(TagsGroup group, int flags) {
            _group = group;
            _flags = flags;
        }

        public bool HasTag(Tag tag) {
            Assert.AreEqual(_group, tag.Group);
            return (_flags & tag.Flag) != 0;
        }

        public bool HasAny(TagsFlags flags) {
            Assert.AreEqual(_group, flags._group);
            return (_flags & flags._flags) != 0;
        }
        
        public bool HasAny() => _flags != 0;

        public bool HasAll(TagsFlags flags) {
            Assert.AreEqual(_group, flags._group);
            return (_flags & flags._flags) == flags._flags;
        }

        private static TagsFlags TryCombine(TagsFlags a, TagsFlags b, int flags) {
            Assert.AreEqual(a._group, b._group);
            return new TagsFlags {_group = a._group, _flags = flags};
        }
        
        private static TagsFlags TryCombine(TagsFlags a, Tag b, int flags) {
            Assert.AreEqual(a._group, b.Group);
            return new TagsFlags(a._group, flags);
        }
        
        private static TagsFlags TryCombine(Tag a, Tag b, int flags) {
            Assert.AreEqual(a.Group, b.Group);
            return new TagsFlags {_group = a.Group, _flags = flags};
        }

        public bool Equals(TagsFlags other) => _group.Equals(other._group) && _flags == other._flags;
        public override bool Equals(object? obj) => obj is TagsFlags other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(_group, _flags);

        public static bool operator ==(TagsFlags left, TagsFlags right) => left.Equals(right);
        public static bool operator !=(TagsFlags left, TagsFlags right) => !left.Equals(right);

        public static TagsFlags operator ~(TagsFlags a) => new() { _group = a._group, _flags = ~a._flags };
        
        public static TagsFlags operator &(TagsFlags a, TagsFlags b) => TryCombine(a, b, a._flags & b._flags);
        public static TagsFlags operator |(TagsFlags a, TagsFlags b) => TryCombine(a, b, a._flags | b._flags);
        public static TagsFlags operator ^(TagsFlags a, TagsFlags b) => TryCombine(a, b, a._flags ^ b._flags);
        
        public static TagsFlags operator &(TagsFlags a, Tag b) => TryCombine(a, b, a._flags & b.Flag);
        public static TagsFlags operator |(TagsFlags a, Tag b) => TryCombine(a, b, a._flags | b.Flag);
        public static TagsFlags operator ^(TagsFlags a, Tag b) => TryCombine(a, b, a._flags ^ b.Flag);
    }
}