namespace CDX.Graphics.G3D.Utils
{
    public interface ShaderProvider
    {
        Shader getShader(Renderable renderable);
        void dispose();
    }
}