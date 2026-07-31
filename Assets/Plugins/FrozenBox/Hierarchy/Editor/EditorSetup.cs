using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FrozenBox.Hierarchy.Editor.Plugins.FrozenBox.Hierarchy.Editor
{
    public static partial class EditorSetup
    {
        private const string IS_SETTUPED_KEY = "HIERARCHY_IS_SETTUPED";
        
        [InitializeOnLoadMethod]
        public static void Setup()
        {
            if (SessionState.GetBool(IS_SETTUPED_KEY, false)) return;
            SessionState.SetBool(IS_SETTUPED_KEY, true);

            /*var tags = new string[] {
                HierarchyExtensions.ROOT_TAG_KEY
            };
            RegisterTags(tags);*/
        }

        private static void RegisterTags(IEnumerable<string> tags)
        {
            Object[] asset = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset");
            if (asset == null || asset.Length <= 0) return;
            
            var scriptableObject = new SerializedObject(asset[0]);
            var tagsProperty = scriptableObject.FindProperty("tags");
            var tagsToAdd = new HashSet<string>(tags);

            for (var i = 0; i < tagsProperty.arraySize; ++i)
            {
                var propertyTag = tagsProperty.GetArrayElementAtIndex(i).stringValue;
                if (tagsToAdd.Remove(propertyTag) && tagsToAdd.Count == 0) return;
            }

            foreach (var tag in tagsToAdd)
            {
                tagsProperty.InsertArrayElementAtIndex(0);
                tagsProperty.GetArrayElementAtIndex(0).stringValue = tag;
            }
                
            scriptableObject.ApplyModifiedProperties();
            scriptableObject.Update();
        }
    }
}