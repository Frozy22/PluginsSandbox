using System;
using System.Collections.Generic;
using System.Linq;

namespace FrozenBox.TagsSystem
{
    public partial struct TagSource
    {
        internal Type? EnumType => _enumType ??= Type.GetType(_enumTypeName ?? "");
        
        internal string? NameOfIndex(int index)
        {
            return _sourceType switch {
                TagSourceType.ENUM => _enumType != null ? Enum.GetName(_enumType, index) : null,
                TagSourceType.ASSET => _tagsGroup?.NameOfIndex(index),
                _ => throw new ArgumentOutOfRangeException()
            } ?? null;
        }

        public string GetShortName()
        {
            switch (_sourceType)
            {
                case TagSourceType.ENUM:
                    if (string.IsNullOrWhiteSpace(_enumTypeName)) return "<NULL ENUM TYPE>";
                    var fullName = _enumTypeName[.._enumTypeName.IndexOf(',')];
                    return fullName[(fullName.LastIndexOf('.')+1)..];
                
                case TagSourceType.ASSET:
                    return _tagsGroup?.name ?? "<NULL ASSET TYPE>";
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        public readonly TagHandle GetByName(string name)
        {
            if (TryGetByName(name, out var handle)) return handle;
            throw new KeyNotFoundException();
        }

        public readonly bool TryGetByName(string name, out TagHandle handle)
        {
            switch (_sourceType)
            {
                case TagSourceType.ENUM:
                    if (_enumType != null && Enum.TryParse(_enumType, name, out var value)) {
                        handle = new TagHandle(this, Convert.ToInt32(value));
                        return true;
                    } break;
                
                case TagSourceType.ASSET:
                    var tempHandle = _tagsGroup?.TagOfName(name);
                    if (tempHandle != null) {
                        handle = tempHandle.Value;
                        return true;
                    } break;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }

            handle = TagHandle.Invalid;
            return false;
        }

        public readonly IEnumerable<string> GetNames()
        {
            return _sourceType switch {
                TagSourceType.ENUM => _enumType != null ? Enum.GetNames(_enumType) : null,
                TagSourceType.ASSET => _tagsGroup?.GetNames(),
                _ => throw new ArgumentOutOfRangeException()
            } ?? Array.Empty<string>();
        }

        public readonly IEnumerable<TagHandle> GetValues()
        {
            var source = this;
            return _sourceType switch {
                TagSourceType.ENUM => _enumType != null ? Enum.GetValues(_enumType).Cast<int>()
                    .Select(value => new TagHandle(source, value)) : null,
                TagSourceType.ASSET => _tagsGroup?.GetValues(),
                _ => throw new ArgumentOutOfRangeException()
            } ?? Array.Empty<TagHandle>();
        }
    }
}