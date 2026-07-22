using FrozenBox.Utils;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace FrozenBox.Utils.Editor
{
    [CustomPropertyDrawer(typeof(FloatRange))]
    public class FloatRangeDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var root = new FloatRangeField(property.displayName);
            root.AddToClassList("unity-base-field__aligned");
            root.BindProperty(property);
            return root;
        }
    }
}