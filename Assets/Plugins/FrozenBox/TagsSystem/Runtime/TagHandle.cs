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
        public static TagHandle Invalid = new() { _source = TagSource.Invalid, _flag = -1 };
        
        [SerializeField] private TagSource _source;
        [SerializeField] private int _flag;

        public string? GetName() => _source.NameOfIndex(_flag);
        
        internal TagSource Source => _source;
        internal int Flag => _flag;

        internal TagHandle(TagSource source, int flag)
        {
            _source = source;
            _flag = flag;
        }
    }
}