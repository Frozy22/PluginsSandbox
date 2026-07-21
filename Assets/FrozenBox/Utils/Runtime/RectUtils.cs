using System;
using UnityEngine;

namespace FrozenBox.Utils
{
    public static class RectUtils
    {
        public static Rect CreateRect(Vector2 position, Vector2 size, Alignment alignment)
        {
            var resultPosition = alignment switch
            {
                Alignment.TopLeft => new Vector2(position.x, position.y - size.y),
                Alignment.TopCenter => new Vector2(position.x - size.x / 2f, position.y - size.y),
                Alignment.TopRight => new Vector2(position.x - size.x, position.y - size.y),
                Alignment.BottomLeft => position,
                Alignment.BottomCenter => new Vector2(position.x - size.x / 2f, position.y),
                Alignment.BottomRight => new Vector2(position.x - size.x, position.y),
                Alignment.CenterLeft => new Vector2(position.x, position.y - size.y / 2f),
                Alignment.Center => new Vector2(position.x - size.x / 2f, position.y - size.y / 2f),
                Alignment.CenterRight => new Vector2(position.x - size.x, position.y - size.y / 2f),
                _ => throw new ArgumentOutOfRangeException(nameof(alignment), alignment, null)
            };
            
            return new Rect(resultPosition, size);
        }
    }
}