using System;
using System.IO;
using CDX.Graphics;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace CDX.GLFWBackend.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new ApplicationConfiguration();
            config.setInitialBackgroundColor(Color.BLACK);

            var app = new Application(new Sandbox(), config);
        }
    }

    public class Sandbox : ApplicationAdapter
    {
        private SpriteBatch   _batch;
        private Texture       _texture;

        public override void create()
        {
            Gdx.app.log("Game", "Create");
            Gdx.input.setInputProcessor(new InputController());

            _batch = new SpriteBatch();
            _texture = Texture.loadFromFile("badlogic.jpg");
        }

        public override void render()
        {
            GL.Viewport(0,0, Gdx.graphics.getBackBufferWidth(), Gdx.graphics.getHeight());
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.ClearColor(0.5f,0,0.25f, 1);
            
            _batch.begin();
            _batch.draw(_texture, 0, 0);
            _batch.end();
        }
    }

    public class TestMesh : IApplicationListener
    {
        public ShaderProgram shader;
        public Mesh          mesh;
        public Texture       texture;
        public OrthographicCamera camera;

        public void create()
        {
            Gdx.app.log("Game", "Create");
            Gdx.input.setInputProcessor(new InputController());

            texture = Texture.loadFromFile("badlogic.jpg");
            var stuff = createMesh();
            mesh = stuff.Item1;
            shader = stuff.Item2;
            camera = new OrthographicCamera(Gdx.graphics.getWidth(), Gdx.graphics.getHeight());
        }

        public void resize(int width, int height)
        {
            Gdx.app.log("Game", $"Resize: {width}:{height}");

            camera.viewportWidth = width;
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

    class InputController : InputAdapter
    {
        public override bool keyDown(Keys keycode)
        {
            Gdx.app.log("InputController", $"KeyDown: {keycode}");
            return base.keyDown(keycode);
        }

        public override bool keyUp(Keys keycode)
        {
            Gdx.app.log("InputController", $"KeyUp: {keycode}");
            return base.keyDown(keycode);
        }

        public override bool keyTyped(char character)
        {
            Gdx.app.log("InputController", $"KeyTyped: {character}");
            return base.keyTyped(character);
        }

        public override bool touchDown(int screenX, int screenY, int pointer, Buttons button)
        {
            Gdx.app.log("InputController", $"TouchDown: {button} {screenX}:{screenY}");
            return base.touchDown(screenX, screenY, pointer, button);
        }

        public override bool touchUp(int screenX, int screenY, int pointer, Buttons button)
        {
            Gdx.app.log("InputController", $"TouchUp: {button} {screenX}:{screenY}");
            return base.touchDown(screenX, screenY, pointer, button);
        }
    }
}