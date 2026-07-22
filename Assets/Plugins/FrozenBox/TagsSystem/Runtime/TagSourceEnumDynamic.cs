using System;
using UnityEngine.Assertions;

namespace FrozenBox.TagsSystem
{
    internal sealed class TagSourceEnumDynamic<T> : TagSourceEnum where T : Enum
    {
        internal override Type EnumType => typeof(T);
        
        public TagHandle CreateTagFrom(T value)
        {
            Assert.IsTrue(!IsFlags || EnumExtensions.IsPowerOfTwo(value));
            return new TagHandle(this, Convert.ToInt32(value));
        }
        
        public FlagsHandle CreateFlagsFrom(T value)
        {
            Assert.IsTrue(CanBeFlag);
            return new FlagsHandle(this, Convert.ToInt32(value));
        }
    }
}