using System;
using System.Collections.Generic;
using System.Linq;
using FrozenBox.Utils.Editor;
using FrozenBox.Utils;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace FrozenBox.Inspector.Editor
{
    internal abstract class GameObjectInspectorExtension : UnityEditor.Editor
    {
        protected abstract Type DefaultEditorType { get; }
        
        private UnityEditor.Editor? _baseEditor;
        private readonly List<GameObjectInspectorContext> _contexts = new();
        
        private void OnEnable()
        {
            if (_baseEditor != null) 
                DestroyImmediate(_baseEditor);

            _baseEditor = serializedObject.isEditingMultipleObjects 
                ? CreateEditor(serializedObject.targetObjects, DefaultEditorType) 
                : CreateEditor(serializedObject.targetObject, DefaultEditorType);
            
            foreach (var context in _contexts) 
                DestroyImmediate(context);
                
            _contexts.Clear();

            foreach (var type in TypeCache.GetTypesDerivedFrom<GameObjectInspectorContext>().Where(type => !type.IsAbstract))
            {
                var context = (GameObjectInspectorContext)EditorUtils.CreateEditor(serializedObject, type);
                _contexts.Add(context);
            }
        }
        
        public override VisualElement CreateInspectorGUI()
        {
            if (_baseEditor == null) {
                Debug.LogError("BaseEditor is null");
                return new Label("!!!BaseEditor is null!!!");
            }
            
            var root = new VisualElement();
            var imGuiContainer = new IMGUIContainer(() => _baseEditor.OnInspectorGUI());
            root.Add(imGuiContainer);

            root.ScheduledFindRoot(inspectorRoot => ConstructContext(root, inspectorRoot), 
                                    onTimedOut: () => Debug.LogError("Failed to find inspector root."));
            return root;
        }

        private void ConstructContext(VisualElement contextContainer, VisualElement inspectorRoot)
        {
            InspectorStyleInjector.Instance.TryInject(inspectorRoot);
            
            foreach (var context in _contexts)
            {
                var divider = new VisualElement();
                divider.AddToClassList("fr--go-inspector-context__divider");
                contextContainer.Add(divider);
                
                context.Setup(inspectorRoot);
                contextContainer.Add(context.VisualElement);

                void HandleVisibilityChanged()
                {
                    if (context.IsVisible)
                        divider.RemoveFromClassList(StyleConstants.CLASS_HIDDEN_ELEMENT);
                    else 
                        divider.AddToClassList(StyleConstants.CLASS_HIDDEN_ELEMENT);
                }

                context.OnVisibilityChanged += HandleVisibilityChanged;
                HandleVisibilityChanged();
            }
        }

        private void OnDisable()
        {
            if (_baseEditor != null) 
                DestroyImmediate(_baseEditor);
            
            foreach (var context in _contexts) 
                DestroyImmediate(context);
                
            _contexts.Clear();
        }
    }
}