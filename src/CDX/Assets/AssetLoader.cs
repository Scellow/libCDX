using System;
using System.Collections.Generic;

namespace CDX.Assets
{
    public class AssetDescriptor<T>
    {
        public readonly string fileName;
        public readonly Type type;
        public AssetLoaderParameters<T> parameters;

        public AssetDescriptor(string fileName)
        {
            
        }
    }
    public class AssetLoaderParameters<T>
    {
        
    }

    public abstract class AssetLoader<T, P> where P : AssetLoaderParameters<T>
    {
        public abstract List<AssetDescriptor<T>> getDependencies(String fileName, FileHandle file, P parameter);
    }

    public abstract class AsynchronousAssetLoader<T, P> : AssetLoader<T, P> where P : AssetLoaderParameters<T>
    {
        public abstract void loadAsync(AssetManager manager, String fileName, FileHandle file, P parameter);
        public abstract T loadSync(AssetManager manager, String fileName, FileHandle file, P parameter);
    }
}