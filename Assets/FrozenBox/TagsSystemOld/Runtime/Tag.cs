using System;
using UnityEngine;

namespace FrozenBox.TagsSystem
{
    [Serializable]
    public struct Tag : IEquatable<Tag>
    {
        [SerializeField] internal TagsGroup _group;
        [SerializeField] internal int _index;

        internal Tag(TagsGroup group, int index) {
            _group = group;
            _index = index;
        }
        
        public readonly TagsGroup Group => _group;
        internal readonly int Flag => 1 << _index;
        
        public bool Equals(Tag other) => _group.Equals(other._group) && _index == other._index;
        public override bool Equals(object? obj) => obj is Tag other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(_group, _index);

        public static TagsFlags operator ~(Tag a) => new() { _group = a._group, _flags = ~a.Flag };
        public static bool operator ==(Tag left, Tag right) => left.Equals(right);
        public static bool operator !=(Tag left, Tag right) => !left.Equals(right);
    }
}