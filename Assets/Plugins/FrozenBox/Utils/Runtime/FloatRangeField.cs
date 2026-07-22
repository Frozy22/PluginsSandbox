using UnityEngine.UIElements;

namespace FrozenBox.Utils
{
    [UxmlElement]
    public partial class FloatRangeField : BaseField<FloatRange>
    {
        [UxmlAttribute]
        public float Min
        {
            get => value.Min;
            set
            {
                _changeFlag = -1;
                
                if (value > Max)
                {
                    _minField.SetValueWithoutNotify(Max);
                    this.value = new FloatRange(Max, Max);
                    _changeFlag = 0;
                    return;
                }
                
                this.value = new FloatRange(value, Max);
                _changeFlag = 0;
            }
        }

        [UxmlAttribute]
        public float Max
        {
            get => value.Max;
            set
            {
                _changeFlag = 1;
                
                if (value < Min)
                {
                    _maxField.SetValueWithoutNotify(Min);
                    this.value = new FloatRange(Min, Min);
                    _changeFlag = 0;
                    return;
                }
                
                this.value = new FloatRange(Min, value);
                _changeFlag = 0;
            }
        }
        
        private readonly FloatField _minField;
        private readonly FloatField _maxField;
        private int _changeFlag;

        public FloatRangeField() : this(null) { }

        public FloatRangeField(string? label) : base(label, new VisualElement())
        {
            AddToClassList("unity-composite-field");
            
            var inputContainer = this.Q<VisualElement>(className: inputUssClassName);
            inputContainer.style.flexDirection = FlexDirection.Row;
            inputContainer.AddToClassList("unity-composite-field__input");
            
            _minField = new FloatField("Min")
            {
                bindingPath = "Min"
            };
            _minField.AddToClassList("unity-composite-field__field");
            _minField.AddToClassList("unity-composite-field__field--first");
            _minField.labelElement.style.flexShrink = 1;
            _minField.labelElement.style.minWidth = 30;
            _minField.RegisterValueChangedCallback(evt => Min = evt.newValue);
            inputContainer.Add(_minField);

            _maxField = new FloatField("Max")
            {
                bindingPath = "Max",
                style =
                {
                    marginRight = 0
                }
            };
            _maxField.AddToClassList("unity-composite-field__field");
            _maxField.labelElement.style.flexShrink = 1;
            _maxField.labelElement.style.minWidth = 30;
            _maxField.RegisterValueChangedCallback(evt => Max = evt.newValue);
            inputContainer.Add(_maxField);
        }
        
        protected override void UpdateMixedValueContent()
        {
            _minField.showMixedValue = this.showMixedValue;
            _maxField.showMixedValue = this.showMixedValue;
        }
        
        public override void SetValueWithoutNotify(FloatRange newValue)
        {
            base.SetValueWithoutNotify(newValue);
            
            if (_changeFlag < 1)
                _minField.SetValueWithoutNotify(newValue.Min);
            
            if (_changeFlag > -1)
                _maxField.SetValueWithoutNotify(newValue.Max);
        }
    }
}