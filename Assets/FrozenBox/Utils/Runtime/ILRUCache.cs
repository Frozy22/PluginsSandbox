using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace FrozenBox.Utils
{
    // ReSharper disable once InconsistentNaming
    public interface ILRUCache<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
    {
        public int Count { get; }
        public int Capacity { get; }
        
        public void Add(TKey key, TValue value);
        public bool Remove(TKey key);
        public bool Remove(TKey key, [MaybeNullWhen(false)] out TValue value);
        public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value);
        
        public void Clear();
    }
}