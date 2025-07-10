using AutoMapper;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Linq;
using System.Threading.Tasks;
using TGramHunt.Contract;
using TGramHunt.Contract.Exceptions;
using TGramHunt.Contract.ViewModels.EditProfile;
using TGramHunt.Helpers;
using TGramHunt.Models;
using TGramHunt.Services.Helpers;
using TGramHunt.Services.Helpers.IHelpers;
using TGramHunt.Services.Services.IServices;
using TGramHunt.Validators;

namespace TGramHunt.Pages.EditProfile
{
    [Authorize]
    public class IndexModel : BasePageModel
    {
        private readonly IUserService _userService;
        private readonly LogOutHelper _logOutHelper;
        private readonly IMapper _mapper;
        private readonly IImageHelper _imageHelper;
        private readonly ISmallFilesService _smallFilesService;
        private const int IMAGE_SIZE_PROFILEx41 = 41;
        private const int IMAGE_SIZE_PROFILEx100 = 100;

        public IndexModel(
            IUserService userService,
            IMapper mapper,
            IImageHelper imageHelper,
            ISmallFilesService _smallFilesService,
            LogOutHelper _logOutHelper)
        {
            this._userService = userService;
            this._mapper = mapper;
            this._imageHelper = imageHelper;
            this._smallFilesService = _smallFilesService;
            this._logOutHelper = _logOutHelper;
        }

        public ProfileViewModel Profile { get; set; }

        public async Task<IActionResult> OnGet()
        {
            var res = base.TestHanler();
            if (res != null)
            {
                return res;
            }

            if (User == null)
            {
                return Redirect("/");
            }

            var user = await _userService.Get(User);
            if (user == null)
            {
                return Redirect("/");
            }

            Profile = _mapper.Map<User, ProfileViewModel>(user);
            return Page();
        }

        public async Task<IActionResult> OnGetCheckAccess()
        {
            //ignore test handler
            if (User == null)
            {
                return new JsonResult(false);
            }

            var user = await _userService.Get(User);
            if (user == null)
            {
                return new JsonResult(false);
            }

            return new JsonResult(true);
        }

        public async Task<IActionResult> OnPostImageSave(string imgBase64)
        {
            if (string.IsNullOrWhiteSpace(imgBase64) ||
                !imgBase64.Contains(','))
            {
                return BadRequest();
            }

            var user = await _userService.GetIfAuthorized(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var secondPart = imgBase64.Split(',')[1];
            var bytes = new byte[(secondPart.Length * 3) / 4];

            if (!Convert.TryFromBase64String(secondPart, bytes, out _))
            {
                return BadRequest();
            }

            var badRequestText = "Invalid image.";

            var pictureIdx41 = await _imageHelper.ResizeAndSaveImage(bytes, IMAGE_SIZE_PROFILEx41, IMAGE_SIZE_PROFILEx41);
            if (pictureIdx41 == null)
            {
                return BadRequest(badRequestText);
            }
            await this._smallFilesService
                .Delete(user.PictureIdx41);
            user.PictureIdx41 = pictureIdx41;

            var pictureIdx100 = await _imageHelper.ResizeAndSaveImage(bytes, IMAGE_SIZE_PROFILEx100, IMAGE_SIZE_PROFILEx100);
            if (pictureIdx100 == null)
            {
                return BadRequest(badRequestText);
            }
            await this._smallFilesService
                .Delete(user.PictureIdx100);
            user.PictureIdx100 = pictureIdx100;
            user.PictureCache++;

            await _userService.Update(user);

            return new OkObjectResult(new
            {
                Link = LinkHelper.ImageRelativLink(user.PictureIdx100,
                user.PictureCache,
                user.Picture)
            });
        }

        public async Task<IActionResult> OnPostCloseAccount()
        {
            var user = await _userService.Get(User);

            if (user == null)
            {
                return BadRequest();
            }

            user.IsClosed = true;
            await _userService.Update(user);
            await _userService.Remove(user);
            await _logOutHelper.Logout(HttpContext);

            return StatusCode(204);
        }

        public async Task<IActionResult> OnPostSave(ProfileViewModelBase model)
        {
            this.PrepareNameProperty(model);

            ModelState.Clear();
            model.Name = model.Name?.Trim();
            var validator = new ProfileViewModelBaseValidator();
            var results = validator.Validate(model);
            results.AddToModelState(ModelState, null);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userService.GetIfAuthorized(User);
            if (user == null)
            {
                return Unauthorized();
            }

            user.Name = model.Name.Trim();
            await _userService.Update(user);
            return StatusCode(204);
        }

        private void PrepareNameProperty(ProfileViewModelBase model)
        {
            var propName = nameof(model.Name);
            if (ModelState.ContainsKey(propName) &&
                ModelState[propName].ValidationState == ModelValidationState.Invalid &&
                ModelState[propName].Errors != null)
            {
                var error = ModelState[propName].Errors.FirstOrDefault(x =>
                    x.Exception is SanitizedException)?.Exception;

                if (error is SanitizedException sanitizedException)
                {
                    //even if it invalid after sanitizer we still need to show up appropriate message
                    model.Name = sanitizedException.RawValue;
                }
            }

            if (string.IsNullOrWhiteSpace(model.Name))
            {
                return;
            }

            model.Name = model.Name.Trim();
            var ind = model.Name.IndexOf(' ');
            if (ind != -1)
            {
                var name = model.Name;
                model.Name = name[..ind].Trim() + " " + name[ind..].Trim();
            }
        }
    }
}
