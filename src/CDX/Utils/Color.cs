using CDX.Utils;

namespace CDX.Graphics
{
    public struct Color
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
    }
}