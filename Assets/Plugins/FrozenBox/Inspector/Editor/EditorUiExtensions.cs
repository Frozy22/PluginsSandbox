using System;
using FrozenBox.Utils;
using UnityEngine.UIElements;

namespace FrozenBox.Inspector.Editor
{
    internal static class EditorUiExtensions
    {
        public static void ScheduledInit(this VisualElement visualElement, Func<bool> tryInitFunc, int period = 10)
        {
            var isInited = false;
            visualElement.schedule.Execute(() => isInited = tryInitFunc())
                .Until(() => isInited).Every(period);
        }

        public static void ScheduledFindRoot(this VisualElement visualElement, Action<VisualElement> onFoundRoot, int period = 10, int timeout = 1000, Action? onTimedOut = null)
        {
            VisualElement? inspectorRoot = null;
            var currentTime = 0;
            IVisualElementScheduledItem schedule = null!;
            schedule = visualElement.schedule.Execute(TryFindRoot).Every(period);
            return;

            void TryFindRoot()
            {
                if (currentTime > timeout)
                {
                    onTimedOut?.Invoke();
                    schedule.Pause();
                    return;
                }
                
                inspectorRoot = visualElement.QAncestor(null, "unity-inspector-main-container");

                if (inspectorRoot != null)
                {
                    onFoundRoot(inspectorRoot);
                    schedule.Pause();
                    return;
                }
                
                currentTime += period;
            }
            
        }
    }
}