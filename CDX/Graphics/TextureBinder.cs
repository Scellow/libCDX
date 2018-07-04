namespace CDX.Graphics
{
    public interface TextureBinder {
        void begin ();

        void end ();

        int bind (TextureDescriptor textureDescriptor);

        int bind (GLTexture texture);

        int getBindCount ();

        int getReuseCount ();

        void resetCounts ();
    }

    public class TextureDescriptor
    {
        public GLTexture texture = null;
        public TextureFilter minFilter;
        public TextureFilter magFilter;
        public TextureWrap   uWrap;
        public TextureWrap   vWrap;
    }
}