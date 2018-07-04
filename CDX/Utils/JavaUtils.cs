using System;

namespace CDX.Utils
{
    public static class JavaUtils
    {
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

        public static float intToFloatColor(int color)
        {
            byte[] bytes = BitConverter.GetBytes(color);
            float  f     = BitConverter.ToSingle(bytes, 0);
            return f;
        }
    }
}