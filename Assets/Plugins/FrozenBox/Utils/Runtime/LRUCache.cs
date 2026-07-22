using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace FrozenBox.Utils
{
    // ReSharper disable once InconsistentNaming
    public class LRUCache<TKey, TValue> : ILRUCache<TKey, TValue>
    {
        private readonly LinkedList<KeyValuePair<TKey, TValue>> _list;
        private readonly Dictionary<TKey, LinkedListNode<KeyValuePair<TKey, TValue>>> _dictionary;
        
        public int Count => _list.Count;
        public int Capacity { get; }

        public LRUCache(int capacity)
        {
            Capacity = capacity;
            _list = new LinkedList<KeyValuePair<TKey, TValue>>();
            _dictionary = new Dictionary<TKey, LinkedListNode<KeyValuePair<TKey, TValue>>>(capacity);
        }

        public void Add(TKey key, TValue value)
        {
            if (_dictionary.TryGetValue(key, out var node))
            {
                node.Value = new KeyValuePair<TKey,TValue>(key, value);
                _list.Remove(node);
                _list.AddFirst(node);
                return;
            }
            
            if (_dictionary.Count >= Capacity)
            {
                var lastKey = _list.Last.Value.Key;
                _list.RemoveLast();
                _dictionary.Remove(lastKey);
            }
            
            var addedNode = _list.AddFirst(new KeyValuePair<TKey,TValue>(key, value));
            _dictionary.Add(key, addedNode);
        }

        public bool Remove(TKey key)
        {
            if (_dictionary.Remove(key, out var node))
            {
                _list.Remove(node);
                return true;
            }
            
            return false;
        }

        public bool Remove(TKey key, [MaybeNullWhen(false)] out TValue value)
        {
            if (_dictionary.Remove(key, out var node))
            {
                _list.Remove(node);
                value = node.Value.Value;
                return true;
            }
            
            value = default;
            return false;
        }

        public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
        {
            if (!_dictionary.TryGetValue(key, out var node))
            {
                value = default;
                return false;
            }

            value = node.Value.Value;
            _list.Remove(node);
            _list.AddFirst(node);
            return true;
        }

        public void Clear()
        {
            _list.Clear();
            _dictionary.Clear();
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() 
            => _list.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() 
            => GetEnumerator();
    }
}