using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TGramHunt.Contract;
using TGramHunt.Contract.Enums;
using TGramHunt.Contract.ViewModels.MainPage;
using TGramHunt.Data.Repositories.IRepositories;
using TGramHunt.Services.Helpers.IHelpers;
using TGramHunt.Services.Services.IServices;

namespace TGramHunt.Services.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly UserManager<User> _userManager;
        private readonly IImageHelper _imageHelper;
        private readonly ISmallFilesService _smallFilesService;
        private readonly IUserVotesForProductsRepositories _userVotesForProductsRepositories;

        private const int IMAGE_SIZE_MAIN = 56;
        private const int IMAGE_SIZE_ABOUT = 104;

        public ProductService(
            IProductRepository productRepository,
            UserManager<User> userManager,
            IImageHelper imageHelper,
            ISmallFilesService smallFilesService,
            IUserVotesForProductsRepositories userVotesForProductsRepositories)
        {
            this._productRepository = productRepository;
            this._userManager = userManager;
            this._imageHelper = imageHelper;
            this._smallFilesService = smallFilesService;
            this._userVotesForProductsRepositories = userVotesForProductsRepositories;
        }

        public async Task<long> GetUserProductCount(Guid guid)
        {
            return await this._productRepository
                .GetUserProductCount(guid);
        }

        public async Task<bool> IsProductExist(string tag)
        {
            var product = await this._productRepository
                .GetByTag(tag.ToLower());

            return product != null;
        }

        public async Task<ProductCreationStatus> CreateProduct(Product product, IFormFile cover, string owner)
        {
            var user = await this._userManager.FindByNameAsync(owner);

            if (user == null || user.IsClosed)
            {
                return ProductCreationStatus.Unauthorized;
            }

            if (cover != null && cover.Length > 0)
            {
                product.ImageIdx56 =
                    await ResizeAndSaveImage(
                        cover,
                        IMAGE_SIZE_MAIN,
                        IMAGE_SIZE_MAIN);
                product.ImageIdx104 =
                    await ResizeAndSaveImage(
                        cover,
                        IMAGE_SIZE_ABOUT,
                        IMAGE_SIZE_ABOUT);
            }

            product.OwnerId = user.Id;
            product.DateOfCreation = DateTime.UtcNow;

            try
            {
                await _productRepository.Create(product);
            }
            catch (MongoWriteException e)
            {
                if (e.WriteError != null &&
                    e.WriteError.Code == 11000 &&
                    e.WriteError.Category == ServerErrorCategory.DuplicateKey)
                {
                    await this._smallFilesService
                        .Delete(product.ImageIdx56);
                    await this._smallFilesService
                        .Delete(product.ImageIdx104);

                    return ProductCreationStatus.Duplicate;
                }
            }

            return ProductCreationStatus.Success;
        }

        private async Task<string?> ResizeAndSaveImage(IFormFile formFile, int width, int height)
        {
            using var ms = new MemoryStream();
            formFile.CopyTo(ms);
            ms.Position = 0;

            return await this._imageHelper.ResizeAndSaveImage(ms.ToArray(), width, height);
        }

        public async Task IncrementVotes(
            string? prodId,
            int count = 1)
        {
            await this._productRepository.IncrementVotes(prodId, count: count);
        }

        public async Task DeleteProduct(string? tag)
        {
            await this._productRepository.Delete(tag);
        }

        public async Task<Product> GetByTag(string tag, Guid? ownerId = null)
        {
            var product = await this._productRepository
                .GetByTag(tag);

            await this.FillWithUserVotes(ownerId,
                new List<ProductsForPeriodViewModel>() {
                    new ProductsForPeriodViewModel()
                    {
                        Products = new List<ProductBase>() { product }
                    }
                });

            return product;
        }

        public async Task<IEnumerable<ProductPicture>> ListProductWithPicture(Guid userId)
        {
            return await this._productRepository
                .ListProductWithPicture(userId);
        }

        public async Task<List<ProductsForPeriodViewModel>> GetFirstProducts(
            ProductListSorting? _sorting,
            bool verifiedOnly,
            ProductCategory? productCategory = null,
            Guid? ownerId = null,
            IEnumerable<string?>? productIds = null,
            Guid? currentUser = null)
        {
            var prods = await this._productRepository.GetFirstProducts(_sorting, verifiedOnly, productCategory, ownerId, productIds);

            await this.FillWithUserVotes(currentUser, prods);
            return prods;
        }

        public async Task<List<ProductsForPeriodViewModel>> GetAfterProducts(DateTime after,
            ProductListSorting? _sorting,
            bool verifiedOnly,
            ProductCategory? productCategory = null,
            Guid? ownerId = null,
            IEnumerable<string?>? productIds = null,
            Guid? currentUser = null)
        {
            var prods = await this._productRepository.GetAfterProducts(after, _sorting, verifiedOnly, productCategory, ownerId, productIds);

            await this.FillWithUserVotes(currentUser, prods);
            return prods;
        }

        private async Task FillWithUserVotes(Guid? ownerId, List<ProductsForPeriodViewModel> prods)
        {
            if (ownerId == null || prods == null || !prods.Any())
            {
                return;
            }

            var listofProds = new Dictionary<string, ProductBase>();
            foreach (var products in prods.Select(x => x.Products))
            {
                if (products == null || !products.Any())
                {
                    continue;
                }

                foreach (var prod in products)
                {
                    if (string.IsNullOrWhiteSpace(prod.Tag))
                    {
                        continue;
                    }

                    listofProds.Add(prod.Tag, prod);
                }
            }

            var keys = listofProds.Keys;
            if (keys == null || !keys.Any())
            {
                return;
            }

            var votes = await _userVotesForProductsRepositories
                .GetAllProductsIdsForUser(ownerId.Value, keys.ToList());

            if (votes == null || !votes.Any())
            {
                return;
            }

            foreach (var prodId in votes.Select(vote => vote.ProductId))
            {
                if (string.IsNullOrEmpty(prodId))
                {
                    continue;
                }

                listofProds[prodId].IsUserVoted = true;
            }
        }
    }
}