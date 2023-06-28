using System;
using System.IO;
using System.Text;

namespace LogComponent.FileHelper
{
    public class FileHelper : IDisposable, IFileHelper
    {
        public string FileHeader { get; set; }
        private readonly string _fileName;
        private FileStream _fileStream;

        public FileHelper(string folderPath, string fileName)
        {
            InitDirectory(folderPath);
            //TODO: move to service/extension
            _fileName = Path.Combine(folderPath, $"{fileName}_{DateTime.UtcNow:yyyy-MM-dd}.log");
        }

        public void Write(string text)
        {

            if (!File.Exists(_fileName))
            {
                //TODO: as an idea refactor to using()
                _fileStream?.Flush();
                _fileStream?.Dispose();
                _fileStream = new FileStream(_fileName, FileMode.Create, FileAccess.Write, FileShare.Read);

                if (!string.IsNullOrEmpty(FileHeader))
                {
                    var header = Encoding.UTF8.GetBytes($"{FileHeader}\r\n");
                    _fileStream.Write(header, 0, header.Length);
                }
            }

            var bytes = Encoding.UTF8.GetBytes(text);
            _fileStream.Write(bytes, 0, bytes.Length);
        }

        public void Dispose() => _fileStream.Dispose();

        public void Flush() => _fileStream.Flush();

        private void InitDirectory(string folderPath)
        {
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
        }
    }
}