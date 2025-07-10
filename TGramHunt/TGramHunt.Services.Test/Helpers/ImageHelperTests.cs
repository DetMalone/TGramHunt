using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TGramHunt.Services.Helpers;
using TGramHunt.Services.Services.IServices;
using TGramHunt.Contract;

namespace TGramHunt.Services.Test.Helpers
{
    [TestClass]
    public class ImageHelperTests
    {
        private const string IMAGE_ID = "id";

        private ImageHelper _imageHelper;

        private byte[] _imageBytes;

        public ImageHelperTests()
        {
            _imageBytes = new byte[] { 137, 80, 78, 71, 13, 10, 26, 10, 0, 0, 0, 13,
                73, 72, 68, 82, 0, 0, 0, 1, 0, 0, 0, 1, 8, 2, 0, 0, 0, 144, 119, 83, 222,
                0, 0, 0, 1, 115, 82, 71, 66, 0, 174, 206, 28, 233, 0, 0, 0, 12, 73, 68,
                65, 84, 24, 87, 99, 248, 255, 255, 63, 0, 5, 254, 2, 254, 167, 53, 129,
                132, 0, 0, 0, 0, 73, 69, 78, 68, 174, 66, 96, 130 }; // 1x1 png image

            var mockFileService = new Mock<ISmallFilesService>();
            mockFileService.Setup(service => service.Create(It.IsAny<SmallFile>()))
                .Callback<SmallFile>(file => file.Id = IMAGE_ID);

            _imageHelper = new ImageHelper(mockFileService.Object);
        }

        [TestMethod]
        public void ResizeImage_NullFormat_ReturnsEmptyImage()
        {
            var result = _imageHelper.ResizeImage(new byte[0], 0, 0);

            Assert.AreEqual(Array.Empty<byte>(), result);
        }

        [TestMethod]
        public void ResizeImage_1PixelImage_ReturnsNotEmptyImage()
        {
            var result = _imageHelper.ResizeImage(_imageBytes, 1, 1);

            Assert.IsTrue(result.Any());
        }

        [TestMethod]
        public void ResizeImage_1PixelImage_SavesAndReturnsId()
        {
            var testTask = _imageHelper.ResizeAndSaveImage(_imageBytes, 1, 1);
            testTask.Wait();

            Assert.AreEqual(IMAGE_ID, testTask.Result);
        }
    }
}
