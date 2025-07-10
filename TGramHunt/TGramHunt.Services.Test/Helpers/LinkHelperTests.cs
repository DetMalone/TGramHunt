using Microsoft.VisualStudio.TestTools.UnitTesting;
using TGramHunt.Services.Helpers;

namespace TGramHunt.Services.Test.Helpers
{
    [TestClass]
    public class LinkHelperTests
    {
        private const string IMAGE_LINK = "picture";

        [TestMethod]
        [DataRow(null)]
        [DataRow(" ")]
        public void CreateLink_NullOrEmptyTag_ReturnsNull(string tag)
        {
            var result = LinkHelper.CreateLink(tag);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void CreateLink_CorrectTag_ReturnsCorrectResult()
        {
            var result = LinkHelper.CreateLink("@tag");

            Assert.AreEqual("https://telegram.me/tag", result);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow(" ")]
        public void CreateLink_NullOrImageId_ReturnsNull(string id)
        {
            var result = LinkHelper.ImageRelativLink(id, 0, IMAGE_LINK);

            Assert.AreEqual(IMAGE_LINK, result);
        }

        [TestMethod]
        public void CreateLink_CorrectIdCachePicture_ReturnsCorrectResult()
        {
            var result = LinkHelper.ImageRelativLink("id", 1, IMAGE_LINK);

            Assert.AreEqual("/api/Image/id?cache=1", result);
        }
    }
}
