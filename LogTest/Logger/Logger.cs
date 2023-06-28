using LogComponent.FileHelper;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace LogComponent
{
    public sealed class Logger : ILogger, IDisposable
    {
        private readonly CancellationTokenSource _stopCTS = new CancellationTokenSource();
        private readonly CancellationTokenSource _softCTS = new CancellationTokenSource();
        private readonly ConcurrentQueue<string> _logs = new ConcurrentQueue<string>();
        private readonly IFileHelper _writer;
        private readonly Task _runner;

        public Logger(IFileHelper writer)
        {
            _writer = writer;
            _runner = Task.Run(HandleLogs, _stopCTS.Token);
        }

        public void Stop(bool withFlush)
        {
            if (!withFlush)
            {
                _softCTS.Cancel();
                _runner.Wait();
                _writer.Flush();
            }
            else
            {
                _stopCTS.Cancel();
            }

        }

        public void WriteLog(string text)
        {
            _logs.Enqueue($"{DateTime.Now:yyyy-MM-dd HH:mm:ss:fff} {text}. \r\n");
        }

        public void Dispose()
        {
            _stopCTS.Cancel();
            _stopCTS.Dispose();
            _runner.Dispose();
        }

        private async Task HandleLogs()
        {
            while (!_softCTS.IsCancellationRequested)
            {
                while (!_logs.IsEmpty && _logs.TryDequeue(out var line))
                {
                    try
                    {
                        _writer.Write(line);
                    }
                    catch (Exception ex)
                    {
                        //TODO: create custom exceptions for some cases
                        Console.WriteLine($"Log: {line} throw exception: {ex.Message}");
                    }
                }
                //TODO: refactor Task.Delay
                await Task.Delay(TimeSpan.FromMilliseconds(30), _stopCTS.Token).ConfigureAwait(false);
            }
        }
    }
}