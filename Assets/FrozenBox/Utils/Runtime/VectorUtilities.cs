using System;
using Unity.Mathematics;
using UnityEngine;

namespace FrozenBox.Utils
{
    [Serializable]
    public enum Axis
    {
        /// <summary>Vertical == X Axis</summary>
        AxisX = 0,
        /// <summary>Vertical == Y Axis</summary>
        AxisY = 1,
        /// <summary>Vertical == Z Axis</summary>
        AxisZ = 2,
        AxisMinusX,
        AxisMinusY,
        AxisMinusZ,
        /// <summary>Vertical == X Axis</summary>
        Horizontal = AxisX,
        /// <summary>Vertical == Y Axis</summary>
        Vertical= AxisY,
        /// <summary>Vertical == Z Axis</summary>
        Forward = AxisZ
    }
    
    public static class VectorUtilities
    {
        public static bool IsUnitScaled(this Vector3 vector3)
        {
            return Mathf.Approximately(vector3.x, 1f) 
                   && Mathf.Approximately(vector3.y, 1f) 
                   && Mathf.Approximately(vector3.z, 1f);
        }
        
        public static bool IsUnitScaled(this Vector2 vector2)
        {
            return Mathf.Approximately(vector2.x, 1f)
                   && Mathf.Approximately(vector2.y, 1f);
        }
        
        public static Vector2Int ToVector2Int(Tuple<int, int>tuple)
        {
            return new Vector2Int(tuple.Item1, tuple.Item2);
        }

        public static Vector3 ToXZ(this Vector2 vector2, float y = 0) => new(vector2.x, y, vector2.y);
        public static Vector2 FromXZ(this Vector3 vector3) => new(vector3.x, vector3.z);

        public static Vector3 WithReplaceX(this Vector3 vector3, float x) => new(x, vector3.y, vector3.z);
        public static Vector3 WithReplaceY(this Vector3 vector3, float y) => new(vector3.x, y, vector3.z);
        public static Vector3 WithReplaceZ(this Vector3 vector3, float z) => new(vector3.x,vector3. y, z);
        
        public static Vector2Int WithReplaceX(this Vector2Int vector2, int x) => new(x, vector2.y);
        public static Vector2Int WithReplaceY(this Vector2Int vector2, int y) => new(vector2.x, y);

        public static Vector3 ReplaceX(this Vector3 vector3, float x)
        {
            vector3.x = x;
            return vector3;
        }
        
        public static Vector3 ReplaceY(this Vector3 vector3, float y)
        {
            vector3.y = y;
            return vector3;
        }
        
        public static Vector3 ReplaceZ(this Vector3 vector3, float z)
        {
            vector3.z = z;
            return vector3;
        }
        
        public static Vector3 ToX(this float x) 
            => new(x, 0, 0);

        public static Vector3 ToY(this float y) 
            => new(0, y, 0);

        public static Vector3 ToZ(this float z) 
            => new(0, 0, z);

        public static Vector3 ToXYZ(this float value) 
            => new(value, value, value);

        public static Vector3 ProjectOnPlaneAndScale(Vector3 vector, Vector3 planeNormal)
        {
            return Vector3.ProjectOnPlane(vector, planeNormal).normalized * vector.magnitude;
        }

        public static Vector3 ProjectOnPlaneWithUp(Vector3 vector, Vector3 planeNormal, Vector3 upNormal)
        {
            var cross = Vector3.Cross(vector.normalized, upNormal).normalized;
            var project = Vector3.ProjectOnPlane(planeNormal, cross);
            return Vector3.ProjectOnPlane(vector, project);
        }
        
        public static Vector3 ProjectOnPlaneAndScaleWithUp(Vector3 vector, Vector3 planeNormal, Vector3 upNormal)
        {
            return ProjectOnPlaneWithUp(vector, planeNormal, upNormal).normalized * vector.magnitude;
        }

        public static Vector3 ToVector(this Axis axis)
        {
            return axis switch
            {
                Axis.AxisY => Vector3.up,
                Axis.AxisX => Vector3.right,
                Axis.AxisZ => Vector3.forward,
                Axis.AxisMinusY => Vector3.down,
                Axis.AxisMinusX => Vector3.left,
                Axis.AxisMinusZ => Vector3.back,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
        
        public static Vector3 ToVector(this Axis axis, Transform origin)
        {
            return axis switch
            {
                Axis.AxisY => origin.up,
                Axis.AxisX => origin.right,
                Axis.AxisZ => origin.forward,
                Axis.AxisMinusY => -origin.up,
                Axis.AxisMinusX => -origin.right,
                Axis.AxisMinusZ => -origin.forward,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
        
        public static float3 ProjectOnLine(float3 start, float3 end, float3 point)
        {
            return math.projectsafe(point - start, end - start);
        }
        
        public static float DistanceOnLine(float3 start, float3 end, float3 point)
        {
            return math.length(ProjectOnLine(start, end, point));
        }
        
        public static float DistanceToLine(float3 start, float3 end, float3 point)
        {
            var project = math.projectsafe(point - start, end - start);
            return math.distance(point, project);
        }

        public static float DeltaOnLine(float3 start, float3 end, float3 point)
        {
            var length = math.length(end - start);
            
            if (length <= math.EPSILON)
                return 0f;

            return DistanceOnLine(start, end, point) / length;
        }
        
        /*
        public static float3 Slerp(float3 start, float3 end, float percent)
        {
            // Dot product - the cosine of the angle between 2 vectors.
            float dot = math.dot(start, end);

            // Clamp it to be in the range of Acos()
            // This may be unnecessary, but floating point
            // precision can be a fickle mistress.
            dot =  math.clamp(dot, -1.0f, 1.0f);

            // Acos(dot) returns the angle between start and end,
            // And multiplying that by percent returns the angle between
            // start and the final result.
            float theta = math.acos(dot) * percent;
            float3 relativeVec = math.normalizesafe(end - start * dot);

            // Orthonormal basis
            // The final result.
            return start * math.cos(theta) + relativeVec * math.sin(theta);
        }
        
        public static float2 Slerp(float2 start, float2 end, float percent)
        {
            // Dot product - the cosine of the angle between 2 vectors.
            float dot = math.dot(start, end);

            // Clamp it to be in the range of Acos()
            // This may be unnecessary, but floating point
            // precision can be a fickle mistress.
            dot =  math.clamp(dot, -1.0f, 1.0f);

            // Acos(dot) returns the angle between start and end,
            // And multiplying that by percent returns the angle between
            // start and the final result.
            float theta = math.acos(dot) * percent;
            float2 relativeVec = math.normalizesafe(end - start * dot);

            // Orthonormal basis
            // The final result.
            return start * math.cos(theta) + relativeVec * math.sin(theta);
        }

        public static float3 ProjectOnLine(float3 start, float3 end, float3 point)
        {
            return math.projectsafe(point - start, end - start);
        }
        
        public static float DistanceOnLine(float3 start, float3 end, float3 point)
        {
            return math.length(ProjectOnLine(start, end, point));
        }
        
        public static float DistanceToLine(float3 start, float3 end, float3 point)
        {
            var project = math.projectsafe(point - start, end - start);
            return math.distance(point, project);
        }

        public static float DeltaOnLine(float3 start, float3 end, float3 point)
        {
            var length = math.length(end - start);
            
            if (length <= math.EPSILON)
                return 0f;

            return DistanceOnLine(start, end, point) / length;
        }*/
        
    }
}