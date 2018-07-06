namespace CDX.Graphics.G3D.Utils
{
    public interface TextureProvider
    {
        Texture load(string fileName);
    }

    public class FileTextureProvider : TextureProvider
    {
        private TextureFilter minFilter, magFilter;
        private TextureWrap   uWrap,     vWrap;
        private bool          useMipMaps;

        public FileTextureProvider()
        {
            minFilter  = magFilter = TextureFilter.Linear;
            uWrap      = vWrap     = TextureWrap.Repeat;
            useMipMaps = false;
        }

        public FileTextureProvider(TextureFilter minFilter, TextureFilter magFilter, TextureWrap uWrap,
            TextureWrap vWrap, bool useMipMaps)
        {
            this.minFilter  = minFilter;
            this.magFilter  = magFilter;
            this.uWrap      = uWrap;
            this.vWrap      = vWrap;
            this.useMipMaps = useMipMaps;
        }

        public Texture load(string fileName)
        {
            Texture result = Texture.loadFromFile(fileName, useMipMaps);
            result.setFilter(minFilter, magFilter);
            result.setWrap(uWrap, vWrap);
            return result;
        }
    }
}