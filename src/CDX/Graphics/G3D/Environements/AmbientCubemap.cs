using System;
using CDX.Utils;
using OpenTK;

namespace CDX.Graphics.G3D.Environements
{
    public class AmbientCubemap
    {
        private static readonly int NUM_VALUES = 6 * 3;

        private static float clamp(float v)
        {
            return v < 0f ? 0f : (v > 1f ? 1f : v);
        }

        public readonly float[] data;

        public AmbientCubemap()
        {
            data = new float[NUM_VALUES];
        }

        public AmbientCubemap(float[] copyFrom)
        {
            if (copyFrom.Length != (NUM_VALUES)) throw new Exception("Incorrect array size");
            data = new float[copyFrom.Length];
            Array.Copy(copyFrom, 0, data, 0, data.Length);
        }

        public AmbientCubemap(AmbientCubemap copyFrom) : this(copyFrom.data)
        {
        }

        public AmbientCubemap set(float[] values)
        {
            for (int i = 0; i < data.Length; i++)
                data[i] = values[i];
            return this;
        }

        public AmbientCubemap set(AmbientCubemap other)
        {
            return set(other.data);
        }

        public AmbientCubemap set(Color color)
        {
            return set(color.r, color.g, color.b);
        }

        public AmbientCubemap set(float r, float g, float b)
        {
            for (int idx = 0; idx < NUM_VALUES;)
            {
                data[idx]     =  r;
                data[idx + 1] =  g;
                data[idx + 2] =  b;
                idx           += 3;
            }

            return this;
        }

        public Color getColor(int side)
        {
            side *= 3;
            return new Color(data[side], data[side + 1], data[side + 2], 1f);
        }

        public AmbientCubemap clear()
        {
            for (int i = 0; i < data.Length; i++)
                data[i] = 0f;
            return this;
        }

        public AmbientCubemap clamp()
        {
            for (int i = 0; i < data.Length; i++)
                data[i] = clamp(data[i]);
            return this;
        }

        public AmbientCubemap add(float r, float g, float b)
        {
            for (int idx = 0; idx < data.Length;)
            {
                data[idx++] += r;
                data[idx++] += g;
                data[idx++] += b;
            }

            return this;
        }

        public AmbientCubemap add(Color color)
        {
            return add(color.r, color.g, color.b);
        }

        public AmbientCubemap add(float r, float g, float b, float x, float y, float z)
        {
            float x2 = x * x, y2 = y * y, z2 = z * z;
            float d  = x2 + y2 + z2;
            if (d == 0f) return this;
            d = 1f / d * (d + 1f);
            float rd  = r * d, gd = g * d, bd = b * d;
            int   idx = x > 0 ? 0 : 3;
            data[idx]     += x2 * rd;
            data[idx + 1] += x2 * gd;
            data[idx + 2] += x2 * bd;
            idx           =  y > 0 ? 6 : 9;
            data[idx]     += y2 * rd;
            data[idx + 1] += y2 * gd;
            data[idx + 2] += y2 * bd;
            idx           =  z > 0 ? 12 : 15;
            data[idx]     += z2 * rd;
            data[idx + 1] += z2 * gd;
            data[idx + 2] += z2 * bd;
            return this;
        }

        public AmbientCubemap add(Color color, Vector3 direction)
        {
            return add(color.r, color.g, color.b, direction.X, direction.Y, direction.Z);
        }

        public AmbientCubemap add(float r, float g, float b, Vector3 direction)
        {
            return add(r, g, b, direction.X, direction.Y, direction.Z);
        }

        public AmbientCubemap add(Color color, float x, float y, float z)
        {
            return add(color.r, color.g, color.b, x, y, z);
        }

        public AmbientCubemap add(Color color, Vector3 point, Vector3 target)
        {
            return add(color.r, color.g, color.b, target.X - point.X, target.Y - point.Y, target.Z - point.Z);
        }

        public AmbientCubemap add(Color color, Vector3 point, Vector3 target, float intensity)
        {
            float t = intensity / (1f + target.dst(point));
            return add(color.r * t, color.g * t, color.b * t, target.X - point.X, target.Y - point.Y, target.Z - point.Z);
        }

        public override string ToString()
        {
            String result = "";
            for (int i = 0; i < data.Length; i += 3)
            {
                result += Convert.ToString(data[i]) + ", " + Convert.ToString(data[i + 1]) + ", " + Convert.ToString(data[i + 2]) + "\n";
            }

            return result;
        }
    }
}