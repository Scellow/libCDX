using System.IO;

namespace CDX
{
    public class FileHandle
    {
        protected string   file;
        protected FileType type;

        public FileHandle(string fileName, FileType type = FileType.Absolute)
        {
            this.file = fileName;
            this.type = type;
        }

        public string path()
        {
            return Path.GetFullPath(file);
        }
    }
}