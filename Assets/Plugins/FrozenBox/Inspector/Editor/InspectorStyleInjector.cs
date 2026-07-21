using System;
using System.Collections.Generic;
using FrozenBox.Utils.Editor;
using FrozenBox.Utils;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UIElements;

namespace FrozenBox.Inspector.Editor
{
    [Serializable]
    internal class InspectorStyleInjector : EditorSingletonData<InspectorStyleInjector>
    {
        [SerializeField] 
        private StyleSheet _inspectorStyleSheet = null!;

        public void TryInject(VisualElement rootElement)
        {
            if (rootElement.styleSheets.Contains(_inspectorStyleSheet))
                return;
            
            rootElement.styleSheets.Add(_inspectorStyleSheet);
        }

        public void TryScheduleInject(VisualElement visualElement)
        {
            if (visualElement.styleSheets.Contains(_inspectorStyleSheet))
                return;
            
            visualElement.ScheduledFindRoot(HandleRootFound, 100, onTimedOut: () => Debug.LogWarning("Failed to find root."));
        }

        private void HandleRootFound(VisualElement rootElement)
        {
            if (rootElement.styleSheets.Contains(_inspectorStyleSheet))
                return;
            
            rootElement.styleSheets.Add(_inspectorStyleSheet);
        }
    }
}