using System;
using System.Collections.Generic;

namespace FrozenBox.Serialization
{
    internal interface ISerializablePair<TKey, TValue> : IEquatable<ISerializablePair<TKey, TValue>>
    {
        public TKey Key { get; set; }
        public TValue Value { get; set; }
        
        bool IEquatable<ISerializablePair<TKey, TValue>>.Equals(ISerializablePair<TKey, TValue> other) 
            => EqualityComparer<TKey>.Default.Equals(Key, other.Key);

        public bool Equals(object obj) 
            => obj is ISerializablePair<TKey, TValue> other && Equals(other);

        public int GetHashCode() 
            => EqualityComparer<TKey>.Default.GetHashCode(Key);
    }
}