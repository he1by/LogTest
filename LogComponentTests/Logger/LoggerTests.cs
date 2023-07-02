namespace LogComponent.Tests
{
    public class LoggerTests
    {
        [Theory]
        [InlineData("Log message 1")]
        [InlineData("Log message 2")]
        [InlineData("Log message 3")]
        public void WriteLog_WhenCalled_ShouldEnqueueLog(string logMessage)
        {
            // Arrange
            var fileHelperMock = new Mock<IFileHelper>();
            var logger = new Logger(fileHelperMock.Object);
            // Act
            logger.WriteLog(logMessage);

            // Assert
            Assert.True(logger.Logs.TryPeek(out var log));
            Assert.Contains(logMessage, log);
        }

        [Theory]
        [InlineData("Log message 1")]
        [InlineData("Log message 2")]
        [InlineData("Log message 3")]
        public void Stop_WithFlushFalse_ShouldCancelSoftCTS_WaitForRunner_FlushWriter(string logMessage)
        {
            // Arrange
            var fileHelperMock = new Mock<IFileHelper>();
            var logger = new Logger(fileHelperMock.Object);
            logger.WriteLog(logMessage);

            // Act
            logger.Stop(false);

            // Assert
            fileHelperMock.Verify(m => m.Flush(), Times.Once);
        }
    }
}
