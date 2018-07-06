using System;
using System.IO;

namespace CDX.GLFWBackend
{
    public class GLFWFileHandle : FileHandle
    {
        public GLFWFileHandle(string fileName, FileType type) : base(fileName, type)
        {
        }
    }

    public class Files : IFiles
    {
        public static readonly string externalPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + Path.DirectorySeparatorChar;
        public static readonly string localPath = Environment.CurrentDirectory + Path.DirectorySeparatorChar;
        
        public FileHandle getFileHandle(string path, FileType type)
        {
            return new GLFWFileHandle(path, type);
        }

        public FileHandle classpath(string path)
        {
            return new GLFWFileHandle(path, FileType.Classpath);
        }

        public FileHandle @internal(string path)
        {
            return new GLFWFileHandle(path, FileType.Internal);
        }

        public FileHandle external(string path)
        {
            return new GLFWFileHandle(path, FileType.External);
        }

        public FileHandle absolute(string path)
        {
            return new GLFWFileHandle(path, FileType.Absolute);
        }

        public FileHandle local(string path)
        {
            return new GLFWFileHandle(path, FileType.Local);
        }

        public string getExternalStoragePath()
        {
            return externalPath;
        }

        public bool isExternalStorageAvailable()
        {
            return true;
        }

        public string getLocalStoragePath()
        {
            return localPath;
        }

        public bool isLocalStorageAvailable()
        {
            return true;
        }
    }
}