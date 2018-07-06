namespace CDX.Graphics.G3D
{
    public class ColorAttribute : Attribute
    {
        public static readonly string DiffuseAlias      = "diffuseColor";
        public static readonly long   Diffuse           = register(DiffuseAlias);
        public static readonly string SpecularAlias     = "specularColor";
        public static readonly long   Specular          = register(SpecularAlias);
        public static readonly string AmbientAlias      = "ambientColor";
        public static readonly long   Ambient           = register(AmbientAlias);
        public static readonly string EmissiveAlias     = "emissiveColor";
        public static readonly long   Emissive          = register(EmissiveAlias);
        public static readonly string ReflectionAlias   = "reflectionColor";
        public static readonly long   Reflection        = register(ReflectionAlias);
        public static readonly string AmbientLightAlias = "ambientLightColor";
        public static readonly long   AmbientLight      = register(AmbientLightAlias);
        public static readonly string FogAlias          = "fogColor";
        public static readonly long   Fog               = register(FogAlias);

        protected static long Mask = Ambient | Diffuse | Specular | Emissive | Reflection | AmbientLight | Fog;

        public static bool @is(long mask)
        {
            return (mask & Mask) != 0;
        }

        public static ColorAttribute createAmbient(Color color)
        {
            return new ColorAttribute(Ambient, color);
        }

        public static ColorAttribute createAmbient(float r, float g, float b, float a)
        {
            return new ColorAttribute(Ambient, r, g, b, a);
        }

        public static ColorAttribute createDiffuse(Color color)
        {
            return new ColorAttribute(Diffuse, color);
        }

        public static ColorAttribute createDiffuse(float r, float g, float b, float a)
        {
            return new ColorAttribute(Diffuse, r, g, b, a);
        }

        public static ColorAttribute createSpecular(Color color)
        {
            return new ColorAttribute(Specular, color);
        }

        public static ColorAttribute createSpecular(float r, float g, float b, float a)
        {
            return new ColorAttribute(Specular, r, g, b, a);
        }

        public static ColorAttribute createReflection(Color color)
        {
            return new ColorAttribute(Reflection, color);
        }

        public static ColorAttribute createReflection(float r, float g, float b, float a)
        {
            return new ColorAttribute(Reflection, r, g, b, a);
        }

        public readonly Color color;


        public ColorAttribute(long type) : base(type)
        {
        }

        public ColorAttribute(long type, Color color) : this(type)
        {
            if (color != null) this.color = color;
        }

        public ColorAttribute(long type, float r, float g, float b, float a) : this(type)
        {
            this.color = new Color(r, g, b, a);
        }

        public ColorAttribute(ColorAttribute copyFrom) : this(copyFrom.type, copyFrom.color)
        {
        }


        public override Attribute copy()
        {
            return new ColorAttribute(this);
        }

        public override int GetHashCode()
        {
            int result = base.GetHashCode();
            result = 953 * result + color.toIntBits();
            return result; 
        }

        public override int CompareTo(Attribute o)
        {
            if (type != o.type) return (int)(type - o.type);
            return ((ColorAttribute)o).color.toIntBits() - color.toIntBits();
        }
    }
}