using OpenTK.Graphics.OpenGL4;

namespace CDX.Graphics.G3D
{
    public class IntAttribute : Attribute
    {
        public static readonly string CullFaceAlias = "cullface";
        public static readonly long   CullFace      = register(CullFaceAlias);

        public static IntAttribute createCullFace(CullFaceMode value)
        {
            return new IntAttribute(CullFace, value);
        }

        public CullFaceMode value;

        public IntAttribute(long type) : base(type)
        {
        }

        public IntAttribute(long type, CullFaceMode value) : base(type)
        {
            this.value = value;
        }


        public override Attribute copy()
        {
            return new IntAttribute(type, value);
        }

        public override int GetHashCode()
        {
            int result = base.GetHashCode();
            result = 983 * result + (int) value;
            return result; 
        }

        public override int CompareTo(Attribute o)
        {
            if (type != o.type) return (int)(type - o.type);
            return value - ((IntAttribute)o).value;
        }
    }
}