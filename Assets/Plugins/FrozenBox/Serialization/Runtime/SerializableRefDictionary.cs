using System;
using System.Collections.Generic;
using UnityEngine;

namespace FrozenBox.Serialization
{
    [Serializable]
    public class SerializableRefDictionary<TKey, TValue> : SerializableDictionary<TKey, TValue, SerializableRefDictionary<TKey, TValue>.SerializablePair>
    {
        [Serializable]
        public struct SerializablePair : ISerializablePair<TKey, TValue>
        {
            [SerializeField]
            private TKey _key;
            
            [SerializeReference]
            private TValue _value;

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

            public bool Equals(SerializablePair other)
            {
                return EqualityComparer<TKey>.Default.Equals(Key, other.Key);
            }
        }
    }
}