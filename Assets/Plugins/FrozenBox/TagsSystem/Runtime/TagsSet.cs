using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace FrozenBox.TagsSystem
{
    [Serializable]
    public class TagsSet : ISerializationCallbackReceiver
    {
        [SerializeField] private TagSource _source = null!;
        [SerializeField] private int[] _tags = Array.Empty<int>();
        
        private HashSet<TagHandle> _internalSet = new();
        
        public bool Add(TagHandle item) => _internalSet.Add(item);
        public bool Remove(TagHandle item) => _internalSet.Remove(item);
        public void Clear() => _internalSet.Clear();
        
        public bool HasTag(TagHandle tag) {
            Assert.AreEqual(_source, tag._source);
            return _internalSet.Contains(tag);
        }

        public bool HasAny(TagsSet tagsSet) {
            Assert.AreEqual(_source, tagsSet._source);
            return _internalSet.Overlaps(tagsSet._internalSet);
        }

        public bool HasAny() => _internalSet.Count > 0;

        public bool HasAll(TagsSet tagsSet) {
            Assert.AreEqual(_source, tagsSet._source);
            return _internalSet.IsSupersetOf(tagsSet._internalSet);
        }
        
        public void OnBeforeSerialize() {
            _tags = _internalSet.Select(tag => tag._value).ToArray();
        }

        public void OnAfterDeserialize() {
            _internalSet.Clear();
#if UNITY_EDITOR
            if (_tags == null!) return;
#endif
            _internalSet.UnionWith(_tags.Select(value => new TagHandle(_source, value)));
#if !UNITY_EDITOR
            _tags = Array.Empty<TagHandle>();
#endif
        }
    }
}