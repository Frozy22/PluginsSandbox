using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Assertions;

[assembly: InternalsVisibleTo("FrozenBox.TagsSystem.Editor")]

namespace FrozenBox.TagsSystem
{
    [Serializable]
    public struct TagHandle
    {
        public static TagHandle Invalid = new() { _source = TagSource.Invalid, _index = -1 };
        
        [SerializeField] private TagSource _source;
        [SerializeField] private int _index;

        public string? GetName() => _source.NameOfIndex(_index);
        
        internal TagSource Source => _source;
        internal int Flag => 1 << _index;

        internal TagHandle(TagSource source, int index)
        {
            _source = source;
            _index = index;
        }
    }
}