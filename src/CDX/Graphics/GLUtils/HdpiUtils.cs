using OpenTK.Graphics.OpenGL4;

namespace CDX.Graphics.GLUtils
{
    public class HdpiUtils
    {
        public static void glScissor(int x, int y, int width, int height)
        {
            if (Gdx.graphics.getWidth() != Gdx.graphics.getBackBufferWidth()
                || Gdx.graphics.getHeight() != Gdx.graphics.getBackBufferHeight())
            {
                GL.Scissor(toBackBufferX(x), toBackBufferY(y), toBackBufferX(width), toBackBufferY(height));
            }
            else
            {
                GL.Scissor(x, y, width, height);
            }
        }

        public static void glViewport(int x, int y, int width, int height)
        {
            if (Gdx.graphics.getWidth() != Gdx.graphics.getBackBufferWidth()
                || Gdx.graphics.getHeight() != Gdx.graphics.getBackBufferHeight())
            {
                GL.Viewport(toBackBufferX(x), toBackBufferY(y), toBackBufferX(width), toBackBufferY(height));
            }
            else
            {
                GL.Viewport(x, y, width, height);
            }
        }

        public static int toLogicalX(int backBufferX)
        {
            return (int) (backBufferX * Gdx.graphics.getWidth() / (float) Gdx.graphics.getBackBufferWidth());
        }

        public static int toLogicalY(int backBufferY)
        {
            return (int) (backBufferY * Gdx.graphics.getHeight() / (float) Gdx.graphics.getBackBufferHeight());
        }

        public static int toBackBufferX(int logicalX)
        {
            return (int) (logicalX * Gdx.graphics.getBackBufferWidth() / (float) Gdx.graphics.getWidth());
        }

        public static int toBackBufferY(int logicalY)
        {
            return (int) (logicalY * Gdx.graphics.getBackBufferHeight() / (float) Gdx.graphics.getHeight());
        }
    }
}