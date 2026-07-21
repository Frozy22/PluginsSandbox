using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace FrozenBox.Inspector.Editor
{
    [CustomEditor(typeof(RectTransform))]
    [CanEditMultipleObjects]
    internal sealed class GameObjectInspectorExtensionWithRectTransform : GameObjectInspectorExtension
    {
        private static readonly Type RectTransformEditorType 
            = TypeCache.GetTypesDerivedFrom<UnityEditor.Editor>().First(type => type.Name == "RectTransformEditor");

        protected override Type DefaultEditorType => RectTransformEditorType;
    }
}