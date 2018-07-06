using System;
using System.Runtime.InteropServices;

namespace CDX.Utils
{
    public static class JavaUtils
    {
        // todo: test this
        internal static int numberOfTrailingZeros(int i)
        {
            if (i == 0)
                return 32;

            var x = (i & -i) - 1;
            x -= ((x >> 1) & 0x55555555);
            x =  (((x >> 2) & 0x33333333) + (x & 0x33333333));
            x =  (((x >> 4) + x) & 0x0f0f0f0f);
            x += (x >> 8);
            x += (x >> 16);
            return (x & 0x0000003f);
        }
        
        // todo: test this
        internal static int numberOfTrailingZeros(long i)
        {
            int x, y;
            if (i == 0) return 64;
            int n                                                = 63;
            y = (int)i; if (y != 0) { n = n -32; x = y; } else x = (int)((uint) i>>32);
            y = x <<16; if (y != 0) { n = n -16; x = y; }
            y = x << 8; if (y != 0) { n = n - 8; x = y; }
            y = x << 4; if (y != 0) { n = n - 4; x = y; }
            y = x << 2; if (y != 0) { n = n - 2; x = y; }
            return (int) (n - ((uint) (x << 1) >> 31));
        }

        // todo: test this
        public static float intToFloatColor(int color)
        {
            byte[] bytes = BitConverter.GetBytes(color);
            float  f     = BitConverter.ToSingle(bytes, 0);
            return f;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct FloatToIntConverter
        {
            [FieldOffset(0)]
            public int IntValue;
            [FieldOffset(0)]
            public float FloatValue;
        }

        
        // todo: test this
        public static int floatToRawIntBits(float value)
        {
            var converter = new FloatToIntConverter();
            converter.FloatValue = value;
            return  converter.IntValue;            
        }
    }
}