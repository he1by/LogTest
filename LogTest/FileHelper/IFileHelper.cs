namespace LogComponent.FileHelper
{
    public interface IFileHelper
    {
        void Write(string text);

        void Flush();

        void Dispose();

        void InitDirectory(string folderPath);

        void InitFileStream();
    }
}
