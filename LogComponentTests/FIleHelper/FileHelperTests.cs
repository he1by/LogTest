using System.Text;

namespace LogComponent.FileHelper.Tests
{
    public class FileHelperTests : IDisposable
    {
        private string _testFolderPath;
        private string _testFileName;
        private string _testFilePath;

        public FileHelperTests()
        {
            _testFolderPath = Path.Combine(Path.GetTempPath(), "FileHelperTests");
            _testFileName = "testfile";
            _testFilePath = Path.Combine(_testFolderPath, $"{_testFileName}_{DateTime.UtcNow:yyyy-MM-dd}.log");

            if (Directory.Exists(_testFolderPath))
                Directory.Delete(_testFolderPath, true);
        }

        public void Dispose()
        {
            if (Directory.Exists(_testFolderPath))
                Directory.Delete(_testFolderPath, true);
        }

        [Theory]
        [InlineData("Log message 1")]
        [InlineData("Log message 2")]
        [InlineData("Log message 3")]
        public void Write_WhenFileDoesNotExist_ShouldCreateFileWithHeaderAndWriteText(string logMessage)
        {
            // Arrange
            string expectedHeader = "Timestamp               Data\r\n";

            using (var fileHelper = new FileHelper(_testFolderPath, _testFileName))
            {
                // Act
                fileHelper.Write(logMessage);
            }

            // Assert
            Assert.True(File.Exists(_testFilePath));
            string fileContent = File.ReadAllText(_testFilePath);
            Assert.StartsWith(expectedHeader, fileContent);
            Assert.EndsWith(logMessage, fileContent);
        }

        [Theory]
        [InlineData("Log message 1")]
        [InlineData("Log message 2")]
        [InlineData("Log message 3")]
        public void Write_WhenFileExists_ShouldAppendText(string logMessage)
        {
            // Arrange
            string expectedContent = $"Timestamp               Data\r\n\r\n{logMessage}";

            using (var fileHelper = new FileHelper(_testFolderPath, _testFileName))
            {
                // Act
                fileHelper.Write(logMessage);
            }

            // Assert
            Assert.True(File.Exists(_testFilePath));
            string fileContent = File.ReadAllText(_testFilePath);
            Assert.Equal(expectedContent, fileContent);
        }

        [Fact]
        public void InitDirectory_WhenDirectoryDoesNotExist_ShouldCreateDirectory()
        {
            // Arrange
            string nonExistingDirectory = Path.Combine(_testFolderPath, "newfolder");

            using (var fileHelper = new FileHelper(_testFolderPath, _testFileName))
            {
                // Act
                fileHelper.InitDirectory(nonExistingDirectory);
            }

            // Assert
            Assert.True(Directory.Exists(nonExistingDirectory));
        }

        [Fact]
        public void InitDirectory_WhenDirectoryExists_ShouldNotCreateDirectory()
        {
            // Arrange
            string existingDirectory = _testFolderPath;

            using (var fileHelper = new FileHelper(_testFolderPath, _testFileName))
            {
                // Act
                fileHelper.InitDirectory(existingDirectory);
            }

            // Assert
            Assert.True(Directory.Exists(existingDirectory));
        }

        [Fact]
        public void InitFileStream_WhenCalled_ShouldCreateNewFileStream()
        {
            // Arrange
            using (var fileHelper = new FileHelper(_testFolderPath, _testFileName))
            {
                fileHelper.InitFileStream();

                // Act
                var fileStream = fileHelper.FileStream;

                // Assert
                Assert.NotNull(fileStream);
                Assert.True(fileStream.CanWrite);
            }
        }
    }
}