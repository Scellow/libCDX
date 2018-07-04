using CDX.Utils;
using OpenTK.Graphics.OpenGL4;

namespace CDX.Graphics
{
    public class VertexAttribute
    {
        public int                     usage;
        public int                     numComponents;
        public bool                    normalized;
        public VertexAttribPointerType type;
        public int                     offset;
        public string                  alias;

        public int unit;
        public int usageIndex;

        public VertexAttribute(int usage, int numComponents, string alias, int unit = 0) 
            : this(usage, numComponents, usage == VertexAttributes.Usage.ColorPacked ? VertexAttribPointerType.UnsignedByte : VertexAttribPointerType.Float,
                usage == VertexAttributes.Usage.ColorPacked, alias, unit)
        {
        }

        public VertexAttribute(int usage, int numComponents, VertexAttribPointerType type, bool normalized, string alias, int unit)
        {
            this.usage         = usage;
            this.numComponents = numComponents;
            this.type          = type;
            this.normalized    = normalized;
            this.alias         = alias;
            this.unit          = unit;
            this.usageIndex    = JavaUtils.numberOfTrailingZeros(usage);
        }


        public int getSizeInBytes()
        {
            switch (type)
            {
                case VertexAttribPointerType.Float:
                case VertexAttribPointerType.Fixed:
                    return 4 * numComponents;
                case VertexAttribPointerType.UnsignedByte:
                case VertexAttribPointerType.Byte:
                    return numComponents;
                case VertexAttribPointerType.UnsignedShort:
                case VertexAttribPointerType.Short:
                    return 2 * numComponents;
            }

            return 0;
        }
    }
}