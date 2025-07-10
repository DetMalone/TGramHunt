using Microsoft.AspNetCore.Mvc;
using TGramHunt.Contract.Enums;
using TGramHunt.Services.Helpers;

namespace TGramHunt.Helpers
{
    public class CustomUrlHelper
    {
        private readonly IUrlHelper _url;

        public CustomUrlHelper(IUrlHelper urlHelper)
        {
            this._url = urlHelper;
        }

        public string GenerateUrlParameters(
            string handler,
            string after,
            string category,
            string filter,
            string sorting)
        {
            return this._url.Page("", handler, new
            {
                after,
                category,
                filter,
                sorting
            });
        }

        public string GenerateUrlParameters(ProfileFilter? filter, ProductListSorting? sorting)
        {
            return this._url.Page("", "", new
            {
                filter,
                sorting
            });
        }

        public string ProfileIndex()
        {
            return LinkHelper.profileIndex;
        }
    }
}