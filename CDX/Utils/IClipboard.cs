namespace CDX.Utils
{
    public interface IClipboard
    {
        string getContents();

        void setContents(string content);
    }
}