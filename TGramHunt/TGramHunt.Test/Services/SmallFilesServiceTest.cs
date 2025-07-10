using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TGramHunt.Contract;
using TGramHunt.Data.Repositories.IRepositories;
using TGramHunt.Services.Services;

namespace TGramHunt.Test.Services;

[TestClass]
public class SmallFilesServiceTest
{
    
        private Mock<ISmallFilesRepository> _smallFilesRepositoryMock;
        private SmallFilesService _smallFilesService;

        [TestInitialize]
        public void Initialize()
        {
            _smallFilesRepositoryMock = new Mock<ISmallFilesRepository>();
            _smallFilesService = new SmallFilesService(_smallFilesRepositoryMock.Object);
        }

        [TestMethod]
        public async Task Create_ValidSmallFile_CallsRepositoryCreate()
        {
            // Arrange
            var smallFileDTO = new SmallFile { Id = "123" };

            // Act
            await _smallFilesService.Create(smallFileDTO);

            // Assert
            _smallFilesRepositoryMock.Verify(x => x.Create(smallFileDTO), Times.Once);
        }

        [TestMethod]
        public async Task Delete_ValidId_CallsRepositoryDelete()
        {
            // Arrange
            var id = "123";

            // Act
            await _smallFilesService.Delete(id);

            // Assert
            _smallFilesRepositoryMock.Verify(x => x.Delete(id), Times.Once);
        }

        [TestMethod]
        public async Task Get_ValidId_ReturnsSmallFile()
        {
            // Arrange
            var id = "123";
            var smallFile = new SmallFile { Id = "123" };
            _smallFilesRepositoryMock.Setup(x => x.Get(id)).ReturnsAsync(smallFile);

            // Act
            var result = await _smallFilesService.Get(id);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(smallFile.Id, result.Id);
        }

        [TestMethod]
        public async Task Get_InvalidId_ReturnsNull()
        {
            // Arrange
            var id = "123";
            _smallFilesRepositoryMock.Setup(x => x.Get(id)).ReturnsAsync((SmallFile)null);

            // Act
            var result = await _smallFilesService.Get(id);

            // Assert
            Assert.IsNull(result);
        }
    
}