using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;
using NSubstitute;
using TGramHunt.Contract;
using TGramHunt.Contract.Enums;
using TGramHunt.Contract.ViewModels.MainPage;
using TGramHunt.Data.Repositories.IRepositories;
using TGramHunt.Services.Helpers.IHelpers;
using TGramHunt.Services.Services;
using TGramHunt.Services.Services.IServices;
using TGramHunt.Test.WebServices.AuthControllerTests;

namespace TGramHunt.Services.Test.Services.ProductServiceTests
{
    [TestClass]
    public class ProductServiceTests
    {
        #region Constructor
        public ProductServiceTests()
        {
            _productRepository = Substitute.For<IProductRepository>();
            _userManager = Substitute.For<FakeUserManager>();
            _imageHelper = Substitute.For<IImageHelper>();
            _smallFilesService = Substitute.For<ISmallFilesService>();
            _userVotesForProductsRepositories = Substitute.For<IUserVotesForProductsRepositories>();
            _cover = Substitute.For<IFormFile>();
            _product = Substitute.For<Product>();
            _productService = new ProductService(
                _productRepository,
                _userManager,
                _imageHelper,
                _smallFilesService,
                _userVotesForProductsRepositories);
        }

        #endregion

        #region Test methods

        [DataTestMethod]
        [DynamicData(nameof(UnauthorizedUsers))]
        public async Task TestCreateProduct_Unauthorized(User user, string owner)
        {
            // Arrange
            _userManager.FindByNameAsync(owner).Returns(user);

            // Act
            var result = await _productService.CreateProduct(_product, _cover, owner);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ProductCreationStatus));
            Assert.AreEqual(result, ProductCreationStatus.Unauthorized);
        }

        [DataTestMethod]
        [DataRow(1, "owner")]
        public async Task TestCreateProduct_CoverResizedIfSpecified(int converLength, string owner)
        {
            // Arrange
            var user = new User();
            _userManager.FindByNameAsync(Arg.Any<string>()).Returns(user);
            _cover.Length.Returns(converLength);

            // Act
            await _productService.CreateProduct(_product, _cover, owner);

            // Assert
            await _imageHelper.Received(1).ResizeAndSaveImage(Arg.Any<byte[]>(), IMAGE_SIZE_MAIN, IMAGE_SIZE_MAIN);
            await _imageHelper.Received(1).ResizeAndSaveImage(Arg.Any<byte[]>(), IMAGE_SIZE_ABOUT, IMAGE_SIZE_ABOUT);
        }

        [TestMethod]
        [DataRow("owner")]
        public async Task TestCreateProduct_Success(string owner)
        {
            // Arrange
            var user = new User();
            _userManager.FindByNameAsync(Arg.Any<string>()).Returns(user);
            _productRepository.Create(Arg.Any<Product>()).Returns(Task.CompletedTask);

            // Act
            var result = await _productService.CreateProduct(_product, _cover, owner);

            // Assert
            await _productRepository.Received().Create(_product);
            Assert.IsInstanceOfType(result, typeof(ProductCreationStatus));
            Assert.AreEqual(ProductCreationStatus.Success, result);
        }

        [DataTestMethod]
        [DataRow(null, DisplayName = "null input")]
        [DataRow("")]
        [DataRow("@tag")]
        public async Task TestDeleteProduct(string? tag)
        {
            // Arrange
            _productRepository.Delete(tag).Returns(Task.CompletedTask);

            // Act
            await _productService.DeleteProduct(tag);

            // Assert
            await _productRepository.Received(1).Delete(tag);
        }

        [DataTestMethod]
        [DynamicData(nameof(ProductTagsUserVotes), DynamicDataSourceType.Method)]
        public async Task TestGetAfterProducts(List<string> allTags, List<string> userVotes)
        {
            // Arrange
            DateTime after = DateTime.Parse("01/01/2023 00:00:00");
            var productsForPeriodVM = _PrepareProductsForPeriodNotVotedVM(allTags);
            var votes = _PrepareUserVotesForProduct(userVotes);

            _productRepository
                .GetAfterProducts(after, default, default, default, Guid.Empty, default)
                .Returns(productsForPeriodVM);

            _userVotesForProductsRepositories
                .GetAllProductsIdsForUser(Arg.Any<Guid>(), Arg.Any<List<string>>()).Returns(votes);

            // Act
            var result = await _productService.GetAfterProducts(after, default, default, default, Guid.Empty, default, Guid.Empty);
            var resultProducts = result.First().Products;
            var productsWithVotes = resultProducts?.Where(x => votes.Any(v => v.ProductId == x.Tag)).ToList();
            var productsWithoutVotes = resultProducts?.Where(x => !votes.Any(v => v.ProductId == x.Tag)).ToList();

            // Assert
            await _productRepository.Received(1).GetAfterProducts(after, default, default, default, Guid.Empty, default);
            await _userVotesForProductsRepositories.Received(1).GetAllProductsIdsForUser(Guid.Empty, Arg.Any<List<string>>());
            Assert.IsNotNull(result);
            Assert.IsNotNull(resultProducts);
            Assert.IsNotNull(productsWithVotes);
            Assert.IsNotNull(productsWithoutVotes);
            Assert.IsTrue(productsWithVotes.All(x => x.IsUserVoted));
            Assert.IsTrue(productsWithoutVotes.All(x => !x.IsUserVoted));
        }

        [TestMethod]
        public async Task TestGetAfterProducts_NoCurrentUser()
        {
            // Arrange
            var after = DateTime.Parse("01/01/2023 00:00:00");
            Guid? currentUser = null;

            // Act
            await _productService.GetAfterProducts(after, default, default, default, Guid.Empty, default, currentUser);

            // Assert
            await _userVotesForProductsRepositories.DidNotReceive().GetAllProductsIdsForUser(Arg.Any<Guid>(), Arg.Any<List<string>>());
        }

        [DataTestMethod]
        [DynamicData(nameof(ProductTagUserVotes), DynamicDataSourceType.Method)]
        public async Task TestGetByTag(string tag, List<string> tagsWithVotes, bool isUserVotedExpected)
        {
            // Arrange
            var product = new Product() { Tag = tag };
            var votes = _PrepareUserVotesForProduct(tagsWithVotes);

            _productRepository
                .GetByTag(tag, Arg.Any<IClientSessionHandle>())
                .Returns(product);

            _userVotesForProductsRepositories
                .GetAllProductsIdsForUser(Arg.Any<Guid>(), Arg.Any<List<string>>()).Returns(votes);

            // Act
            var result = await _productService.GetByTag(tag, Guid.Empty);

            // Assert
            await _productRepository.Received(1).GetByTag(tag);
            await _userVotesForProductsRepositories.Received(1).GetAllProductsIdsForUser(Arg.Any<Guid>(), Arg.Any<List<string>>());
            Assert.AreEqual(isUserVotedExpected, result.IsUserVoted);
        }

        [TestMethod]
        public async Task TestGetByTag_NoCurrentUser()
        {
            // Arrange
            Guid? currentUser = null;

            // Act
            await _productService.GetByTag("@tag", currentUser);

            // Assert
            await _userVotesForProductsRepositories.DidNotReceive().GetAllProductsIdsForUser(Arg.Any<Guid>(), Arg.Any<List<string>>());
        }

        [DataTestMethod]
        [DynamicData(nameof(ProductTagsUserVotes), DynamicDataSourceType.Method)]
        public async Task TestGetFirstProducts(
            List<string> allTags,
            List<string> tagsWithVotes)
        {
            // Arrange
            var productsVMFromRepository = _PrepareProductsForPeriodNotVotedVM(allTags);
            _productRepository.GetFirstProducts(
                Arg.Any<ProductListSorting?>(),
                Arg.Any<bool>(),
                Arg.Any<ProductCategory?>(),
                Arg.Any<Guid?>(),
                Arg.Any<IEnumerable<string?>?>()).Returns(productsVMFromRepository);

            var votes = _PrepareUserVotesForProduct(tagsWithVotes);
            _userVotesForProductsRepositories
                .GetAllProductsIdsForUser(Arg.Any<Guid>(), Arg.Any<List<string>>()).Returns(votes);

            // Act
            var result = await _productService.GetFirstProducts(default, default, default, Guid.Empty, default, Guid.Empty);
            var resultProducts = result.First().Products;
            var productsWithVotes = resultProducts?.Where(x => votes.Any(v => v.ProductId == x.Tag)).ToList();
            var productsWithoutVotes = resultProducts?.Where(x => !votes.Any(v => v.ProductId == x.Tag)).ToList();

            // Assert
            await _productRepository.Received(1).GetFirstProducts(default, default, default, Guid.Empty, default);
            await _userVotesForProductsRepositories.Received().GetAllProductsIdsForUser(Arg.Any<Guid>(), Arg.Any<List<string>>());

            Assert.IsNotNull(result);
            Assert.IsNotNull(resultProducts);
            Assert.IsNotNull(productsWithVotes);
            Assert.IsNotNull(productsWithoutVotes);
            Assert.IsTrue(productsWithVotes.All(x => x.IsUserVoted));
            Assert.IsTrue(productsWithoutVotes.All(x => !x.IsUserVoted));
        }

        [TestMethod]
        public async Task TestGetFirstProducts_NoCurrentUser()
        {
            // Arrange
            Guid? currentUser = null;

            // Act
            await _productService.GetFirstProducts(default, default, default, Guid.Empty, default, currentUser);

            // Assert
            await _userVotesForProductsRepositories.DidNotReceive().GetAllProductsIdsForUser(Arg.Any<Guid>(), Arg.Any<List<string>>());
        }

        [DataTestMethod]
        [DataRow(1)]
        public async Task TestGetUserProductCount(long productCount)
        {
            // Arrange
            _productRepository.GetUserProductCount(Arg.Any<Guid>()).Returns(productCount);

            // Act
            var result = await _productService.GetUserProductCount(Guid.Empty);

            // Assert
            Assert.AreEqual(productCount, result);
        }

        [DataTestMethod]
        [DataRow("@tag", 1)]
        public async Task TestIncrementVotes(string? prodId, int count)
        {
            // Arrange

            // Act
            await _productService.IncrementVotes(prodId, count);

            // Assert
            await _productRepository.Received(1).IncrementVotes(prodId, null, null, count);

        }

        [DataTestMethod]
        [DynamicData(nameof(IsProductExistData), DynamicDataSourceType.Method)]
        public async Task TestIsProductExist(string tag, Product product, bool expectedExists)
        {
            // Arrange
            _productRepository.GetByTag(Arg.Any<string>()).Returns(product);

            // Act
            var result = await _productService.IsProductExist(tag);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedExists, result);
        }

        [TestMethod]
        public async Task TestListProductWithPicture()
        {
            // Arrange
            var userid = Guid.Empty;

            // Act
            await _productService.ListProductWithPicture(userid);

            // Assert
            await _productRepository.Received().ListProductWithPicture(userid);
        }

        #endregion

        #region DynamicData

        private static IEnumerable<object?[]> UnauthorizedUsers
        {
            get
            {
                yield return new object?[] { new User() { Name = "username", IsClosed = true }, "owner" };
                yield return new object?[] { null, "owner" };
            }
        }

        private static IEnumerable<object[]> ProductTagsUserVotes()
        {
            // first - all products, second - with user votes
            yield return new object[] { new List<string>() { "@tag1" }, new List<string>() { } };
            yield return new object[] { new List<string>() { "@tag1", "@tag2" }, new List<string>() { "@tag1" } };
            yield return new object[] { new List<string>() { "@tag1", "@tag2" }, new List<string>() { "@tag1", "@tag2" } };
        }

        private static IEnumerable<object[]> ProductTagUserVotes()
        {
            // first - single product, second - with user votes, third - mark indicating whether votes contains tag
            yield return new object[] { "@tag1", new List<string>() { }, false };
            yield return new object[] { "@tag1", new List<string>() { "@tag1" }, true };
        }

        private static IEnumerable<object?[]> IsProductExistData()
        {
            // first - single product, second - with user votes, third - mark indicating whether votes contains tag
            var tag = "@tag";
            yield return new object?[] { tag, new Product() { Tag = tag }, true };
            yield return new object?[] { tag, null, false };
        }


        #endregion

        #region Constants

        private const int IMAGE_SIZE_MAIN = 56;

        private const int IMAGE_SIZE_ABOUT = 104;

        #endregion

        #region Private methods

        private static List<ProductsForPeriodViewModel> _PrepareProductsForPeriodNotVotedVM(List<string> productTags)
        {
            var result = new List<ProductsForPeriodViewModel>();
            productTags.ForEach(tag => result.Add(
                new ProductsForPeriodViewModel()
                {
                    Products = new List<ProductBase>()
                       {
                            new ProductBase(){ Tag = tag, IsUserVoted = false }
                       }
                }));
            return result;
        }

        private static List<UserVotesForProducts> _PrepareUserVotesForProduct(List<string> votedTags)
        {
            var result = new List<UserVotesForProducts>();
            votedTags.ForEach(tag => result.Add(new UserVotesForProducts() { ProductId = tag }));
            return result;
        }


        #endregion

        #region Private fields

        private readonly IProductRepository _productRepository;
        private readonly UserManager<User> _userManager;
        private readonly IImageHelper _imageHelper;
        private readonly ISmallFilesService _smallFilesService;
        private readonly IUserVotesForProductsRepositories _userVotesForProductsRepositories;
        private readonly IFormFile _cover;
        private readonly Product _product;
        private readonly ProductService _productService;

        #endregion
    }
}
