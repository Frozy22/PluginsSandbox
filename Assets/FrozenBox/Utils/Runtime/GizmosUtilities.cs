using UnityEngine;

namespace FrozenBox.Utils
{
    public static class GizmosUtilities
    {
        private static Color _tempColor;

        public static void BeginDraw()
        {
            _tempColor = Gizmos.color;
        }
        
        public static void BeginDraw(Color color)
        {
            BeginDraw();
            Gizmos.color = color;
        }
        
        public static void EndDraw()
        {
            Gizmos.color = _tempColor;
        }

        public static void DrawCapsuleWire(Vector3 topSphereCenter, Vector3 bottomSphereCenter, float radius)
        {
            Gizmos.DrawWireSphere(topSphereCenter, radius);
            Gizmos.DrawWireSphere(bottomSphereCenter, radius);
            
            Vector3 dir = topSphereCenter-bottomSphereCenter;
            Vector3 left = Vector3.Cross(dir.normalized, Vector3.right).normalized * radius;
            Vector3 forward = Vector3.Cross(dir.normalized, left.normalized).normalized * radius;
            Gizmos.DrawLine(topSphereCenter + left, bottomSphereCenter + left);
            Gizmos.DrawLine(topSphereCenter - left, bottomSphereCenter - left);
            Gizmos.DrawLine(topSphereCenter + forward, bottomSphereCenter + forward);
            Gizmos.DrawLine(topSphereCenter - forward, bottomSphereCenter - forward);
        }
    }
}