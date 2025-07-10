using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;
using TGramHunt.Contract;
using TGramHunt.Contract.Logging;
using TGramHunt.Data.Repositories.IRepositories;
using TGramHunt.Services.Services;
using TGramHunt.Services.Services.IServices;

namespace TGramHunt.Test.Services
{ 
    [TestClass]
    public class LoggServiceTests
    {
        private Mock<ISystemSettingsService> _systemSettingsServiceMock;
        private Mock<ILoggRepository> _loggRepositoryMock;
        private ILoggService _logService;

        [TestInitialize]
        public void Initialize()
        {
            _systemSettingsServiceMock = new Mock<ISystemSettingsService>();
            _loggRepositoryMock = new Mock<ILoggRepository>();
            _logService = new LoggService(_loggRepositoryMock.Object, _systemSettingsServiceMock.Object);
        }


        [TestMethod]
        public async Task Log_WithNullMessage_ShouldNotCallRepository()
        {
            // Arrange
            string message = null;
            string userName = "John Doe";
            var setting = new SystemSetting { IsLoggingEnabled = true };
            _systemSettingsServiceMock.Setup(m => m.GetSettings()).Returns(setting);

            // Act
            await _logService.Log(message, userName);

            // Assert
            _loggRepositoryMock.Verify(m => m.Log(It.IsAny<LoggingDto>()), Times.Never());
        }

        [TestMethod]
        public async Task Log_WithWhiteSpaceMessage_ShouldNotCallRepository()
        {
            // Arrange
            string message = "   ";
            string userName = "John Doe";
            var setting = new SystemSetting { IsLoggingEnabled = true };
            _systemSettingsServiceMock.Setup(m => m.GetSettings()).Returns(setting);

            // Act
            await _logService.Log(message, userName);

            // Assert
            _loggRepositoryMock.Verify(m => m.Log(It.IsAny<LoggingDto>()), Times.Never());
        }

        [TestMethod]
        public async Task Log_WithLoggingEnabled_ShouldCallRepositoryWithInfoType()
        {
            // Arrange
            string message = "Test message";
            string userName = "John Doe";
            var setting = new SystemSetting { IsLoggingEnabled = true };
            _systemSettingsServiceMock.Setup(m => m.GetSettings()).Returns(setting);

            // Act
            await _logService.Log(message, userName);

            // Assert
            _loggRepositoryMock.Verify(m => m.Log(It.Is<LoggingDto>(x => x.Type == LoggingType.Info && x.Message == message && x.UserName == userName)), Times.Once());
        }

        [TestMethod]
        public async Task Log_WithLoggingDisabled_ShouldNotCallRepository()
        {
            // Arrange
            string message = "Test message";
            string userName = "John Doe";
            var setting = new SystemSetting { IsLoggingEnabled = false };
            _systemSettingsServiceMock.Setup(m => m.GetSettings()).Returns(setting);

            // Act
            await _logService.Log(message, userName);

            // Assert
            _loggRepositoryMock.Verify(m => m.Log(It.IsAny<LoggingDto>()), Times.Never());
        }

        [TestMethod]
        public async Task Log_WithNullException_ShouldNotCallRepository()
        {
            // Arrange
            Exception ex = null;
            string userName = "John Doe";
            var setting = new SystemSetting { IsLoggingEnabled = true };
            _systemSettingsServiceMock.Setup(m => m.GetSettings()).Returns(setting);

            // Act
            await _logService.Log(ex, userName);

            // Assert
            _loggRepositoryMock.Verify(m => m.Log(It.IsAny<LoggingDto>()), Times.Never());
        }
    }
}
