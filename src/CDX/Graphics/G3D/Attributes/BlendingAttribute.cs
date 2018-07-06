using CDX.Utils;
using OpenTK.Graphics.OpenGL4;

namespace CDX.Graphics.G3D
{
    public class BlendingAttribute : Attribute
    {
        public static readonly string Alias = "blended";
        public static readonly long   Type  = register(Alias);

        public static bool @is(long mask)
        {
            return (mask & Type) == mask;
        }

        public bool               blended;
        public BlendingFactor sourceFunction;
        public BlendingFactor destFunction;
        public float              opacity = 1.0f;


        public BlendingAttribute(BlendingAttribute copyFrom = null) :
            this(
                copyFrom?.blended ?? true,
                copyFrom?.sourceFunction ?? BlendingFactor.SrcAlpha,
                copyFrom?.destFunction ?? BlendingFactor.OneMinusSrcAlpha,
                copyFrom?.opacity ?? 1.0f
            )
        {
        }

        public BlendingAttribute(bool blended, BlendingFactor sourceFunc, BlendingFactor destFunc, float opacity) : base(Type)
        {
            this.blended        = blended;
            this.sourceFunction = sourceFunc;
            this.destFunction   = destFunc;
            this.opacity        = opacity;
        }

        public override Attribute copy()
        {
            return new BlendingAttribute(this);
        }

        public override int GetHashCode()
        {
            int result = base.GetHashCode();
            result = 947 * result + (blended ? 1 : 0);
            result = 947 * result + (int)sourceFunction;
            result = 947 * result + (int)destFunction;
            result = 947 * result + JavaUtils.floatToRawIntBits(opacity);
            return result;
        }

        public override int CompareTo(Attribute o)
        {
            if (type != o.type) return (int)(type - o.type);
            BlendingAttribute other = (BlendingAttribute)o;
            if (blended != other.blended) return blended ? 1 : -1;
            if (sourceFunction != other.sourceFunction) return sourceFunction - other.sourceFunction;
            if (destFunction != other.destFunction) return destFunction - other.destFunction;
            return (MathHelper.isEqual(opacity, other.opacity)) ? 0 : (opacity < other.opacity ? 1 : -1);
        }
    }
}