using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TGramHunt.Contract;
using TGramHunt.Helpers;
using TGramHunt.Services.Services.IServices;

namespace TGramHunt.Services.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IPasswordValidator<User> _passwordValidator;
        private readonly IUserVotesForProductsServices _userVotesForProductsServices;
        private readonly IProductService _productService;
        private readonly ISmallFilesService _smallFilesService;

        public UserService(UserManager<User> userManager,
            SignInManager<User> signInManager,
            RoleManager<Role> roleManager,
            IPasswordValidator<User> passwordValidator,
            IUserVotesForProductsServices _userVotesForProductsServices,
            IProductService _productService,
            ISmallFilesService _smallFilesService)
        {
            this._userManager = userManager;
            this._roleManager = roleManager;
            this._passwordValidator = passwordValidator;
            this._signInManager = signInManager;
            this._userVotesForProductsServices = _userVotesForProductsServices;
            this._productService = _productService;
            this._smallFilesService = _smallFilesService;
        }

        public async Task<User?> GetIfAuthorized(ClaimsPrincipal currentUser)
        {
            if (currentUser != null &&
                this._signInManager.IsSignedIn(currentUser))
            {
                var user = await this.Get(currentUser);
                if (user != null && !user.IsClosed)
                {
                    return user;
                }
            }

            return null;
        }

        public async Task Login(User user, bool staySignIn)
        {
            await _signInManager.SignInAsync(user, true);
        }

        public async Task<User?> Get(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return null;
            }

            return await _userManager.FindByIdAsync(id);
        }

        public async Task<User?> Get(ClaimsPrincipal currentUser)
        {
            if (currentUser == null)
            {
                return null;
            }

            return await _userManager.GetUserAsync(currentUser);
        }

        public async Task<IdentityResult> Update(User user)
        {
            return await _userManager.UpdateAsync(user);
        }

        public async Task Remove(User user)
        {
            var votes = await this._userVotesForProductsServices.GetAllVotesForUser(user.Id);

            var products = await this._productService.ListProductWithPicture(user.Id) ?? new List<ProductPicture>();

            if (votes != null)
            {
                await this._userVotesForProductsServices
                    .DeleteUserVotes(user.Id);

                var prodIds = votes
                    // we will remove those products later
                    .Where(vote => !products.Any(x => string.Equals(x.Tag, vote.ProductId, StringComparison.OrdinalIgnoreCase)))
                    .Select(vote => vote.ProductId);

                foreach (var prodId in prodIds)
                {
                    await this._productService
                        .IncrementVotes(prodId, -1);
                }
            }

            if (products.Any())
            {
                var tasks = new List<Task>();
                foreach (var product in products)
                {
                    tasks.Add(this._userVotesForProductsServices
                        .DeleteProductVotes(product.Tag));
                    tasks.Add(this._smallFilesService
                        .Delete(product.ImageIdx56));
                    tasks.Add(this._smallFilesService
                        .Delete(product.ImageIdx104));
                    tasks.Add(this._productService
                        .DeleteProduct(product.Tag));
                }

                await Task.WhenAll(tasks);
            }

            await this._smallFilesService
                .Delete(user.PictureIdx41);
            await this._smallFilesService
                .Delete(user.PictureIdx100);

            await _userManager.DeleteAsync(user);
        }

        public async Task<string?> Register(User user, string password, string role)
        {
            var result = await BaseRegister(user, password, role);
            if (!string.IsNullOrEmpty(result))
            {
                return result;
            }

            return null;
        }

        public async Task<string?> BaseRegister(User user, string password, string role)
        {
            var validResult = await _passwordValidator.ValidateAsync(_userManager, user, password);

            if (!validResult.Succeeded)
            {
                return "Not Valid Password";
            }

            var roleExist = await _roleManager.FindByNameAsync(role);
            if (roleExist == null)
            {
                await _roleManager.CreateAsync(new Role { Name = Constants.Users, NormalizedName = Constants.Users.ToUpper() });
            }

            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                return result?.Errors?.ToString() ?? string.Empty;
            }

            var roleResult = await _userManager.AddToRoleAsync(user, role);
            if (!roleResult.Succeeded)
            {
                return "User Assign Role Error";
            }

            return null;
        }
    }
}