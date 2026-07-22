using FrozenBox.TagsSystem;
using UnityEngine;
using TagHandle = FrozenBox.TagsSystem.TagHandle;

namespace DefaultNamespace
{
    public class Test : MonoBehaviour
    {
        [SerializeField] private TagHandle _tagHandle;
        [SerializeField] private FlagsHandle _flagsHandle;
        [SerializeField] private TagsSet _tagsSet;
    }
}