using System;
using System.IO;
using System.Text;

namespace LogComponent.FileHelper
{
    public class FileHelper : IDisposable, IFileHelper
    {
        //TODO: move to resources
        public FileStream FileStream;
        //TODO: refactor
        private readonly string _fileHeader = "Timestamp               Data\r\n";
        private string _folderPath;
        private string _fileName;
        private string _fullPathToFile;
        private DateTime currentDateTime = DateTime.UtcNow;

        //TODO: DI Filestream
        public FileHelper(string folderPath, string fileName)
        {
            _folderPath = folderPath;
            _fileName = fileName;
            InitDirectory(folderPath);
            //TODO: move to service/extension
            _fullPathToFile = Path.Combine(folderPath, $"{fileName}_{currentDateTime:yyyy-MM-dd}.log");
        }

        public void Write(string text)
        {

            if (!File.Exists(_fullPathToFile))
            {
                InitFileStream();
                if (!string.IsNullOrEmpty(_fileHeader))
                {
                    var header = Encoding.UTF8.GetBytes($"{_fileHeader}\r\n");
                    FileStream.Write(header, 0, header.Length);
                }
            }

            var bytes = Encoding.UTF8.GetBytes(text);
            FileStream.Write(bytes, 0, bytes.Length);
        }

        public void Dispose() => FileStream?.Dispose();

        public void Flush() => FileStream?.Flush();

        public void InitDirectory(string folderPath)
        {
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
        }

        public void InitFileStream()
        {
            //TODO: as an idea refactor to using()
            FileStream?.Flush();
            FileStream?.Dispose();
            if(IsCrossesMidnight(currentDateTime, DateTime.UtcNow)){
                currentDateTime = DateTime.UtcNow;
                _fullPathToFile = Path.Combine(_folderPath, $"{_fileName}_{currentDateTime:yyyy-MM-dd}.log");
            }
            FileStream = new FileStream(_fullPathToFile, FileMode.Create, FileAccess.Write, FileShare.Read);
        }


        //TODO: move to helper/TimeService
        private bool IsCrossesMidnight(DateTime start, DateTime end)
        {
            return start.TimeOfDay > end.TimeOfDay;
        }
    }
}