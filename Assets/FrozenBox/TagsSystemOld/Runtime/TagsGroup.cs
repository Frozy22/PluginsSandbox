using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using FrozenBox.Utils;
using UnityEngine;
using UnityEngine.Assertions;

namespace FrozenBox.TagsSystem
{
    [CreateAssetMenu(menuName = "FrozenBox/Tags Group", fileName = "New Tags Group")]
    public class TagsGroup : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField] internal string[] _tags = new string[32];
        private FrozenDictionary<string, int> _tagsHash = null!;

        public void OnBeforeSerialize() {
            Array.Resize(ref _tags, 32);
        }

        public void OnAfterDeserialize()
        {
            Array.Resize(ref _tags, 32);
            
            _tagsHash = _tags.Select((tagName, index) => (index, name: tagName))
                .Where(tag => !string.IsNullOrWhiteSpace(tag.name))
                .ToFrozenDictionary(tag => tag.name, tag => tag.index);
        }

        public bool HasName(string tagName) => _tagsHash.ContainsKey(tagName);
        public string NameOfIndex(int index) => _tags[index];
        public int IndexOfName(string tagName) => _tagsHash[tagName];
        
        public string NameOfTag(Tag tag)
        {
            Assert.AreEqual(tag.Group, this);
            return NameOfIndex(tag._index);
        }

        public Tag? TagOfName(string tagName)
        {
            if (_tagsHash.TryGetValue(tagName, out var index))
                return new Tag(this, index);

            return null;
        }

        public Tag TagOfIndex(int tagIndex)
        {
            FrAssert.IsInRange(tagIndex, 0, _tags.Length - 1);
            return new Tag(this, tagIndex);
        }

        public IEnumerable<string> GetNames() => _tags.Where(tag => !string.IsNullOrWhiteSpace(tag));
        public IEnumerable<string> GetRawNames() => _tags;
    }
}