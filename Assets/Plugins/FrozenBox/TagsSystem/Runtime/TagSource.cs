using System;
using System.Collections.Frozen;
using UnityEngine;

namespace FrozenBox.TagsSystem
{
    public abstract class TagSource : ScriptableObject
    {
        internal abstract int MaxValue { get; }
        internal abstract bool IsFlags { get; }
        internal abstract bool CanBeFlag { get; }
        internal abstract string Name { get; }

        internal FrozenDictionary<string, TagHandle> NameToTagHash { get; set; } = null!;
        internal FrozenDictionary<TagHandle, string> TagToNameHash { get; set; } = null!;
        public FrozenSet<TagHandle> DefinedTags { get; protected set; } = null!;
        public FrozenSet<FlagsHandle> DefinedFlags { get; protected set; } = null!;

        internal TagHandle CreateTag_Internal(int index) => new(this, index);
        internal abstract void UpdateCache_Internal();

        internal void ResetCache_Internal()
        {
            NameToTagHash = null!;
            TagToNameHash = null!;
            DefinedTags = null!;
            DefinedFlags = null!;
        }
    }
}