using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FrozenBox.Utils.Runtime
{
    public static class EditorUtils
    {
        private const string UNITY_EDITOR = "UNITY_EDITOR";
        
        [Conditional(UNITY_EDITOR)]
        public static void SetDirty(Object target)
        {
            if (Application.isPlaying) {
                Debug.LogError($"Do not mark as dirty objects in play mode! target: {target}");
                return;
            }
            EditorUtility.SetDirty(target);
        }
    }
}