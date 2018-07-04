using System;
using CDX.Graphics.G2D;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace CDX.Graphics
{
    public class SpriteBatch
    {
        private Mesh mesh;


        readonly float[] vertices;
        int              idx         = 0;
        Texture          lastTexture = null;
        float            invTexWidth = 0, invTexHeight = 0;

        bool drawing = false;

        private Matrix4 transformMatrix  = Matrix4.Identity;
        private Matrix4 projectionMatrix = Matrix4.Identity;
        private Matrix4 combinedMatrix   = Matrix4.Identity;

        private bool               blendingDisabled  = false;
        private BlendingFactorSrc  blendSrcFunc      = BlendingFactorSrc.SrcAlpha;
        private BlendingFactorDest blendDstFunc      = BlendingFactorDest.OneMinusSrcAlpha;
        private BlendingFactorSrc  blendSrcFuncAlpha = BlendingFactorSrc.SrcAlpha;
        private BlendingFactorDest blendDstFuncAlpha = BlendingFactorDest.OneMinusSrcAlpha;

        private readonly ShaderProgram shader;
        private          ShaderProgram customShader = null;
        private          bool          ownsShader;

        float         color     = Color.WHITE.toFloatBits();
        private Color tempColor = new Color(1, 1, 1, 1);

        public int renderCalls = 0;

        public int totalRenderCalls = 0;

        public int maxSpritesInBatch = 0;


        /** Constructs a new SpriteBatch. Sets the projection matrix to an orthographic projection with y-axis point upwards, x-axis
         * point to the right and the origin being in the bottom left corner of the screen. The projection will be pixel perfect with
         * respect to the current screen resolution.
         * <p>
         * The defaultShader specifies the shader to use. Note that the names for uniforms for this default shader are different than
         * the ones expect for shaders set with {@link #setShader(ShaderProgram)}. See {@link #createDefaultShader()}.
         * @param size The max number of sprites in a single batch. Max of 8191.
         * @param defaultShader The default shader to use. This is not owned by the SpriteBatch and must be disposed separately. */
        public SpriteBatch(int size = 1000, ShaderProgram defaultShader = null)
        {
            // todo: since we use uint for indices, we could rais this number uint.MAX / 4
            // 32767 is max vertex index, so 32767 / 4 vertices per sprite = 8191 sprites max.
            if (size > 8191) throw new Exception("Can't have more than 8191 sprites per batch: " + size);

            var vertexDataType = Mesh.VertexDataType.VertexBufferObjectWithVAO;

            mesh = new Mesh(vertexDataType, false, size * 4, size * 6,
                new VertexAttribute(VertexAttributes.Usage.Position, 2, ShaderProgram.POSITION_ATTRIBUTE),
                new VertexAttribute(VertexAttributes.Usage.ColorPacked, 4, ShaderProgram.COLOR_ATTRIBUTE),
                new VertexAttribute(VertexAttributes.Usage.TextureCoordinates, 2, ShaderProgram.TEXCOORD_ATTRIBUTE + "0"));

            projectionMatrix = Matrix4.CreateOrthographicOffCenter(0, Gdx.graphics.getWidth(), 0, Gdx.graphics.getHeight(), 0, 1);

            vertices = new float[size * Sprite.SPRITE_SIZE];

            int    len     = size * 6;
            uint[] indices = new uint[len];
            uint   j       = 0;
            for (int i = 0; i < len; i += 6, j += 4)
            {
                indices[i]     = j;
                indices[i + 1] = (uint) (j + 1);
                indices[i + 2] = (uint) (j + 2);
                indices[i + 3] = (uint) (j + 2);
                indices[i + 4] = (uint) (j + 3);
                indices[i + 5] = j;
            }

            mesh.setIndices(indices);

            if (defaultShader == null)
            {
                shader     = createDefaultShader();
                ownsShader = true;
            }
            else
                shader = defaultShader;
        }

        static public ShaderProgram createDefaultShader()
        {
            String vertexShader = "attribute vec4 " + ShaderProgram.POSITION_ATTRIBUTE + ";\n"                    //
                                  + "attribute vec4 " + ShaderProgram.COLOR_ATTRIBUTE + ";\n"                     //
                                  + "attribute vec2 " + ShaderProgram.TEXCOORD_ATTRIBUTE + "0;\n"                 //
                                  + "uniform mat4 u_projTrans;\n"                                                 //
                                  + "varying vec4 v_color;\n"                                                     //
                                  + "varying vec2 v_texCoords;\n"                                                 //
                                  + "\n"                                                                          //
                                  + "void main()\n"                                                               //
                                  + "{\n"                                                                         //
                                  + "   v_color = " + ShaderProgram.COLOR_ATTRIBUTE + ";\n"                       //
                                  + "   v_color.a = v_color.a * (255.0/254.0);\n"                                 //
                                  + "   v_texCoords = " + ShaderProgram.TEXCOORD_ATTRIBUTE + "0;\n"               //
                                  + "   gl_Position =  u_projTrans * " + ShaderProgram.POSITION_ATTRIBUTE + ";\n" //
                                  + "}\n";
            String fragmentShader = "#ifdef GL_ES\n"                                                    //
                                    + "#define LOWP lowp\n"                                             //
                                    + "precision mediump float;\n"                                      //
                                    + "#else\n"                                                         //
                                    + "#define LOWP \n"                                                 //
                                    + "#endif\n"                                                        //
                                    + "varying LOWP vec4 v_color;\n"                                    //
                                    + "varying vec2 v_texCoords;\n"                                     //
                                    + "uniform sampler2D u_texture;\n"                                  //
                                    + "void main()\n"                                                   //
                                    + "{\n"                                                             //
                                    + "  gl_FragColor = v_color * texture2D(u_texture, v_texCoords);\n" //
                                    + "}";

            ShaderProgram shader = new ShaderProgram(vertexShader, fragmentShader);
            if (shader.isCompiled_() == false) throw new Exception("Error compiling shader: " + shader.getLog());
            return shader;
        }
        
        
        public void begin () {
            if (drawing) throw new Exception("SpriteBatch.end must be called before begin.");
            renderCalls = 0;

            GL.DepthMask(false);
            if (customShader != null)
                customShader.begin();
            else
                shader.begin();
            setupMatrices();

            drawing = true;
        }
        
        public void end () {
            if (!drawing) throw new Exception("SpriteBatch.begin must be called before end.");
            if (idx > 0) flush();
            lastTexture = null;
            drawing     = false;

            GL.DepthMask(true);
            if (isBlendingEnabled()) GL.Disable(EnableCap.Blend);

            if (customShader != null)
                customShader.end();
            else
                shader.end();
        }
        
        
        public void flush () {
            if (idx == 0) return;

            renderCalls++;
            totalRenderCalls++;
            int spritesInBatch                                        = idx / 20;
            if (spritesInBatch > maxSpritesInBatch) maxSpritesInBatch = spritesInBatch;
            int count                                                 = spritesInBatch * 6;

            lastTexture.bind();
            var mesh = this.mesh;
            mesh.setVertices(vertices, 0, idx);
            //mesh.getIndicesBuffer().position(0);
            //mesh.getIndicesBuffer().limit(count);
 
            if (blendingDisabled) {
                GL.Disable(EnableCap.Blend);
            } else {
                GL.Enable(EnableCap.Blend);
                // todo: it's never -1, but check again to make sure..
                //if (blendSrcFunc != -1) 
                    GL.BlendFuncSeparate(blendSrcFunc, blendDstFunc, blendSrcFuncAlpha, blendDstFuncAlpha);
            }

            mesh.render(customShader != null ? customShader : shader, PrimitiveType.Triangles, 0, count);

            idx = 0;
        }
        
        
        public void setTransformMatrix (Matrix4 transform) {
            if (drawing) flush();
            transformMatrix = transform;
            if (drawing) setupMatrices();
        }
        
        public void setProjectionMatrix (Matrix4 projection) {
            if (drawing) flush();
            projectionMatrix = projection;
            if (drawing) setupMatrices();
        }

        private void setupMatrices () {
            combinedMatrix = projectionMatrix * transformMatrix;
            if (customShader != null) {
                customShader.setUniformMatrix("u_projTrans", combinedMatrix);
                customShader.setUniformi("u_texture", 0);
            } else {
                shader.setUniformMatrix("u_projTrans", combinedMatrix);
                shader.setUniformi("u_texture", 0);
            }
        }


        public void draw(Texture texture, float x, float y)
        {
            draw(texture, x, y, texture.getWidth(), texture.getHeight());
        }
        
        public void draw (Texture texture, float x, float y, float width, float height) {
            if (!drawing) throw new Exception("SpriteBatch.begin must be called before draw.");

            float[] vertices = this.vertices;

            if (texture != lastTexture)
                switchTexture(texture);
            else if (this.idx == vertices.Length) //
                flush();

            float fx2 = x + width;
            float fy2 = y + height;
            float u   = 0;
            float v   = 1;
            float u2  = 1;
            float v2  = 0;

            float color = this.color;
            int   idx   = this.idx;
            vertices[idx]     = x;
            vertices[idx + 1] = y;
            vertices[idx + 2] = color;
            vertices[idx + 3] = u;
            vertices[idx + 4] = v;

            vertices[idx + 5] = x;
            vertices[idx + 6] = fy2;
            vertices[idx + 7] = color;
            vertices[idx + 8] = u;
            vertices[idx + 9] = v2;

            vertices[idx + 10] = fx2;
            vertices[idx + 11] = fy2;
            vertices[idx + 12] = color;
            vertices[idx + 13] = u2;
            vertices[idx + 14] = v2;

            vertices[idx + 15] = fx2;
            vertices[idx + 16] = y;
            vertices[idx + 17] = color;
            vertices[idx + 18] = u2;
            vertices[idx + 19] = v;
            this.idx           = idx + 20;
        }
        
        protected void switchTexture (Texture texture) {
            flush();
            lastTexture  = texture;
            invTexWidth  = 1.0f / texture.getWidth();
            invTexHeight = 1.0f / texture.getHeight();
        }
        
        
        public bool isBlendingEnabled () {
            return !blendingDisabled;
        }

        public bool isDrawing () {
            return drawing;
        }
    }
}