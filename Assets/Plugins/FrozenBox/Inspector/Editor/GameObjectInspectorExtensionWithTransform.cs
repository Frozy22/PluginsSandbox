using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace FrozenBox.Inspector.Editor
{
    [CustomEditor(typeof(Transform))]
    [CanEditMultipleObjects]
    internal sealed class GameObjectInspectorExtensionWithTransform : GameObjectInspectorExtension
    {
        private static readonly Type TransformEditorType 
            = TypeCache.GetTypesDerivedFrom<UnityEditor.Editor>().First(type => type.Name == "TransformInspector");

        protected override Type DefaultEditorType => TransformEditorType;
    }
}