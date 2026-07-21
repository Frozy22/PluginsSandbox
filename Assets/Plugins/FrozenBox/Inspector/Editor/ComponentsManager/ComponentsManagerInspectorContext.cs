using System;
using System.Collections.Generic;
using System.Linq;
using FrozenBox.Utils;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace FrozenBox.Inspector.Editor.ComponentsManager
{
    internal sealed class ComponentsManagerInspectorContext : GameObjectInspectorContext
    {
        private HashSet<Type>? _commonTypesSet;
        private ComponentsManagerElement? _managerElement;

        public override bool CanShow() 
            => _commonTypesSet is { Count: > 1 };

        private void OnEnable()
        {
            var targetGameObject = ((Transform)serializedObject.targetObject).gameObject;
            _commonTypesSet = new HashSet<Type>(targetGameObject.GetComponents<Component>()
                .Where(element => element != null).Select(component => component.GetType()));

            foreach (var gameObject in serializedObject.targetObjects.Select(obj => ((Transform)obj).gameObject)) 
                _commonTypesSet.IntersectWith(gameObject.GetComponents<Component>()
                .Where(element => element != null).Select(component => component.GetType()));
            
            _commonTypesSet.Remove(serializedObject.targetObject.GetType());
        }

        public override VisualElement CreateInspectorGUI()
        {
            if (_commonTypesSet == null) {
                Debug.LogError("_commonTypesSet is null");
                return new Label("!!!_commonTypesSet is null!!!");
            }
            
            _managerElement = new ComponentsManagerElement(InspectorRoot, _commonTypesSet);
            var objectId = GlobalObjectId.GetGlobalObjectIdSlow(serializedObject.targetObject);

            if (ComponentsManagerCache.Instance.TryGetValue(objectId, out var savedString))
            {
                var commonTypesDict = _commonTypesSet.ToDictionary(type => type.Name, type => type);
                var hiddenComponents = savedString.Split('|')
                    .Select(commonTypesDict.GetValueOrDefault);
                
                _managerElement.SetHiddenComponents(hiddenComponents);
            }
            
            return _managerElement;
        }

        private void OnDisable()
        {
            if (_managerElement == null)
                return;

            var cacheInstance = ComponentsManagerCache.Instance;
            
            var ids = new GlobalObjectId[serializedObject.targetObjects.Length];
            GlobalObjectId.GetGlobalObjectIdsSlow(serializedObject.targetObjects, ids);
            
            var hiddenComponents = string.Join('|', _managerElement.HiddenComponents.Select(type => type.Name));

            if (string.IsNullOrEmpty(hiddenComponents))
            {
                foreach (var objectId in ids) 
                    cacheInstance.Remove(objectId);
                
                return;
            }
            
            foreach (var objectId in ids) 
                cacheInstance.Add(objectId, hiddenComponents);
        }
    }
}