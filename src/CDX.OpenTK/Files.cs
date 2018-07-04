using System;

namespace CDX.GLFWBackend
{
    public class Files : IFiles
    {
        public FileHandle getFileHandle(string path, FileType type)
        {
            throw new NotImplementedException();
        }

        public FileHandle classpath(string path)
        {
            throw new NotImplementedException();
        }

        public FileHandle @internal(string path)
        {
            throw new NotImplementedException();
        }

        public FileHandle external(string path)
        {
            throw new NotImplementedException();
        }

        public FileHandle absolute(string path)
        {
            throw new NotImplementedException();
        }

        public FileHandle local(string path)
        {
            throw new NotImplementedException();
        }

        public string getExternalStoragePath()
        {
            throw new NotImplementedException();
        }

        public bool isExternalStorageAvailable()
        {
            throw new NotImplementedException();
        }

        public string getLocalStoragePath()
        {
            throw new NotImplementedException();
        }

        public bool isLocalStorageAvailable()
        {
            throw new NotImplementedException();
        }
    }
}