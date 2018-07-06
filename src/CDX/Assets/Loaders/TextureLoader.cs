using System;
using System.Collections.Generic;
using CDX.Graphics;

namespace CDX.Assets.Loaders
{
    public class TextureLoader : AsynchronousAssetLoader<Texture, TextureParameter>
    {
        public override List<AssetDescriptor<Texture>> getDependencies(string fileName, FileHandle file, TextureParameter parameter)
        {
            throw new NotImplementedException();
        }

        public override void loadAsync(AssetManager manager, string fileName, FileHandle file, TextureParameter parameter)
        {
        }

        public override Texture loadSync(AssetManager manager, string fileName, FileHandle file, TextureParameter parameter)
        {
            throw new NotImplementedException();
        }
    }

    public class TextureParameter : AssetLoaderParameters<Texture>
    {
        //public Format format = null;

        public bool genMipMaps = false;

        public Texture texture = null;

        // optional
        //public TextureData   textureData = null;
        public TextureFilter minFilter   = TextureFilter.Nearest;
        public TextureFilter magFilter   = TextureFilter.Nearest;
        public TextureWrap   wrapU       = TextureWrap.ClampToEdge;
        public TextureWrap   wrapV       = TextureWrap.ClampToEdge;
    }
}