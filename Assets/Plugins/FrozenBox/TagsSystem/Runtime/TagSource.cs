using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace FrozenBox.TagsSystem
{
    [Serializable]
    public partial struct TagSource : IEquatable<TagSource>, ISerializationCallbackReceiver
    {
        public static TagSource Invalid = new() {_sourceType = TagSourceType.INVALID};
        
        [SerializeField] private TagSourceType _sourceType;
        [SerializeField] private string? _enumTypeName;
        [SerializeField] private TagsGroup? _tagsGroup;
        
        private Type? _enumType;
        
        public bool IsValid => _sourceType switch {
            TagSourceType.ENUM => _enumType != null,
            TagSourceType.ASSET => _tagsGroup != null,
            TagSourceType.INVALID => false,
            _ => throw new ArgumentOutOfRangeException()
        };

        public void OnBeforeSerialize() { }

        public void OnAfterDeserialize() {
            if (_sourceType == TagSourceType.ENUM)
            {
                _enumType = Type.GetType(_enumTypeName ?? "");
                if (_enumType == null && !string.IsNullOrWhiteSpace(_enumTypeName))
                    Debug.LogWarning("TagSource: Failed parse enum type!");
            }
        }
        
        public bool Equals(TagSource other) => _sourceType == other._sourceType && GetSourceHashCode() == other.GetSourceHashCode();
        public override bool Equals(object? obj) => obj is TagSource other && Equals(other);
        public override int GetHashCode() => HashCode.Combine((int)_sourceType, GetSourceHashCode());
        
        public static bool operator ==(TagSource? left, TagSource? right) => Equals(left, right);
        public static bool operator !=(TagSource? left, TagSource? right) => !Equals(left, right);
        
        private int GetSourceHashCode() {
            return _sourceType switch {
                TagSourceType.ENUM => _enumType?.GetHashCode() ?? -1,
                TagSourceType.ASSET => _tagsGroup?.GetHashCode() ?? -1,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
        
        public static implicit operator TagSource(TagsGroup? tagsGroup) => From(tagsGroup);

        public static TagSource From(TagsGroup? tagsGroup)
        {
            return new TagSource {
                _sourceType = TagSourceType.ASSET,
                _tagsGroup = tagsGroup
            };
        }

        public static TagSource From<TEnum>() where TEnum : Enum
        {
            return new TagSource {
                _sourceType = TagSourceType.ENUM,
                _enumType = typeof(TEnum),
                _enumTypeName = typeof(TEnum).AssemblyQualifiedName
            };
        }
    }
}