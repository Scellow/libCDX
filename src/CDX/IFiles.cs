using System;

namespace CDX
{
    public enum FileType
    {
        Classpath,

        Internal,

        External,

        Absolute,

        Local
    }

    public interface IFiles
    {
        FileHandle getFileHandle(string path, FileType type);

        FileHandle classpath(string path);

        FileHandle @internal(string path);

        FileHandle external(string path);

        FileHandle absolute(string path);

        FileHandle local(string path);

        string getExternalStoragePath();

        bool isExternalStorageAvailable();

        string getLocalStoragePath();

        bool isLocalStorageAvailable();
    }
}