using System;
using System.Collections.Frozen;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace FrozenBox.TagsSystem
{
    internal abstract class TagSourceEnum : TagSource
    {
        internal abstract Type EnumType { get; }
        
        private int _maxValue;
        private bool _isFlags;
        private bool _canBeFlag;

        internal override int MaxValue => _maxValue;
        internal override bool IsFlags => _isFlags;
        internal override bool CanBeFlag => _canBeFlag;
        internal override string Name => EnumType.Name;

        internal override void UpdateCache_Internal()
        {
            var rawValues = Enum.GetValues(EnumType).Cast<Enum>().ToArray();
            _isFlags = EnumType.IsDefined(typeof(FlagsAttribute), false);
            _maxValue = rawValues.Max(Convert.ToInt32);
            _canBeFlag = _isFlags || _maxValue < sizeof(int) * 8;

            DefinedTags = _isFlags 
                ? rawValues.Where(value => !EnumExtensions.IsPowerOfTwo(value)).Select(CreateTagFrom_Internal).ToFrozenSet() 
                : rawValues.Select(CreateTagFrom_Internal).ToFrozenSet();

            DefinedFlags = CanBeFlag 
                ? rawValues.Select(CreateFlagsFrom_Internal).ToFrozenSet() 
                : FrozenSet<FlagsHandle>.Empty;
            
            NameToTagHash = DefinedTags.ToFrozenDictionary(tag => Enum.GetName(EnumType, tag._value), tag => tag);
            TagToNameHash = NameToTagHash.ToFrozenDictionary(pair => pair.Value, pair => pair.Key);
        }
        
        internal TagHandle CreateTagFrom_Internal(Enum value)
        {
            Assert.IsTrue(!_isFlags || EnumExtensions.IsPowerOfTwo(value));
            return new TagHandle(this, Convert.ToInt32(value));
        }
        
        internal FlagsHandle CreateFlagsFrom_Internal(Enum value)
        {
            Assert.IsTrue(CanBeFlag);
            return new FlagsHandle(this, Convert.ToInt32(value));
        }
    }
}