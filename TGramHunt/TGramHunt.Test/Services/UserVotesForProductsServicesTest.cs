using Moq;
using TGramHunt.Contract;
using TGramHunt.Data.Repositories.IRepositories;
using TGramHunt.Services.Services;

namespace TGramHunt.Test.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class UserVotesForProductsServicesTest
{
    
        private Mock<IUserVotesForProductsRepositories> _mockRepository;
        private UserVotesForProductsServices _userVotesForProductsServices;

        [TestInitialize]
        public void Initialize()
        {
            _mockRepository = new Mock<IUserVotesForProductsRepositories>();
            _userVotesForProductsServices = new UserVotesForProductsServices(_mockRepository.Object);
        }

        [TestMethod]
        public async Task GetCountForUser_Returns_Count_From_Repository()
        {
            // Arrange
            Guid userId = Guid.NewGuid();
            long expectedCount = 10;
            _mockRepository.Setup(x => x.GetCountForUser(userId)).ReturnsAsync(expectedCount);

            // Act
            long actualCount = await _userVotesForProductsServices.GetCountForUser(userId);

            // Assert
            Assert.AreEqual(expectedCount, actualCount);
        }

        [TestMethod]
        public async Task AddVote_Calls_AddVote_On_Repository_And_Returns_Result()
        {
            // Arrange
            UserVotesForProducts userVotesForProducts = new UserVotesForProducts();
            bool expected = true;
            _mockRepository.Setup(x => x.AddVote(userVotesForProducts)).ReturnsAsync(expected);

            // Act
            bool actual = await _userVotesForProductsServices.AddVote(userVotesForProducts);

            // Assert
            _mockRepository.Verify(x => x.AddVote(userVotesForProducts), Times.Once());
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public async Task RemoveVote_Calls_RemoveVote_On_Repository_And_Returns_Result()
        {
            // Arrange
            Guid userId = Guid.NewGuid();
            string productId = "123";
            bool expected = true;
            _mockRepository.Setup(x => x.RemoveVote(userId, productId)).ReturnsAsync(expected);

            // Act
            bool actual = await _userVotesForProductsServices.RemoveVote(userId, productId);

            // Assert
            _mockRepository.Verify(x => x.RemoveVote(userId, productId), Times.Once());
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public async Task DeleteUserVotes_Calls_DeleteUserVotes_On_Repository()
        {
            // Arrange
            Guid userId = Guid.NewGuid();

            // Act
            await _userVotesForProductsServices.DeleteUserVotes(userId);

            // Assert
            _mockRepository.Verify(x => x.DeleteUserVotes(userId), Times.Once());
        }

        [TestMethod]
        public async Task DeleteProductVotes_Calls_DeleteProductVotes_On_Repository()
        {
            // Arrange
            string productId = "123";

            // Act
            await _userVotesForProductsServices.DeleteProductVotes(productId);

            // Assert
            _mockRepository.Verify(x => x.DeleteProductVotes(productId), Times.Once());
        }
    
}