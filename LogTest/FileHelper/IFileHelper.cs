namespace LogComponent.FileHelper
{
    public interface IFileHelper
    {
        void Write(string text);

        void Flush();
    }
}
