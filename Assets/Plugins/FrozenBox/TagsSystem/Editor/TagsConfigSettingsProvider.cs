using System.Linq;
using Plugins.FrozenBox.Utils.Editor;
using UnityEditor;
using UnityEngine;

namespace FrozenBox.TagsSystem.Editor
{
    public class TagsConfigSettingsProvider : ConfigSettingsProvider<TagsConfig>
    {
        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider() 
            => CreateSettingsProvider_Internal("Project/FrozenBox/TagsSystem");
    }
}