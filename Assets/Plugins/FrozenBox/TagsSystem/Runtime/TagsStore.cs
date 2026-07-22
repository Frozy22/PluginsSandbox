using System;
using System.Collections.Generic;
using FrozenBox.Serialization;
using UnityEngine;

namespace FrozenBox.TagsSystem
{
    [Serializable]
    public class TagsStore : SerializableDictionary<TagsSource, FlagsHandle, TagsStore.FlagsPair>
    {
        public bool HasTag(TagHandle tagHandle) => this.TryGetValue(tagHandle.Source, out var flags) && flags.HasTag(tagHandle);
        public bool HasAny(FlagsHandle flagsHandle) => this.TryGetValue(flagsHandle.Source, out var flags) && flags.HasAny(flagsHandle);
        public bool HasAll(FlagsHandle flagsHandle) => this.TryGetValue(flagsHandle.Source, out var flags) && flags.HasAll(flagsHandle);
        
        [Serializable]
        public struct FlagsPair : ISerializablePair<TagsSource, FlagsHandle>
        {
            [SerializeField]
            private FlagsHandle _value;

            public TagsSource Key
            {
                readonly get => _value.Source;
                set => _value = new FlagsHandle(value, _value.Flags);
            }

            public FlagsHandle Value
            {
                readonly get => _value;
                set => _value = value;
            }

            public bool Equals(FlagsPair other)
            {
                return EqualityComparer<TagsSource>.Default.Equals(Key, other.Key);
            }
        }
    }
}