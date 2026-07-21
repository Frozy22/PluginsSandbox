using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace FrozenBox.TagsSystem
{
    [CreateAssetMenu(menuName = "FrozenBox/TagsSystem/TagsGroup", fileName = "New TagsGroup")]
    public class TagsGroup : ScriptableObject, ISerializationCallbackReceiver
    {
        public const int FlagsCount = sizeof(int) * 8;
        public const int MinFlag = 0;
        public const int MaxFlag = FlagsCount - 1;
        
        [SerializeField] internal string[] _tags = new string[FlagsCount];
        private FrozenDictionary<string, int> _tagsHash = null!;

        public void OnBeforeSerialize() {
            Array.Resize(ref _tags, FlagsCount);
        }

        public void OnAfterDeserialize()
        {
            Array.Resize(ref _tags, FlagsCount);
            
            _tagsHash = _tags.Select((tagName, index) => (index, name: tagName))
                .Where(tag => !string.IsNullOrWhiteSpace(tag.name))
                .ToFrozenDictionary(tag => tag.name, tag => tag.index);
        }

        public bool HasName(string tagName) => _tagsHash.ContainsKey(tagName);
        public string NameOfIndex(int index) => _tags[index];
        public int IndexOfName(string tagName) => _tagsHash[tagName];
        
        public string NameOfTag(TagHandle tag)
        {
            Assert.AreEqual(tag.Source, this);
            return NameOfIndex(tag.Flag);
        }

        public TagHandle? TagOfName(string tagName)
        {
            if (_tagsHash.TryGetValue(tagName, out var index))
                return new TagHandle(this, index);

            return null;
        }

        public TagHandle TagOfIndex(int tagIndex)
        {
            Assert.IsTrue(tagIndex >= MinFlag && tagIndex <= MaxFlag, $"Flag={tagIndex} is out of range [{MinFlag}..{MaxFlag}].");
            return new TagHandle(this, tagIndex);
        }

        public IEnumerable<string> GetNames() => _tags.Where(tag => !string.IsNullOrWhiteSpace(tag));
        public IEnumerable<string> GetRawNames() => _tags;
        
        public IEnumerable<TagHandle> GetValues() => _tags.Select((tag, index) 
            => (tag, index)).Where(tag => !string.IsNullOrWhiteSpace(tag.tag))
            .Select(tag => new TagHandle(this, tag.index));
    }
}