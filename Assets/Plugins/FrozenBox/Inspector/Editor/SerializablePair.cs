using System;
using System.Collections.Generic;

namespace FrozenBox.Serialization
{
    [Serializable]
    internal struct SerializablePair<TKey, TValue> : IEquatable<SerializablePair<TKey, TValue>>, ISerializablePair<TKey, TValue>
    {
        public TKey Key
        {
            readonly get => _key;
            set => _key = value;
        }

        public TValue Value
        {
            readonly get => _value;
            set => _value = value;
        }

        public TKey _key;
        public TValue _value;

        public SerializablePair(TKey key, TValue value)
        {
            _key = key;
            _value = value;
        }
        
        public void Deconstruct(out TKey key, out TValue value)
        {
            key = _key;
            value = _value;
        }
        
        public bool Equals(SerializablePair<TKey, TValue> other) 
            => EqualityComparer<TKey>.Default.Equals(_key, other._key) 
               && EqualityComparer<TValue>.Default.Equals(_value, other._value);
        
        public override bool Equals(object? obj) => obj is SerializablePair<TKey, TValue> other && Equals(other);
        
        public override int GetHashCode() => HashCode.Combine(_key, _value);

        public static implicit operator SerializablePair<TKey, TValue>(KeyValuePair<TKey, TValue> pair) 
            => new(pair.Key, pair.Value);
        
        public static implicit operator KeyValuePair<TKey, TValue>(SerializablePair<TKey, TValue> pair) 
            => new(pair.Key, pair.Value);
    }
}