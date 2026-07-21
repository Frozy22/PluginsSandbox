using System;
using UnityEngine;

namespace FrozenBox.Utils
{
    public static class RectExtensions
    {
        public static Rect WithCenter(this Rect rect, Vector2 center) 
            => new(rect.position + center - rect.center, rect.size);
        
        public static Rect WithSize(this Rect rect, Vector2 size) 
            => new(rect.position, size);

        public static Rect WithSize(this Rect rect, Vector2 size, Alignment alignment)
        {
            var diffSize = size - rect.size;
            var position = alignment switch
            {
                Alignment.TopLeft => new Vector2(rect.xMin, rect.yMin - diffSize.y),
                Alignment.TopCenter => new Vector2(rect.xMin - diffSize.x / 2f, rect.yMin - diffSize.y),
                Alignment.TopRight => new Vector2(rect.xMin - diffSize.x, rect.yMin - diffSize.y),
                Alignment.BottomLeft => rect.position,
                Alignment.BottomCenter => new Vector2(rect.xMin - diffSize.x / 2f, rect.yMin),
                Alignment.BottomRight => new Vector2(rect.xMin - diffSize.x, rect.yMin),
                Alignment.CenterLeft => new Vector2(rect.xMin, rect.yMin - diffSize.y / 2f),
                Alignment.Center => new Vector2(rect.xMin - diffSize.x / 2f, rect.yMin - diffSize.y / 2f),
                Alignment.CenterRight => new Vector2(rect.xMin - diffSize.x, rect.yMin - diffSize.y / 2f),
                _ => throw new ArgumentOutOfRangeException(nameof(alignment), alignment, null)
            };
            
            return new Rect(position, size);
        }
        
        public static Rect WithWidth(this Rect rect, float width) 
            => new(rect.position, new Vector2(width, rect.height));
        
        public static Rect WithWidth(this Rect rect, float width, Alignment alignment) 
            => rect.WithSize(new  Vector2(width, rect.height), alignment);
    }
}