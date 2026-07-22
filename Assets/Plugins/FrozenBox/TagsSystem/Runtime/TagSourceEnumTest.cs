using System;
using UnityEngine;

namespace FrozenBox.TagsSystem
{
    [CreateAssetMenu(fileName = "TagSourceEnumTest", menuName = "FrozenBox/TagSourceEnumTest")]
    internal class TagSourceEnumTest : TagSourceEnum
    {
        internal override Type EnumType => typeof(TestEnum);

        private void Awake()
        {
            UpdateCache_Internal();
        }
    }

    internal enum TestEnum
    {
        Hero = 32,
        Master = 64,
        Legend = 1024
    }
}