using System;
using CDX.Utils;

namespace CDX.Graphics
{
    public struct Color : IEquatable<Color>
    {
        public static readonly Color BLACK = new Color(0,0,0,1);
        public static readonly Color WHITE = new Color(1,1,1,1);
        public float r;
        public float g;
        public float b;
        public float a;
        
        public Color(float r, float g, float b, float a)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }
        
        public float toFloatBits () {
            int color = ((int)(255 * a) << 24) | ((int)(255 * b) << 16) | ((int)(255 * g) << 8) | ((int)(255 * r));
            
            
            return JavaUtils.intToFloatColor(color);
        }
        public int toIntBits () {
            int color = ((int)(255 * a) << 24) | ((int)(255 * b) << 16) | ((int)(255 * g) << 8) | ((int)(255 * r));
            return color;
        }

        public static bool operator ==(Color a, Color b)
        {
            return a.toIntBits() == b.toIntBits();
        }

        public static bool operator !=(Color a, Color b)
        {
            return !(a == b);
        }


        public bool Equals(Color other)
        {
            return r.Equals(other.r) && g.Equals(other.g) && b.Equals(other.b) && a.Equals(other.a);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Color && Equals((Color) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = r.GetHashCode();
                hashCode = (hashCode * 397) ^ g.GetHashCode();
                hashCode = (hashCode * 397) ^ b.GetHashCode();
                hashCode = (hashCode * 397) ^ a.GetHashCode();
                return hashCode;
            }
        }
        
    }
}