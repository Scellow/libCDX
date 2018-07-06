using System.IO;
using CDX.Graphics;
using OpenTK.Graphics.OpenGL4;

namespace CDX.GLFWBackend.Demo
{
    public class TestMesh : IApplicationListener
    {
        public ShaderProgram      shader;
        public Mesh               mesh;
        public Texture            texture;
        public OrthographicCamera camera;

        public void create()
        {
            Gdx.app.log("Game", "Create");
            Gdx.input.setInputProcessor(new DebugInputController());

            texture = Texture.loadFromFile("badlogic.jpg");
            var stuff = createMesh();
            mesh   = stuff.Item1;
            shader = stuff.Item2;
            camera = new OrthographicCamera(Gdx.graphics.getWidth(), Gdx.graphics.getHeight());
        }

        public void resize(int width, int height)
        {
            Gdx.app.log("Game", $"Resize: {width}:{height}");

            camera.viewportWidth  = width;
            camera.viewportHeight = height;
            camera.update();
        }

        public void render()
        {
            GL.Viewport(0,0, Gdx.graphics.getBackBufferWidth(), Gdx.graphics.getHeight());
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.ClearColor(0.5f,0,0.25f, 1);
            
            texture.bind();
            shader.begin();
            shader.setUniformMatrix("u_projTrans", camera.projection);
            shader.setUniformi("u_texture", 0);
            mesh.render(shader, PrimitiveType.Triangles, 0, 6);
            shader.end();
        }

        public void pause()
        {
            Gdx.app.log("Game", "Pause");
        }

        public void resume()
        {
            Gdx.app.log("Game", "Resume");
        }

        public void dispose()
        {
            Gdx.app.log("Game", "Dispose");
            shader.dispose();
        }

        public static (Mesh, ShaderProgram) createMesh()
        {
            var mesh = new Mesh(true, 6, 0,
                new VertexAttributes(new VertexAttribute(VertexAttributes.Usage.Position, 3, ShaderProgram.POSITION_ATTRIBUTE),
                    new VertexAttribute(VertexAttributes.Usage.TextureCoordinates, 2, ShaderProgram.TEXCOORD_ATTRIBUTE + "0")));


            var   verts = new float[30];
            var   i     = 0;
            float x,     y;      // Mesh location in the world
            float width, height; // Mesh width and height

            x     = y      = 50f;
            width = height = 300f;

            //Top Left Vertex Triangle 1
            verts[i++] = x;          //X
            verts[i++] = y + height; //Y
            verts[i++] = 0;          //Z
            verts[i++] = 0f;         //U
            verts[i++] = 0f;         //V

            //Top Right Vertex Triangle 1
            verts[i++] = x + width;
            verts[i++] = y + height;
            verts[i++] = 0;
            verts[i++] = 1f;
            verts[i++] = 0f;

            //Bottom Left Vertex Triangle 1
            verts[i++] = x;
            verts[i++] = y;
            verts[i++] = 0;
            verts[i++] = 0f;
            verts[i++] = 1f;

            //Top Right Vertex Triangle 2
            verts[i++] = x + width;
            verts[i++] = y + height;
            verts[i++] = 0;
            verts[i++] = 1f;
            verts[i++] = 0f;

            //Bottom Right Vertex Triangle 2
            verts[i++] = x + width;
            verts[i++] = y;
            verts[i++] = 0;
            verts[i++] = 1f;
            verts[i++] = 1f;

            //Bottom Left Vertex Triangle 2
            verts[i++] = x;
            verts[i++] = y;
            verts[i++] = 0;
            verts[i++] = 0f;
            verts[i]   = 1f;
            
            mesh.setVertices(verts);

            var shaderProgram = new ShaderProgram(
                File.ReadAllText("shader.vert"),
                File.ReadAllText("shader.frag")
            );

            return (mesh, shaderProgram);
        }
    }
}