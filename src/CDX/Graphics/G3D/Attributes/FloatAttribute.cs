using CDX.Utils;

namespace CDX.Graphics.G3D
{
    public class FloatAttribute : Attribute
    {
        public static readonly string ShininessAlias = "shininess";
        public static readonly long   Shininess      = register(ShininessAlias);

        public static FloatAttribute createShininess(float value)
        {
            return new FloatAttribute(Shininess, value);
        }

        public static readonly string AlphaTestAlias = "alphaTest";
        public static readonly long   AlphaTest      = register(AlphaTestAlias);

        public static FloatAttribute createAlphaTest(float value)
        {
            return new FloatAttribute(AlphaTest, value);
        }

        public float value;

        public FloatAttribute(long type, float value) : base(type)
        {
            this.value = value;
        }

        public FloatAttribute(long type) : base(type)
        {
        }

        public override Attribute copy()
        {
            return new FloatAttribute(type, value);
        }

        public override int GetHashCode()
        {
            int result = base.GetHashCode();
            result = 977 * result + JavaUtils.floatToRawIntBits(value);
            return result;
        }

        public override int CompareTo(Attribute o)
        {
            if (type != o.type) return (int) (type - o.type);
            float v = ((FloatAttribute) o).value;
            return MathHelper.isEqual(value, v) ? 0 : value < v ? -1 : 1;
        }
    }
}