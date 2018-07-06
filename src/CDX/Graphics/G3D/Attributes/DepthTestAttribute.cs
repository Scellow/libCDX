using System;
using CDX.Utils;
using OpenTK.Graphics.OpenGL4;

namespace CDX.Graphics.G3D
{
    public class DepthTestAttribute : Attribute
    {
        public readonly static string Alias = "depthStencil";
        public readonly static long   Type  = register(Alias);

        protected static long Mask = Type;

        public static bool @is(long mask)
        {
            return (mask & Mask) != 0;
        }

        /** The depth test function, or 0 to disable depth test (default: GL10.GL_LEQUAL) */
        public DepthFunction depthFunc;

        /** Mapping of near clipping plane to window coordinates (default: 0) */
        public float depthRangeNear;

        /** Mapping of far clipping plane to window coordinates (default: 1) */
        public float depthRangeFar;

        /** Whether to write to the depth buffer (default: true) */
        public bool depthMask;

        public DepthTestAttribute(DepthTestAttribute mask) : this(mask.type, mask.depthFunc, mask.depthRangeNear, mask.depthRangeFar, mask.depthMask)
        {
        }

        public DepthTestAttribute(bool depthMask) : this(DepthFunction.Lequal, depthMask)
        {
        }

        public DepthTestAttribute(DepthFunction depthFunc, bool depthMask = true) : this(depthFunc, 0, 1, depthMask)
        {
        }

        public DepthTestAttribute(DepthFunction depthFunc, float depthRangeNear, float depthRangeFar, bool depthMask = true) : this(Type, depthFunc, depthRangeNear, depthRangeFar, depthMask)
        {
        }

        public DepthTestAttribute(long type, DepthFunction depthFunc, float depthRangeNear, float depthRangeFar, bool depthMask) : base(type)
        {
            if (!@is(type)) throw new Exception("Invalid type specified");
            this.depthFunc      = depthFunc;
            this.depthRangeNear = depthRangeNear;
            this.depthRangeFar  = depthRangeFar;
            this.depthMask      = depthMask;
        }

        public DepthTestAttribute(long type) : base(type)
        {
            
        }

        public override Attribute copy()
        {
            return new DepthTestAttribute(this);
        }

        public override int GetHashCode()
        {
            int result = base.GetHashCode();
            result = 971 * result +(int) depthFunc;
            result = 971 * result + JavaUtils.floatToRawIntBits(depthRangeNear);
            result = 971 * result + JavaUtils.floatToRawIntBits(depthRangeFar);
            result = 971 * result + (depthMask ? 1 : 0);
            return result;
        }

        public override int CompareTo(Attribute o)
        {
            if (type != o.type) return (int) (type - o.type);
            DepthTestAttribute other = (DepthTestAttribute) o;
            if (depthFunc != other.depthFunc) return depthFunc - other.depthFunc;
            if (depthMask != other.depthMask) return depthMask ? -1 : 1;
            if (!MathHelper.isEqual(depthRangeNear, other.depthRangeNear))
                return depthRangeNear < other.depthRangeNear ? -1 : 1;
            if (!MathHelper.isEqual(depthRangeFar, other.depthRangeFar))
                return depthRangeFar < other.depthRangeFar ? -1 : 1;
            return 0;
        }
    }
}