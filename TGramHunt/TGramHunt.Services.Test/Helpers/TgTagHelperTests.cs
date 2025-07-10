using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using TGramHunt.Configurations;
using TGramHunt.Services.Helpers;
using Microsoft.Extensions.Options;
using TGramHunt.Services.Helpers.IHelpers;
using TGramHunt.Contract;

namespace TGramHunt.Services.Test.Helpers
{
    [TestClass]
    public class TgTagHelperTests
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IOptions<AppSettings> _fakeSettings;
        private readonly ITgTagHelperExtensions _tgTagExt;
        private readonly TgTagHelper _tagHelper;

        public TgTagHelperTests()
        {
            //creating a configuration for mocking AppSettings
            AppSettings appSettings = new AppSettings() { 
                apiURL = "https://api.telegram.org/bot5993770949:AAEqwFuLiGRNao8afQhH6BG06QUF-8ao5iY/",
                TelegramUserUrl = "https://telegram.me/"
            };

            _fakeSettings = Substitute.For<IOptions<AppSettings>>();

            _fakeSettings.Value.Returns(appSettings);

            _httpClientFactory = Substitute.For<IHttpClientFactory>();

            _tgTagExt = Substitute.For<ITgTagHelperExtensions>();

            _tagHelper = new TgTagHelper(_httpClientFactory, _fakeSettings, _tgTagExt);

        }

        [TestMethod]
        public async Task TestCheckTag_ShouldReturnEmpty_IfNoInfo()
        {
            //arrange
            TagResult _fakeProduct = new TagResult();

            string randomTag = @"@test";
            _fakeProduct.isEmpty = true;

            _tgTagExt.checkIfEmpty(Arg.Any<String>(), false).Returns(_fakeProduct);
            _tgTagExt.checkSticker(Arg.Any<String>(), Arg.Any<HttpClient>(), Arg.Any<String>(), false).Returns(_fakeProduct);

            //act
            TagResult result = await _tagHelper.CheckTag(randomTag, false);

            //assert
            Assert.IsInstanceOfType(result, typeof(TagResult));
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.isEmpty);
            Assert.AreEqual(true, result.isEmpty);

        }

        [TestMethod]
        public async Task TestCheckTag_ShouldReturnNonEmpty_IfTagExists()
        {
            //arrange
            TagResult _fakeProduct = new TagResult();

            string randomTag = @"@test";
            _fakeProduct.isEmpty = false;

            _tgTagExt.checkIfEmpty(Arg.Any<String>(), false).Returns(_fakeProduct);
            _tgTagExt.checkSticker(Arg.Any<String>(), Arg.Any<HttpClient>(), Arg.Any<String>(), false).Returns(_fakeProduct);

            //act
            TagResult result = await _tagHelper.CheckTag(randomTag, false);


            //assert
            Assert.IsInstanceOfType(result, typeof(TagResult));
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.isEmpty);
            Assert.AreEqual(false, result.isEmpty);

        }
    }
}
