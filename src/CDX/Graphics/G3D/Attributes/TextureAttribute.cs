using System;
using CDX.Utils;

namespace CDX.Graphics.G3D
{
    public class TextureAttribute : Attribute
    {
        public static readonly string DiffuseAlias    = "diffuseTexture";
        public static readonly long   Diffuse         = register(DiffuseAlias);
        public static readonly string SpecularAlias   = "specularTexture";
        public static readonly long   Specular        = register(SpecularAlias);
        public static readonly string BumpAlias       = "bumpTexture";
        public static readonly long   Bump            = register(BumpAlias);
        public static readonly string NormalAlias     = "normalTexture";
        public static readonly long   Normal          = register(NormalAlias);
        public static readonly string AmbientAlias    = "ambientTexture";
        public static readonly long   Ambient         = register(AmbientAlias);
        public static readonly string EmissiveAlias   = "emissiveTexture";
        public static readonly long   Emissive        = register(EmissiveAlias);
        public static readonly string ReflectionAlias = "reflectionTexture";
        public static readonly long   Reflection      = register(ReflectionAlias);

        protected static long Mask = Diffuse | Specular | Bump | Normal | Ambient | Emissive | Reflection;

        public static bool @is(long mask)
        {
            return (mask & Mask) != 0;
        }

        public static TextureAttribute createDiffuse(Texture texture)
        {
            return new TextureAttribute(Diffuse, texture);
        }

        public static TextureAttribute createDiffuse(TextureRegion region)
        {
            return new TextureAttribute(Diffuse, region);
        }

        public static TextureAttribute createSpecular(Texture texture)
        {
            return new TextureAttribute(Specular, texture);
        }

        public static TextureAttribute createSpecular(TextureRegion region)
        {
            return new TextureAttribute(Specular, region);
        }

        public static TextureAttribute createNormal(Texture texture)
        {
            return new TextureAttribute(Normal, texture);
        }

        public static TextureAttribute createNormal(TextureRegion region)
        {
            return new TextureAttribute(Normal, region);
        }

        public static TextureAttribute createBump(Texture texture)
        {
            return new TextureAttribute(Bump, texture);
        }

        public static TextureAttribute createBump(TextureRegion region)
        {
            return new TextureAttribute(Bump, region);
        }

        public static TextureAttribute createAmbient(Texture texture)
        {
            return new TextureAttribute(Ambient, texture);
        }

        public static TextureAttribute createAmbient(TextureRegion region)
        {
            return new TextureAttribute(Ambient, region);
        }

        public static TextureAttribute createEmissive(Texture texture)
        {
            return new TextureAttribute(Emissive, texture);
        }

        public static TextureAttribute createEmissive(TextureRegion region)
        {
            return new TextureAttribute(Emissive, region);
        }

        public static TextureAttribute createReflection(Texture texture)
        {
            return new TextureAttribute(Reflection, texture);
        }

        public static TextureAttribute createReflection(TextureRegion region)
        {
            return new TextureAttribute(Reflection, region);
        }

        public readonly TextureDescriptor textureDescription;
        public          float                      offsetU = 0;
        public          float                      offsetV = 0;
        public          float                      scaleU  = 1;

        public float scaleV = 1;

        /** The index of the texture coordinate vertex attribute to use for this TextureAttribute. Whether this value is used, depends
         * on the shader and {@link Attribute#type} value. For basic (model specific) types (e.g. {@link #Diffuse}, {@link #Normal},
         * etc.), this value is usually ignored and the first texture coordinate vertex attribute is used. */
        public int uvIndex = 0;


        public TextureAttribute(long type, TextureDescriptor textureDescription) : this(type)
        {
            this.textureDescription.set(textureDescription);
        }

        public TextureAttribute(long type, TextureDescriptor textureDescription, float offsetU, float offsetV, float scaleU, float scaleV, int uvIndex = 0) : this(type, textureDescription)
        {
            this.offsetU = offsetU;
            this.offsetV = offsetV;
            this.scaleU  = scaleU;
            this.scaleV  = scaleV;
            this.uvIndex = uvIndex;
        }

        public TextureAttribute(long type) : base(type)
        {
            if (!@is(type)) throw new Exception("Invalid type specified");
            textureDescription = new TextureDescriptor();
        }

        public TextureAttribute(long type, Texture texture) : this(type)
        {
            textureDescription.texture = texture;
        }

        public TextureAttribute(long type, TextureRegion region) : this(type)
        {
            set(region);
        }

        public TextureAttribute(TextureAttribute copyFrom) : this(copyFrom.type, copyFrom.textureDescription, copyFrom.offsetU, copyFrom.offsetV, copyFrom.scaleU, copyFrom.scaleV, copyFrom.uvIndex)
        {
        }

        public void set(TextureRegion region)
        {
            textureDescription.texture = region.getTexture();
            offsetU                    = region.getU();
            offsetV                    = region.getV();
            scaleU                     = region.getU2() - offsetU;
            scaleV                     = region.getV2() - offsetV;
        }


        public override Attribute copy()
        {
            return new TextureAttribute(this);
        }

        public override int GetHashCode()
        {
            int result = base.GetHashCode();
            result = 991 * result + textureDescription.GetHashCode();
            result = 991 * result + JavaUtils.floatToRawIntBits(offsetU);
            result = 991 * result + JavaUtils.floatToRawIntBits(offsetV);
            result = 991 * result + JavaUtils.floatToRawIntBits(scaleU);
            result = 991 * result + JavaUtils.floatToRawIntBits(scaleV);
            result = 991 * result + uvIndex;
            return result;
        }

        public override int CompareTo(Attribute o)
        {
            if (type != o.type) return type < o.type ? -1 : 1;
            TextureAttribute other = (TextureAttribute)o;
            int        c     = textureDescription.CompareTo(other.textureDescription);
            if (c != 0) return c;
            if (uvIndex != other.uvIndex) return uvIndex - other.uvIndex;
            if (!MathHelper.isEqual(scaleU, other.scaleU)) return scaleU > other.scaleU ? 1 : -1;
            if (!MathHelper.isEqual(scaleV, other.scaleV)) return scaleV > other.scaleV ? 1 : -1;
            if (!MathHelper.isEqual(offsetU, other.offsetU)) return offsetU > other.offsetU ? 1 : -1;
            if (!MathHelper.isEqual(offsetV, other.offsetV)) return offsetV > other.offsetV ? 1 : -1;
            return 0;
        }
    }
}