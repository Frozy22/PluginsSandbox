using System;
using UnityEngine;

namespace FrozenBox.TagsSystem
{
    [Serializable]
    public struct TagHandle : IEquatable<TagHandle>
    {
        [SerializeField] private TagsSource _source;
        [SerializeField] private int _index;

        internal TagHandle(TagsSource source, int index) {
            _source = source;
            _index = index;
        }
        
        internal readonly TagsSource Source => _source;
        internal readonly int Index => _index;
        internal readonly int Flag => 1 << _index;
        
        public bool Equals(TagHandle other) => _source.Equals(other._source) && _index == other._index;
        public override bool Equals(object? obj) => obj is TagHandle other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(_source, _index);

        public static FlagsHandle operator ~(TagHandle a) => new(a._source, ~a.Flag);
        public static bool operator ==(TagHandle left, TagHandle right) => left.Equals(right);
        public static bool operator !=(TagHandle left, TagHandle right) => !left.Equals(right);
    }
}