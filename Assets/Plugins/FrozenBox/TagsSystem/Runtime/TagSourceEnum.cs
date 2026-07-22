using System;
using System.Linq;
using UnityEngine;

namespace FrozenBox.TagsSystem
{
    [CreateAssetMenu(menuName = "FrozenBox/TagsSourceEnum", fileName = "New TagsSourceEnum")]
    public class TagSourceEnum : TagsSource
    {
        [SerializeField] private string? _enumTypeName;
        [SerializeField] private ConvertType _convertType;
        private Type? _enumType;
        
        internal Type? EnumType => _enumType;
        internal ConvertType EnumConvertType => _convertType;

        private void Awake()
        {
            TryInitialize();
        }

        internal void TryInitialize()
        {
            Clear();
            _enumType = _enumTypeName != null ? Type.GetType(_enumTypeName) : null;
            
            if (_enumType == null) {
                OnAfterDeserialize();
                return;
            }
            
            var isFlags = _enumType.IsDefined(typeof(FlagsAttribute), false);
            var values = Enum.GetValues(_enumType).Cast<int>().ToList();
            var maxValue = values.Max();
            
            _convertType = isFlags ? ConvertType.FLAGS : (maxValue < 32 ? ConvertType.DIRECT : ConvertType.SEQUENCE);
            switch (_convertType)
            {
                case ConvertType.FLAGS:
                    var index = 0;
                    foreach (var value in values.Where(value => value > 0 && (value & (value - 1)) == 0)) 
                        _tags[index++] = Enum.GetName(_enumType, value)!;
                    break;
                
                case ConvertType.DIRECT:
                    foreach (var value in values) 
                        _tags[value] = Enum.GetName(_enumType, value)!;
                    break;
                
                case ConvertType.SEQUENCE:
                    var names = Enum.GetNames(_enumType);
                    for (var i = 0; i < names.Length; i++) 
                        _tags[i] = names[i];
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
            OnAfterDeserialize();
        }

        internal void Clear()
        {
            _convertType = ConvertType.INVALID;
            _tags = new string[32];
            OnAfterDeserialize();
        }
        
        [Serializable]
        internal enum ConvertType
        {
            INVALID,
            FLAGS,
            DIRECT,
            SEQUENCE
        }
    }
}