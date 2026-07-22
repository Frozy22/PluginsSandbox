using System;
using System.Collections.Frozen;
using System.Linq;
using UnityEngine;

namespace FrozenBox.TagsSystem
{
    [CreateAssetMenu(menuName = "FrozenBox/Tags/TagSourceCustom", fileName = "TagSourceCustom")]
    public sealed class TagSourceCustom : TagSource, ISerializationCallbackReceiver
    {
        private const int LIMIT = sizeof(int) * 8;
        
        [SerializeField] private string[] _tags = new string[LIMIT];
  
        private int _maxIndex;

        internal override int MaxValue => _maxIndex;
        internal override bool IsFlags => false;
        internal override bool CanBeFlag => true;
        internal override string Name => name;

        public void OnBeforeSerialize() {
            Array.Resize(ref _tags, LIMIT);
        }

        public void OnAfterDeserialize()
        {
            Array.Resize(ref _tags, LIMIT);
            UpdateCache_Internal();
        }

        internal override void UpdateCache_Internal()
        {
            for (var i = _tags.Length - 1; i >= 0; i--) {
                if (string.IsNullOrWhiteSpace(_tags[i])) continue;
                _maxIndex = i;
                break;
            }
            
            NameToTagHash = _tags.Select((tagName, index) => (index, name: tagName))
                .Where(tag => !string.IsNullOrWhiteSpace(tag.name))
                .ToFrozenDictionary(tag => tag.name, tag => new TagHandle(this, tag.index));
            
            TagToNameHash = NameToTagHash.ToFrozenDictionary(pair => pair.Value, pair => pair.Key);
            DefinedTags = NameToTagHash.Select(pair => pair.Value).ToFrozenSet();
            DefinedFlags = DefinedTags.Select(tag => new FlagsHandle(this, tag.Flag)).ToFrozenSet();
        }
    }
}