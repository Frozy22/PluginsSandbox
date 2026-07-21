using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using FrozenBox.Serialization;
using FrozenBox.Utils.Editor;
using FrozenBox.Utils;
using UnityEditor;
using UnityEngine;

namespace FrozenBox.Inspector.Editor.ComponentsManager
{
    [Serializable]
    internal sealed class ComponentsManagerCache : EditorSingletonData<ComponentsManagerCache>, 
        ILRUCache<GlobalObjectId, string>, ISerializationCallbackReceiver
    {
        [SerializeField]
        private List<SerializablePair<string, string>> _hiddenComponents = new();

        public int Count => _hiddenComponentsCache.Count;
        public int Capacity => _hiddenComponentsCache.Capacity;

        private readonly LRUCache<GlobalObjectId, string> _hiddenComponentsCache = new(128);
        
        public void OnBeforeSerialize()
        {
            _hiddenComponents.Clear();
            _hiddenComponents.AddRange(_hiddenComponentsCache
                .Select(pair => new SerializablePair<string, string>(pair.Key.ToString(), pair.Value)));
        }

        public void OnAfterDeserialize()
        {
            _hiddenComponentsCache.Clear();
            
            foreach (var (key, value) in _hiddenComponents)
            {
                if (GlobalObjectId.TryParse(key, out var objectId))
                    _hiddenComponentsCache.Add(objectId, value);
            }
        }

        public void Add(GlobalObjectId key, string value)
        {
            _hiddenComponentsCache.Add(key, value);
            EditorUtility.SetDirty(this);
        }

        public bool Remove(GlobalObjectId key)
        {
            if (_hiddenComponentsCache.Remove(key)) {
                EditorUtility.SetDirty(this);
                return true;
            }
            
            return false;
        }
        
        public bool Remove(GlobalObjectId key, [MaybeNullWhen(false)] out string value)
        {
            if (_hiddenComponentsCache.Remove(key, out value)) {
                EditorUtility.SetDirty(this);
                return true;
            }
            
            return false;
        }

        public bool TryGetValue(GlobalObjectId key, [MaybeNullWhen(false)] out string value) 
            => _hiddenComponentsCache.TryGetValue(key, out value);

        public void Clear()
        {
            _hiddenComponentsCache.Clear();
            EditorUtility.SetDirty(this);
        }

        public void SaveIfDirty()
        {
            AssetDatabase.SaveAssetIfDirty(this);
        }

        public IEnumerator<KeyValuePair<GlobalObjectId, string>> GetEnumerator() 
            => _hiddenComponentsCache.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}