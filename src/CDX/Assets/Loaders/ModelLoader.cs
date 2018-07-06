using System;
using System.Collections.Generic;
using CDX.Graphics;
using CDX.Graphics.G3D;
using CDX.Graphics.G3D.Models.Data;

namespace CDX.Assets.Loaders
{
    public abstract class ModelLoader<P> : AsynchronousAssetLoader<Model, P> where P : ModelParameters
    {
        protected List<KeyValuePair<string, ModelData>> items             = new List<KeyValuePair<string, ModelData>>();
        protected ModelParameters                       defaultParameters = new ModelParameters();

        public override List<AssetDescriptor<Model>> getDependencies(string fileName, FileHandle file, P parameter)
        {
            /*
            var deps = new List<AssetDescriptor<>>();
            var data = loadModelData(file, parameters);
            if (data == null) return deps;

            var item = new ObjectMap.Entry<String, ModelData>();
            item.key   = fileName;
            item.value = data;
            lock (items)
            {
                items.add(item);
            }

            TextureParameter textureParameter = (parameter != null)
                ? parameter.textureParameter
                : defaultParameters.textureParameter;

            foreach (var modelMaterial in data.materials)
            {
                if (modelMaterial.textures != null)
                {
                    foreach (var modelTexture in modelMaterial.textures)
                        deps.Add(new AssetDescriptor(modelTexture.fileName, Texture.class,
                    textureParameter));
                }
            }

            return deps;
            */
            throw new NotImplementedException();
        }
    }

    public class ModelParameters : AssetLoaderParameters<Model>
    {
        public TextureParameter textureParameter;

        public ModelParameters()
        {
            textureParameter           = new TextureParameter();
            textureParameter.minFilter = textureParameter.magFilter = TextureFilter.Linear;
            textureParameter.wrapU     = textureParameter.wrapV     = TextureWrap.Repeat;
        }
    }
}