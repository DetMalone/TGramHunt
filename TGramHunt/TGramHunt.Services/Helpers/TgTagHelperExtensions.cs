using HtmlAgilityPack;
using System;
using System.Threading.Tasks;
using TGramHunt.Contract.Enums;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using TGramHunt.Contract;
using TGramHunt.Services.Helpers.IHelpers;

namespace TGramHunt.Services.Helpers
{
    public class TgTagHelperExtensions : ITgTagHelperExtensions
    {
        public TagResult checkIfEmpty(string url, bool getInfo)
        {

            TagResult _productInfo = new();

            _productInfo.isEmpty = true;

            HtmlWeb htmlWeb = new();

            var htmlDoc = htmlWeb.Load(url);

            //using XPath to get the nodes of HTML
            var robotNode = htmlDoc.DocumentNode.SelectSingleNode("//meta[@name='robots']");

            //if 'robots' node is present, the product is either private or does not exist
            if (robotNode is null) _productInfo.isEmpty = false;

            if (robotNode is null && getInfo)
            {
                
                _productInfo.Name = htmlDoc.DocumentNode.SelectSingleNode("//meta[@property='og:title']").GetAttributeValue("content", "Name not found");
                _productInfo.Description = htmlDoc.DocumentNode.SelectSingleNode("//meta[@property='og:description']").GetAttributeValue("content", "Name not found");

                _productInfo.isEmpty = false;

                //only groups and channels have page_description
                var categoryNode = htmlDoc.DocumentNode.SelectSingleNode("//div[@class='tgme_page_description']");

                if (categoryNode is null)
                {
                    _productInfo.Category = (ProductCategory)5;
                }
                else
                {
                    //checking if it's a group or a channel by page_extra content
                    var userCountNode = htmlDoc.DocumentNode.SelectSingleNode("//div[@class='tgme_page_extra']").InnerText;
                    _productInfo.Category = userCountNode.Contains("subscriber") ? (ProductCategory)1
                                                                                 : (ProductCategory)2;
                }
            }

            return _productInfo;
        }

        public async Task<TagResult> checkSticker(string tag, HttpClient httpClient, string apiUrl, bool getInfo)
        {
            TagResult _productInfo = new();

            _productInfo.isEmpty = true;

            string urlParameters = "getStickerSet?name=" + tag;

            httpClient.BaseAddress = new Uri(apiUrl);


            HttpResponseMessage response = await httpClient.GetAsync(urlParameters);

            if (response.IsSuccessStatusCode)
            {
                _productInfo.isEmpty = false;

                if (getInfo)
                {

                    string stickerInfo = await response.Content.ReadAsStringAsync();

                    JObject stickerSet = JObject.Parse(stickerInfo);

                    _productInfo.Name = (string)stickerSet["result"]["title"];
                    _productInfo.Category = (ProductCategory)3;
                }
            }

            //if response is ok the stickerpack exists
            return _productInfo;
        }
    }
}
