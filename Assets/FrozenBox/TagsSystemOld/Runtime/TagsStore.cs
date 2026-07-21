using System;
using System.Collections.Generic;
using FrozenBox.Serialization;
using UnityEngine;

namespace FrozenBox.TagsSystem
{
    [Serializable]
    public class TagsStore : SerializableDictionary<TagsGroup, TagsFlags, TagsStore.FlagsPair>
    {
        public bool HasTag(Tag tag) => this.TryGetValue(tag._group, out var flags) && flags.HasTag(tag);
        public bool HasAny(TagsFlags inFlags) => this.TryGetValue(inFlags._group, out var flags) && flags.HasAny(inFlags);
        public bool HasAll(TagsFlags inFlags) => this.TryGetValue(inFlags._group, out var flags) && flags.HasAll(inFlags);
        
        [Serializable]
        public struct FlagsPair : ISerializablePair<TagsGroup, TagsFlags>
        {
            [SerializeField]
            private TagsFlags _value;

            public TagsGroup Key
            {
                readonly get => _value._group;
                set => _value = new TagsFlags(value, _value._flags);
            }

            public TagsFlags Value
            {
                readonly get => _value;
                set => _value = value;
            }

            public bool Equals(FlagsPair other)
            {
                return EqualityComparer<TagsGroup>.Default.Equals(Key, other.Key);
            }
        }
    }
}