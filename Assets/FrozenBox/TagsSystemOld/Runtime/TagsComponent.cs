using UnityEngine;

namespace FrozenBox.TagsSystem
{
    public class TagsComponent : MonoBehaviour
    {
        [SerializeField] private TagsStore _tagsStore = new();
        public TagsStore TagsStore => _tagsStore;
    }
}