using System;

namespace CDX.Graphics
{
    public interface TextureBinder
    {
        void begin();

        void end();

        int bind(TextureDescriptor BasetextureDescriptor);

        int bind(GLTexture texture);

        int getBindCount();

        int getReuseCount();

        void resetCounts();
    }

    /*
    public class TextureDescriptor : TextureDescriptor<Texture>
    {
        public TextureDescriptor(Texture texture, TextureFilter minFilter, TextureFilter magFilter, TextureWrap uWrap, TextureWrap vWrap) : base(texture, minFilter, magFilter, uWrap, vWrap)
        {
        }

        public TextureDescriptor(Texture texture) : base(texture)
        {
        }
    }
    
    public class BaseTextureDescriptor : TextureDescriptor<GLTexture>
    {
        public BaseTextureDescriptor()
        {}
            
    }
    */
/*
    public class TextureDescriptor_ : TextureDescriptor<GLTexture>
    {}
    */
    public class TextureDescriptor : IComparable<TextureDescriptor>
    {
        public GLTexture texture = null;
        public TextureFilter minFilter;
        public TextureFilter magFilter;
        public TextureWrap   uWrap;
        public TextureWrap   vWrap;
        
        public TextureDescriptor()
        {}

        public TextureDescriptor(GLTexture texture, TextureFilter minFilter, TextureFilter magFilter,
            TextureWrap uWrap, TextureWrap vWrap)
        {
            set(texture, minFilter, magFilter, uWrap, vWrap);
        }

        public TextureDescriptor(GLTexture texture) : this(texture, texture.getMinFilter(), texture.getMagFilter(), texture.getUWrap(), texture.getVWrap())
        {
        }


        public void set(GLTexture texture, TextureFilter minFilter = TextureFilter.Linear, TextureFilter magFilter = TextureFilter.Linear, TextureWrap uWrap = TextureWrap.ClampToEdge, TextureWrap vWrap = TextureWrap.ClampToEdge)
        {
            this.texture   = texture;
            this.minFilter = minFilter;
            this.magFilter = magFilter;
            this.uWrap     = uWrap;
            this.vWrap     = vWrap;
        }


        public void set(TextureDescriptor other)
        {
            texture   = other.texture;
            minFilter = other.minFilter;
            magFilter = other.magFilter;
            uWrap     = other.uWrap;
            vWrap     = other.vWrap;
        }


        public override bool Equals(object obj)
        {
            
            if (obj == null) return false;
            if (obj == this) return true;
            if (!(obj is TextureDescriptor)) return false;
            TextureDescriptor other = (TextureDescriptor)obj;
            return other.texture == texture && other.minFilter == minFilter && other.magFilter == magFilter && other.uWrap == uWrap
                   && other.vWrap == vWrap;
        }
        
        // todo: gethashcode
        
        public int CompareTo(TextureDescriptor o)
        {
            if (o == this) return 0;
            var t1 = texture == null ? 0 : texture.glTarget;
            var t2 = o.texture == null ? 0 : o.texture.glTarget;
            if (t1 != t2) return t1 - t2;
            int h1 = texture == null ? 0 : texture.getTextureObjectHandle();
            int h2 = o.texture == null ? 0 : o.texture.getTextureObjectHandle();
            if (h1 != h2) return h1 - h2;
            if (minFilter != o.minFilter)
                return (minFilter == null ? 0 : TextureHelper.getGLEnumFromTextureFilter(minFilter)) - (o.minFilter == null ? 0 :TextureHelper.getGLEnumFromTextureFilter(o.minFilter));
            if (magFilter != o.magFilter)
                return (magFilter == null ? 0 : TextureHelper.getGLEnumFromTextureFilter(magFilter)) - (o.magFilter == null ? 0 : TextureHelper.getGLEnumFromTextureFilter(o.magFilter));
            if (uWrap != o.uWrap) return (uWrap == null ? 0 : TextureHelper.getGLEnumFromTextureWrap(uWrap)) - (o.uWrap == null ? 0 : TextureHelper.getGLEnumFromTextureWrap(o.uWrap));
            if (vWrap != o.vWrap) return (vWrap == null ? 0 : TextureHelper.getGLEnumFromTextureWrap(uWrap)) - (o.vWrap == null ? 0 : TextureHelper.getGLEnumFromTextureWrap(o.vWrap));
            return 0;
        }
    }
}