using System;
using OpenTK;

namespace CDX.Utils
{
    public struct BoundingBox
    {
        public Vector3 min;
        public Vector3 max;

        private Vector3 cnt;
        private Vector3 dim;

        public BoundingBox inf()
        {
            min = new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
            max = new Vector3(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);
            cnt = new Vector3(0, 0, 0);
            dim = new Vector3(0, 0, 0);
            return this;
        }

        public BoundingBox ext(Vector3 point)
        {
            return set(
                min = new Vector3(min_(min.X, point.X), min_(min.Y, point.Y), min_(min.Z, point.Z)),
                max = new Vector3(Math.Max(max.X, point.X), Math.Max(max.Y, point.Y), Math.Max(max.Z, point.Z))
            );
        }

        public BoundingBox set(Vector3 minimum, Vector3 maximum)
        {
            min = new Vector3(minimum.X < maximum.X ? minimum.X : maximum.X, minimum.Y < maximum.Y ? minimum.Y : maximum.Y,
                minimum.Z < maximum.Z ? minimum.Z : maximum.Z);
            max = new Vector3(minimum.X > maximum.X ? minimum.X : maximum.X, minimum.Y > maximum.Y ? minimum.Y : maximum.Y,
                minimum.Z > maximum.Z ? minimum.Z : maximum.Z);
            cnt = (min + max) * 0.5f;
            dim = max - min;
            return this;
        }

        static float min_(float a, float b)
        {
            return a > b ? b : a;
        }

        public Vector3 getCenter()
        {
            return cnt;
        }
        public Vector3 getDimension()
        {
            return dim;
        }
    }
}