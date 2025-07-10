using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TGramHunt.Validators.CustomValidators;
using TGramHunt.ViewModels;

namespace TGramHunt.Test.Validators.CustomValidators
{
    [TestClass]
    public class AllowedExtensionsValidatorTests
    {
        private AllowedExtensionsValidator<CreateProductViewModel> _validator;
        private List<byte[]> _signatures = new List<byte[]>()
        {
            new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 }, //jpg
            new byte[] { 0xFF, 0xD8, 0xFF, 0xE1 }, //jpg
            new byte[] { 0xFF, 0xD8, 0xFF, 0xE2 }, //jpg
            new byte[] { 0x89, 0x50, 0x4E, 0x47 }  //png
        };

        public AllowedExtensionsValidatorTests()
        {
            _validator = new AllowedExtensionsValidator<CreateProductViewModel>(_signatures);
        }

        [TestMethod]
        public void IsValid_NullFile_ReturnsFalse()
        {
            var result = _validator.IsValid(null, null);

            Assert.AreEqual(false, result);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(false, 0xFF)]
        [DataRow(false, 0xFF, 0x00, 0xFF, 0x00)]
        [DataRow(false, 0x00, 0xFF, 0xD8, 0xFF, 0xE0)]
        [DataRow(true, 0xFF, 0xD8, 0xFF, 0xE0)]
        [DataRow(true, 0x89, 0x50, 0x4E, 0x47, 0xFF)]
        public void IsValid_NotNullImages_ReturnsCorrectResults(bool expectedResult, params int[] signature)
        {
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(file => file.Length).Returns(signature.Length);
            mockFile.Setup(file => file.OpenReadStream()).Returns(new MemoryStream(signature.Select(i => (byte)i).ToArray()));

            var result = _validator.IsValid(null, mockFile.Object);

            Assert.AreEqual(expectedResult, result);
        }
    }
}
