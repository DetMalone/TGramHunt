using System.Net.Http;
using System.Threading.Tasks;
using TGramHunt.Services.Helpers.IHelpers;
using Microsoft.Extensions.Options;
using TGramHunt.Configurations;

using TGramHunt.Contract;

namespace TGramHunt.Services.Helpers
{
    public class TgTagHelper : ITgTagHelper
    {
        private readonly AppSettings _appSettings;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ITgTagHelperExtensions _tagHelperExt;

    public TgTagHelper (IHttpClientFactory httpClientFactory, IOptions<AppSettings> appSettings, ITgTagHelperExtensions tagHelperExt)
        {
            this._appSettings = appSettings.Value;
            this._httpClientFactory = httpClientFactory;
            this._tagHelperExt = tagHelperExt; 
            //don't collect info by default
        }

        public async Task<TagResult> CheckTag(string tag, bool getInfo)
        {
            //removing the @ symbol
            string _tag = tag.Remove(0, 1);

            string fullUrl = _appSettings.TelegramUserUrl + _tag;

            var httpClient = _httpClientFactory.CreateClient();

            //first try to resolve username via telegram.me
            TagResult _productInfo = _tagHelperExt.checkIfEmpty(fullUrl, getInfo);


            if (!_productInfo.isEmpty) return _productInfo;
            else
            {
                //if user is not found, try to resolve tag as a stickerset via API
                _productInfo = await _tagHelperExt.checkSticker(_tag, httpClient, _appSettings.apiURL, getInfo);
                return _productInfo;
            }
        }
    }
}
