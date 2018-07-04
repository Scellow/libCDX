using System;
using OpenTK.Graphics.OpenGL4;

namespace CDX.Graphics
{
    public enum TextureFilter
    {
        Nearest, // GL20.GL_NEAREST
        Linear,  // GL20.GL_LINEAR
        MipMap, // GL20.GL_LINEAR_MIPMAP_LINEAR
        MipMapNearestNearest,  // GL20.GL_NEAREST_MIPMAP_NEAREST
        MipMapLinearNearest, // GL20.GL_LINEAR_MIPMAP_NEAREST
        MipMapNearestLinear, // GL20.GL_NEAREST_MIPMAP_LINEAR
        MipMapLinearLinear, // GL20.GL_LINEAR_MIPMAP_LINEAR
    }

    public enum TextureWrap
    {
        MirroredRepeat, // GL20.GL_MIRRORED_REPEAT
        ClampToEdge, // GL20.GL_CLAMP_TO_EDGE
        Repeat, // GL20.GL_REPEAT
    }

    public static class TextureHelper
    {
        public static bool isMipMap(TextureFilter filter)
        {
            return filter != TextureFilter.Nearest && filter != TextureFilter.Linear;
        }

        public static int getGLEnumFromTextureFilter(TextureFilter filter)
        {
            switch (filter)
            {
                case TextureFilter.Nearest:
                    return (int) All.Nearest;
                case TextureFilter.Linear:
                    return (int) All.Linear;
                case TextureFilter.MipMap:
                    return (int) All.LinearMipmapLinear;
                case TextureFilter.MipMapNearestNearest:
                    return (int) All.NearestMipmapNearest;
                case TextureFilter.MipMapLinearNearest:
                    return (int) All.LinearMipmapNearest;
                case TextureFilter.MipMapNearestLinear:
                    return (int) All.NearestMipmapLinear;
                case TextureFilter.MipMapLinearLinear:
                    return (int) All.LinearMipmapLinear;
                default:
                    throw new ArgumentOutOfRangeException(nameof(filter), filter, null);
            }
        }

        public static int getGLEnumFromTextureWrap(TextureWrap wrap)
        {
            switch (wrap)
            {
                case TextureWrap.MirroredRepeat:
                    return (int) All.MirroredRepeat;
                case TextureWrap.ClampToEdge:
                    return (int) All.ClampToEdge;
                case TextureWrap.Repeat:
                    return (int) All.Repeat;
                default:
                    throw new ArgumentOutOfRangeException(nameof(wrap), wrap, null);
            }
        }
    }
    
    
    public abstract class GLTexture
    {
        public readonly TextureTarget           glTarget;
        protected       int           glHandle;
        protected       TextureFilter minFilter = TextureFilter.Nearest;
        protected       TextureFilter magFilter = TextureFilter.Nearest;
        protected       TextureWrap   uWrap     = TextureWrap.ClampToEdge;
        protected       TextureWrap   vWrap     = TextureWrap.ClampToEdge;

        public abstract int getWidth();

        public abstract int getHeight();

        public abstract int getDepth();

        public GLTexture(TextureTarget glTarget) : this(glTarget, GL.GenTexture())
        {
        }

        public GLTexture(TextureTarget glTarget, int glHandle)
        {
            this.glTarget = glTarget;
            this.glHandle = glHandle;
        }

        public abstract bool isManaged();

        protected abstract void reload();

        public void bind()
        {
            GL.BindTexture(glTarget, glHandle);
        }

        public void bind(int unit)
        {
            GL.ActiveTexture(TextureUnit.Texture0 + unit);
            GL.BindTexture(glTarget, glHandle);
        }

        public TextureFilter getMinFilter()
        {
            return minFilter;
        }

        public TextureFilter getMagFilter()
        {
            return magFilter;
        }

        public TextureWrap getUWrap()
        {
            return uWrap;
        }

        public TextureWrap getVWrap()
        {
            return vWrap;
        }

        public int getTextureObjectHandle()
        {
            return glHandle;
        }

        public void unsafeSetWrap(TextureWrap u, TextureWrap v)
        {
            unsafeSetWrap(u, v, false);
        }

        public void unsafeSetWrap(TextureWrap u, TextureWrap v, bool force)
        {
            if (u != null && (force || uWrap != u))
            {
                GL.TexParameter(glTarget, TextureParameterName.TextureWrapS, TextureHelper.getGLEnumFromTextureWrap(u));
                uWrap = u;
            }

            if (v != null && (force || vWrap != v))
            {
                GL.TexParameter(glTarget, TextureParameterName.TextureWrapT, TextureHelper.getGLEnumFromTextureWrap(v));
                vWrap = v;
            }
        }

        public void setWrap(TextureWrap u, TextureWrap v)
        {
            this.uWrap = u;
            this.vWrap = v;
            bind();
            GL.TexParameter(glTarget, TextureParameterName.TextureWrapS, TextureHelper.getGLEnumFromTextureWrap(u));
            GL.TexParameter(glTarget, TextureParameterName.TextureWrapT, TextureHelper.getGLEnumFromTextureWrap(v));
        }

        public void unsafeSetFilter(TextureFilter minFilter, TextureFilter magFilter)
        {
            unsafeSetFilter(minFilter, magFilter, false);
        }

        public void unsafeSetFilter(TextureFilter minFilter, TextureFilter magFilter, bool force)
        {
            if (minFilter != null && (force || this.minFilter != minFilter))
            {
                GL.TexParameter(glTarget, TextureParameterName.TextureMinFilter, TextureHelper.getGLEnumFromTextureFilter(minFilter));
                this.minFilter = minFilter;
            }

            if (magFilter != null && (force || this.magFilter != magFilter))
            {
                GL.TexParameter(glTarget, TextureParameterName.TextureMagFilter,TextureHelper.getGLEnumFromTextureFilter(magFilter));
                this.magFilter = magFilter;
            }
        }

        public void setFilter(TextureFilter minFilter, TextureFilter magFilter)
        {
            this.minFilter = minFilter;
            this.magFilter = magFilter;
            bind();
            GL.TexParameter(glTarget, TextureParameterName.TextureMinFilter, TextureHelper.getGLEnumFromTextureFilter(minFilter));
            GL.TexParameter(glTarget, TextureParameterName.TextureMagFilter, TextureHelper.getGLEnumFromTextureFilter(magFilter));
        }

        protected void delete()
        {
            if (glHandle != 0)
            {
                GL.DeleteTexture(glHandle);
                glHandle = 0;
            }
        }

        public void dispose()
        {
            delete();
        }
    }
}