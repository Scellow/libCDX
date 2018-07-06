using OpenTK;

namespace CDX.Utils
{
    public static class Matrix4Exts
    {
        // todo: use properties instead of rows
        public static bool hasRotationOrScaling(this Matrix4 matrix)
        {
            return
                !(
                    // scaling
                    MathHelper.isEqual(matrix.Row0.X, 1)
                    && MathHelper.isEqual(matrix.Row1.Y, 1)
                    && MathHelper.isEqual(matrix.Row2.Z, 1)
                    // rotation
                    && MathHelper.isZero(matrix.Row0.Y)
                    && MathHelper.isZero(matrix.Row0.Z)
                    && MathHelper.isZero(matrix.Row1.X)
                    && MathHelper.isZero(matrix.Row1.Z)
                    && MathHelper.isZero(matrix.Row2.X)
                    && MathHelper.isZero(matrix.Row2.Y)
                );
        }

        // todo: double check
        public static float det3x3(this Matrix4 matrix)
        {
            return
                matrix.M11
                * matrix.M22
                * matrix.M33
                + matrix.M12
                * matrix.M23
                * matrix.M31
                + matrix.M13
                * matrix.M21
                * matrix.M32
                - matrix.M11
                * matrix.M23
                * matrix.M32
                - matrix.M12
                * matrix.M21
                * matrix.M33
                - matrix.M13
                * matrix.M22
                * matrix.M31;
        }
    }
}