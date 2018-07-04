using OpenTK.Graphics.OpenGL4;

namespace CDX.Graphics
{
    public class RenderContext
    {
        public readonly TextureBinder textureBinder;
        private bool blending;
        private BlendingFactor blendSFactor;
        private BlendingFactor blendDFactor;
        private DepthFunction depthFunc;
        private float   depthRangeNear;
        private float   depthRangeFar;
        private bool depthMask;
        private CullFaceMode     cullFace;
        
        public RenderContext (TextureBinder textures) {
            this.textureBinder = textures;
        }

        public void begin () {
            GL.Disable(EnableCap.DepthTest);
            depthFunc = 0;
            GL.DepthMask(true);
            depthMask = true;
            GL.Disable(EnableCap.Blend);
            blending = false;
            GL.Disable(EnableCap.CullFace);
            blendSFactor = blendDFactor = BlendingFactor.Zero;
            // todo: cullface = 0 ?
            textureBinder.begin();
        }
        
        public void end () {
            if (depthFunc != 0) GL.Disable(EnableCap.DepthTest);
            if (!depthMask) GL.DepthMask(true);
            if (blending) GL.Disable(EnableCap.Blend);
            if (cullFace > 0) GL.Disable(EnableCap.CullFace);
            textureBinder.end();
        }
        
        
        public void setDepthMask (bool depthMask) {
            if (this.depthMask != depthMask) GL.DepthMask(this.depthMask = depthMask);
        }

        public void setDepthTest (DepthFunction depthFunction) {
            setDepthTest(depthFunction, 0f, 1f);
        }

        public void setDepthTest (DepthFunction depthFunction, float depthRangeNear, float depthRangeFar) {
            bool wasEnabled = depthFunc != 0;
            bool enabled    = depthFunction != 0;
            if (depthFunc != depthFunction) {
                depthFunc = depthFunction;
                if (enabled) {
                    GL.Enable(EnableCap.DepthTest);
                    GL.DepthFunc(  depthFunction);
                } else
                    GL.Disable(EnableCap.DepthTest);
            }
            if (enabled) {
                if (!wasEnabled || depthFunc != depthFunction) GL.DepthFunc(depthFunc = depthFunction);
                if (!wasEnabled || this.depthRangeNear != depthRangeNear || this.depthRangeFar != depthRangeFar)
                    GL.DepthRange(this.depthRangeNear = depthRangeNear, this.depthRangeFar = depthRangeFar);
            }
        }

        public void setBlending (bool enabled, BlendingFactor sFactor, BlendingFactor dFactor) {
            if (enabled != blending) {
                blending = enabled;
                if (enabled)
                   GL.Enable(EnableCap.Blend);
                else
                   GL.Disable(EnableCap.Blend);
            }
            if (enabled && (blendSFactor != sFactor || blendDFactor != dFactor)) {
               GL.BlendFunc(sFactor, dFactor);
                blendSFactor = sFactor;
                blendDFactor = dFactor;
            }
        }

        public void setCullFace (CullFaceMode face) {
            if (face != cullFace) {
                cullFace = face;
                if ((face == CullFaceMode.Front) || (face == CullFaceMode.Back) || (face == CullFaceMode.FrontAndBack)) {
                   GL.Enable(EnableCap.CullFace);
                   GL.CullFace(face);
                } else
                   GL.Disable(EnableCap.CullFace);
            }
        }
    }
}