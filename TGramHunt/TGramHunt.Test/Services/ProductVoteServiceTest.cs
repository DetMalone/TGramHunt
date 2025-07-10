using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TGramHunt.Contract.Enums;
using TGramHunt.Contract.ViewModels.MainPage;
using TGramHunt.Services.Services;
using TGramHunt.Services.Services.IServices;

namespace TGramHunt.Test.Services;

[TestClass]
public class ProductVoteServiceTest
{
        private Mock<IUserVotesForProductsServices> _userVotesForProductsServicesMock;
        private Mock<IProductService> _productServiceMock;
        private IProductVoteService _productVoteService;
        

        [TestInitialize]
        public void Initialize()
        {
            _userVotesForProductsServicesMock = new Mock<IUserVotesForProductsServices>();
            _productServiceMock = new Mock<IProductService>();
            _productVoteService = new ProductVoteService(_userVotesForProductsServicesMock.Object, _productServiceMock.Object);
        }

        [TestMethod]
        public async Task GetProductsVotedByUser_WithNoProductIds_ReturnsEmptyList()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _userVotesForProductsServicesMock.Setup(x => x.GetAllVotesForUser(userId, null)).ReturnsAsync(new List<Contract.UserVotesForProducts>());

            // Act
            var result = await _productVoteService.GetProductsVotedByUser(userId, null, false);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public async Task GetAfterProductsVotedByUser_WithNoProductIds_ReturnsEmptyList()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _userVotesForProductsServicesMock.Setup(x => x.GetAllVotesForUser(userId, null)).ReturnsAsync(new List<Contract.UserVotesForProducts>());

            // Act
            var result = await _productVoteService.GetAfterProductsVotedByUser(DateTime.Now, userId, null, false);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }
        
        [TestMethod]
        public async Task GetAfterProductsVotedByUser_WithProductIds_ReturnsProducts()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var productIds = new List<string> { "true", "false" };
            var after = DateTime.Now.AddDays(-1);
            _userVotesForProductsServicesMock.Setup(x => x.GetAllVotesForUser(userId, null)).ReturnsAsync(productIds.Select(id => new Contract.UserVotesForProducts { ProductId = id }).ToList());
            _productServiceMock.Setup(x => x.GetAfterProducts(after, null, false, null, null, productIds, userId)).ReturnsAsync(new List<ProductsForPeriodViewModel>
            {
                new ProductsForPeriodViewModel { Products = {}, IsShowDate = true },
                new ProductsForPeriodViewModel { Products = {}, IsShowDate = false },
            });

            // Act
            var result = await _productVoteService.GetAfterProductsVotedByUser(after, userId, null, false);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
        }
 
        [TestMethod]
        public async Task GetProductsVotedByUser_WithProductIds_ReturnsProducts()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var productIds = new List<string> { "true", "false" };
            _userVotesForProductsServicesMock.Setup(x => x.GetAllVotesForUser(userId, null))
                .ReturnsAsync(productIds.Select(id => new Contract.UserVotesForProducts { ProductId = id }).ToList());
            _productServiceMock.Setup(x => x.GetFirstProducts(null, false, null, null, productIds, userId))
                .ReturnsAsync(new List<ProductsForPeriodViewModel>
                {
                    new ProductsForPeriodViewModel { Products = { }, IsShowDate = true },
                    new ProductsForPeriodViewModel { Products = { }, IsShowDate = false },
                });

            // Act
            var result = await _productVoteService.GetProductsVotedByUser(userId, null, false);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        public async Task GetAfterProductsVotedByUser_WithProductIdsAndSorting_ReturnsSortedProducts()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var productIds = new List<string> { "true", "false" };
            var after = DateTime.Now.AddDays(-1);
            _userVotesForProductsServicesMock.Setup(x => x.GetAllVotesForUser(userId, null))
                .ReturnsAsync(productIds.Select(id => new Contract.UserVotesForProducts { ProductId = id }).ToList());
            _productServiceMock
                .Setup(x => x.GetAfterProducts(after, ProductListSorting.newest, false, null, null, productIds, userId))
                .ReturnsAsync(new List<ProductsForPeriodViewModel>
                {
                    new ProductsForPeriodViewModel { Products = { }, IsShowDate = true },
                    new ProductsForPeriodViewModel { Products = { }, IsShowDate = false },
                });

            // Act
            var result =
                await _productVoteService.GetAfterProductsVotedByUser(after, userId, ProductListSorting.newest, false);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
        }
        

}