using System.Collections.Generic;
using CDX.Graphics.G3D.Shaders;

namespace CDX.Graphics.G3D.Utils
{
    public abstract class BaseShaderProvider : ShaderProvider
    {
        protected List<Shader> shaders = new List<Shader>();

        public Shader getShader(Renderable renderable)
        {
            Shader suggestedShader = renderable.shader;
            if (suggestedShader != null && suggestedShader.canRender(renderable)) 
                return suggestedShader;
            foreach (Shader shader in shaders)
            {
                if (shader.canRender(renderable)) return shader;
            }

            Shader ret = createShader(renderable);
            ret.init();
            shaders.Add(ret);
            return ret;
        }

        protected abstract Shader createShader(Renderable renderable);

        public void dispose()
        {
            foreach (Shader shader in shaders)
            {
                shader.Dispose();
            }

            shaders.Clear();
        }
    }

    public class DefaultShaderProvider : BaseShaderProvider
    {
        public DefaultShader.Config config;

        public DefaultShaderProvider(DefaultShader.Config config)
        {
            this.config = config ?? new DefaultShader.Config();
        }

        public DefaultShaderProvider(string vertexShader, string fragmentShader) : this(new DefaultShader.Config(vertexShader, fragmentShader))
        {
        }

        public DefaultShaderProvider() : this(null)
        {
        }


        protected override Shader createShader(Renderable renderable)
        {
            return new DefaultShader(renderable, config);
        }
    }
}