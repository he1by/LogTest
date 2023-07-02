using LogComponent.FileHelper;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace LogComponent
{
    public sealed class Logger : ILogger, IDisposable
    {
        private IFileHelper _writer;
        //TODO: think how to refactor ro private and test it
        public readonly ConcurrentQueue<string> Logs = new ConcurrentQueue<string>();
        private readonly CancellationTokenSource _stopCTS = new CancellationTokenSource();
        private readonly CancellationTokenSource _softCTS = new CancellationTokenSource();  
        private readonly Task _runner;
        private readonly DateTime _currentDateTime = DateTime.Now;

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
            Logs.Enqueue($"{_currentDateTime:yyyy-MM-dd HH:mm:ss:fff} {text}. \r\n");
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
                while (!Logs.IsEmpty && Logs.TryDequeue(out var line))
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