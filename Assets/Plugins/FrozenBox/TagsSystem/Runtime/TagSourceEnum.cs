using System;
using System.Collections.Frozen;
using System.Linq;
using UnityEngine;

namespace FrozenBox.TagsSystem
{
    [CreateAssetMenu(menuName = "FrozenBox/TagsSourceEnum", fileName = "New TagsSourceEnum")]
    public class TagSourceEnum : TagsSource
    {
        [SerializeField] private string? _enumTypeName;

        internal Type? EnumType { get; private set; }
        
        internal readonly Enum[] toEnumHash = new Enum[32];
        internal FrozenDictionary<Enum, TagHandle>? toTagHash;

        private void Awake()
        {
            TryInitialize();
        }

        internal void TryInitialize()
        {
            Clear();
            EnumType = _enumTypeName != null ? Type.GetType(_enumTypeName) : null;
            
            if (EnumType == null) {
                OnAfterDeserialize();
                return;
            }
            
            var values = Enum.GetValues(EnumType).Cast<Enum>().ToList();
            
            if (EnumType.IsDefined(typeof(FlagsAttribute), false))
            {
                var index = 0;
                foreach (var value in values.Where(EnumSupport.IsPowerOfTwo)) 
                {
                    toEnumHash[index] = value;
                    _tags[index++] = Enum.GetName(EnumType, value)!;
                }
            } 
            else if (Convert.ToInt32(values.Max()) < 32)
            {
                foreach (var value in values)
                {
                    var intValue = Convert.ToInt32(value);
                    toEnumHash[intValue] = value;
                    _tags[intValue] = Enum.GetName(EnumType, value)!;
                }
            }
            else
            {
                var names = Enum.GetNames(EnumType);
                for (var i = 0; i < names.Length; i++)
                {
                    toEnumHash[i] = (Enum)Enum.Parse(EnumType, names[i]);
                    _tags[i] = names[i];
                }
            }

            toTagHash = toEnumHash.Where(value => value != null).Select((value, index) => (value, index))
                .ToFrozenDictionary(pair => pair.value, pair => new TagHandle(this, pair.index));
            OnAfterDeserialize();
        }

        internal void Clear()
        {
            _tags = new string[32];
            Array.Fill(toEnumHash, null);
            toTagHash = null;
            OnAfterDeserialize();
        }
    }
}