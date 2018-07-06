using System;
using System.Collections.Generic;
using CDX.Graphics.G3D.Utils;
using CDX.Utils;

namespace CDX.Graphics.G3D
{
    public class ModelBatch : IDisposable
    {
        public class RenderablePool : FlushablePool<Renderable>
        {
            protected override Renderable newObject()
            {
                return new Renderable();
            }

            public override Renderable obtain()
            {
                var renderable = base.obtain();

                renderable.environment = null;
                renderable.material    = null;
                renderable.meshPart.set("", null, 0, 0, 0);
                renderable.shader   = null;
                renderable.userData = null;
                return renderable;
            }
        }

        protected          Camera           camera;
        protected readonly RenderablePool   renderablesPool = new RenderablePool();
        protected readonly List<Renderable> renderables     = new List<Renderable>();
        protected readonly RenderContext    context;
        private readonly   bool             ownContext;
        protected readonly ShaderProvider   shaderProvider;
        protected readonly RenderableSorter sorter;

        public ModelBatch() : this(null, null, null)
        {
        }


        public ModelBatch(RenderContext context, ShaderProvider shaderProvider, RenderableSorter sorter)
        {
            this.sorter         = sorter ?? new DefaultRenderableSorter();
            this.ownContext     = (context == null);
            this.context        = context ?? new RenderContext(new DefaultTextureBinder(DefaultTextureBinder.WEIGHTED, 1));
            this.shaderProvider = shaderProvider ?? new DefaultShaderProvider();
        }

        public void Dispose()
        {
        }

        public void begin(Camera cam)
        {
            if (camera != null) throw new Exception("Call end() first.");
            camera = cam;
            if (ownContext) context.begin();
        }

        public void end()
        {
            flush();
            if (ownContext) context.end();
            camera = null;
        }

        public void flush()
        {
            sorter.sort(camera, renderables);
            Shader currentShader = null;
            for (int i = 0; i < renderables.Count; i++)
            {
                Renderable renderable = renderables[i];
                if (currentShader != renderable.shader)
                {
                    currentShader?.end();
                    currentShader = renderable.shader;
                    currentShader.begin(camera, context);
                }

                currentShader.render(renderable);
            }

            currentShader?.end();
            renderablesPool.flush();
            renderables.Clear();
        }

        public void render(RenderableProvider renderableProvider)
        {
            int offset = renderables.Count;
            renderableProvider.getRenderables(renderables, renderablesPool);
            for (int i = offset; i < renderables.Count; i++)
            {
                Renderable renderable = renderables[i];
                
                if(renderable == null) throw new Exception("shouldn't be null");
                renderable.shader = shaderProvider.getShader(renderable);
            }
        }
    }

    public interface RenderableProvider
    {
        void getRenderables(IList<Renderable> renderables, Pool<Renderable> pool);
    }
}