using System;
using FrozenBox.Serialization;
using UnityEngine;

namespace FrozenBox.TagsSystem
{
    [Serializable]
    public abstract class TaggedLibrary<TElement> : TaggedLibrary<TElement, TElement>
    {
        protected override TElement Convert(TElement element) => element;
    }
    
    [Serializable]
    public abstract class TaggedLibrary<TElement, TSerializable>
    {
        [SerializeField] private SerializableDictionary<TSerializable, TagsStore> _elements = new();
        public TagsStore GetTagsStore(TElement element) => _elements[Convert(element)];
        protected abstract TSerializable Convert(TElement element);
    }
}