using System;
using OpenTK.Graphics.OpenGL4;

namespace CDX.Graphics.G3D.Utils
{
    public class DefaultTextureBinder : TextureBinder
    {
        public static readonly int ROUNDROBIN = 0;

        public static readonly int WEIGHTED = 1;

        public static readonly int MAX_GLES_UNITS = 32;

        private readonly int offset;

        private readonly int count;

        private readonly int reuseWeight;

        private readonly GLTexture[] textures;

        private readonly int[] weights;

        private readonly int method;

        private bool reused;

        private int reuseCount = 0; // TODO remove debug code
        private int bindCount  = 0; // TODO remove debug code

        public DefaultTextureBinder(int method) : this(method, 0)
        {
        }

        public DefaultTextureBinder(int method, int offset) : this(method, offset, -1)
        {
        }

        public DefaultTextureBinder(int method, int offset, int count) : this(method, offset, count, 10)
        {
        }

        public DefaultTextureBinder(int method, int offset, int count, int reuseWeight)
        {
            int max              = Math.Min(getMaxTextureUnits(), MAX_GLES_UNITS);
            if (count < 0) count = max - offset;
            if (offset < 0 || count < 0 || (offset + count) > max || reuseWeight < 1)
                throw new Exception("Illegal arguments");
            this.method      = method;
            this.offset      = offset;
            this.count       = count;
            this.textures    = new GLTexture[count];
            this.reuseWeight = reuseWeight;
            this.weights     = (method == WEIGHTED) ? new int[count] : null;
        }

        private static int getMaxTextureUnits()
        {
            var result = GL.GetInteger(GetPName.MaxTextureImageUnits);
            return result;
        }

        public void begin()
        {
            for (int i = 0; i < count; i++) {
                textures[i] = null;
                if (weights != null) weights[i] = 0;
            }
        }

        public void end()
        {
            GL.ActiveTexture(TextureUnit.Texture0);
        }

        public int bind(TextureDescriptor textureDesc)
        {
            return bindTexture(textureDesc, false);
        }

        // todo: redo this TextureDescriptor thing
        private readonly TextureDescriptor tempDesc = new TextureDescriptor();
        public int bind(GLTexture texture)
        {
            // todo: what about filters ?
            tempDesc.set(texture, texture.getMinFilter(), texture.getMagFilter(), texture.getUWrap(), texture.getVWrap());
            return bindTexture(tempDesc, false);
        }
        
        private int bindTexture (TextureDescriptor textureDesc, bool rebind) {
            int       idx, result;
            GLTexture texture = textureDesc.texture;
            reused = false;

            switch (method) {
                case 0:
                    result = offset + (idx = bindTextureRoundRobin(texture));
                    break;
                case 1:
                    result = offset + (idx = bindTextureWeighted(texture));
                    break;
                default:
                    return -1;
            }

            if (reused) {
                reuseCount++;
                if (rebind)
                    texture.bind(result);
                else
                    GL.ActiveTexture(TextureUnit.Texture0 + result);
            } else
                bindCount++;
            texture.unsafeSetWrap(textureDesc.uWrap, textureDesc.vWrap);
            texture.unsafeSetFilter(textureDesc.minFilter, textureDesc.magFilter);
            return result;
        }
        
        private int currentTexture = 0;
        
        private int bindTextureRoundRobin (GLTexture texture) {
            for (int i = 0; i < count; i++) {
                int idx = (currentTexture + i) % count;
                if (textures[idx] == texture) {
                    reused = true;
                    return idx;
                }
            }
            currentTexture           = (currentTexture + 1) % count;
            textures[currentTexture] = texture;
            texture.bind(offset + currentTexture);
            return currentTexture;
        }

        private int bindTextureWeighted (GLTexture texture) {
            int result = -1;
            int weight = weights[0];
            int windex = 0;
            for (int i = 0; i < count; i++) {
                if (textures[i] == texture) {
                    result     =  i;
                    weights[i] += reuseWeight;
                } else if (weights[i] < 0 || --weights[i] < weight) {
                    weight = weights[i];
                    windex = i;
                }
            }
            if (result < 0) {
                textures[windex] = texture;
                weights[windex]  = 100;
                texture.bind(offset + (result = windex));
            } else
                reused = true;
            return result;
        }
        

        public int getBindCount()
        {
            return bindCount;
        }

        public int getReuseCount()
        {
            return reuseCount;
        }

        public void resetCounts()
        {
            bindCount = reuseCount = 0;
        }
    }
}