using CDX.Graphics;
using OpenTK.Graphics.OpenGL4;

namespace CDX.GLFWBackend.Demo
{
    public class Sandbox : ApplicationAdapter
    {
        private SpriteBatch _batch;
        private Texture     _texture;
        private int         _sprites;

        public override void create()
        {
            Gdx.app.log("Game", "Create");

            _batch   = new SpriteBatch();
            _texture = Texture.loadFromFile("badlogic.jpg");
        }

        public override void render()
        {
            GL.Viewport(0,0, Gdx.graphics.getBackBufferWidth(), Gdx.graphics.getHeight());
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.ClearColor(0.5f,0,0.25f, 1);


            _batch.begin();

            var size = 200;
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    _batch.draw(_texture, x * 16, y * 16, 16, 16);
                }
            }
            
            _batch.end();
        }
    }
}