using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

namespace FrozenBox.Serialization
{
    [Serializable]
    public class SerializableDictionary<TKey, TValue, TPair> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    where TPair : struct, ISerializablePair<TKey, TValue>
    {
        [SerializeField]
        private List<TPair> _valuePairs = new();
        
#if UNITY_EDITOR
        private readonly Dictionary<TKey, TValue> _failedDeserialize = new();
        private readonly List<TValue> _failedDeserializeNull = new();
#endif

        public void OnBeforeSerialize()
        {
            _valuePairs.Clear();
            
            foreach (var keyValuePair in this) 
                _valuePairs.Add(new TPair {
                    Key = keyValuePair.Key,
                    Value = keyValuePair.Value
                });
            
#if UNITY_EDITOR
            foreach (var keyValuePair in _failedDeserialize) 
                _valuePairs.Add(new TPair {
                    Key = keyValuePair.Key,
                    Value = keyValuePair.Value
                });
            
            foreach (var value in _failedDeserializeNull) 
                _valuePairs.Add(new TPair {
                    Key = default!,
                    Value = value
                });
#endif
        }

        public void OnAfterDeserialize()
        {
            this.Clear();
#if UNITY_EDITOR
            _failedDeserialize.Clear();
            _failedDeserializeNull.Clear();
            
            foreach (var valuePair in _valuePairs)
            {
                if (valuePair.Key == null) {
                    _failedDeserializeNull.Add(valuePair.Value);
                    Debug.LogError($"Failed deserialize pair: key is null, value is [{valuePair.Value}]");
                    continue;
                }
                if (!this.TryAdd(valuePair.Key, valuePair.Value)) 
                    _failedDeserialize.Add(valuePair.Key, valuePair.Value);
            }
#else
            foreach (var valuePair in _valuePairs) 
                this.Add(valuePair.Key, valuePair.Value);
            
            _valuePairs.Clear();
#endif
        }
        
        public static TPair PairWithValue(TPair pair, TValue value) 
            => new() { Key = pair.Key, Value = value };
    }

    [Serializable]
    public class SerializableDictionary<TKey, TValue> : SerializableDictionary<TKey, TValue, SerializableDictionary<TKey, TValue>.SerializablePair>
    {
        [Serializable]
        public struct SerializablePair : ISerializablePair<TKey, TValue>
        {
            [SerializeField]
            private TKey _key;
            
            [SerializeField]
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