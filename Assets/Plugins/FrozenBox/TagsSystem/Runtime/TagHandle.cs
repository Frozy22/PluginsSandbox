using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace FrozenBox.TagsSystem
{
    [Serializable]
    public struct TagHandle : IEquatable<TagHandle>
    {
        [SerializeField] internal TagSource _source;
        [SerializeField] internal int _value;
    
        internal int Flag {
            get {
                Assert.IsTrue(_source.CanBeFlag);
                return _source.IsFlags ? _value : 1 << _value;
            }
        }
        
        internal TagHandle(TagSource source, int value) {
            _source = source;
            _value = value;
        }

        public FlagsHandle AsFlag() {
            Assert.IsTrue(_source.CanBeFlag);
            return new FlagsHandle(_source, Flag);
        }
            
        public bool Equals(TagHandle other) => _source.Equals(other._source) && _value == other._value;
        public override bool Equals(object? obj) => obj is TagHandle other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(_source, _value);

        public static bool operator ==(TagHandle left, TagHandle right) => left.Equals(right);
        public static bool operator !=(TagHandle left, TagHandle right) => !left.Equals(right);
    }
}