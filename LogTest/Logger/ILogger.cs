namespace LogComponent
{
    public interface ILogger
    {
        void Stop(bool withFlush);

        void WriteLog(string text);
    }
}
