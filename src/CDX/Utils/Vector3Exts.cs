using System;
using OpenTK;

namespace CDX.Utils
{
    public static class Vector3Exts
    {
        public static float dst2(this Vector3 vector, Vector3 point)
        {
            float a = point.X - vector.X;
            float b = point.Y - vector.Y;
            float c = point.Z - vector.Z;
            return a * a + b * b + c * c;
        }
        public static float dst(this Vector3 vector, Vector3 point)
        {
            float a = point.X - vector.X;
            float b = point.Y - vector.Y;
            float c = point.Z - vector.Z;
            return (float)Math.Sqrt(a * a + b * b + c * c);
        }
    }
}