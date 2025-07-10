using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TGramHunt.Validators.CustomValidators;
using TGramHunt.ViewModels;

namespace TGramHunt.Test.Validators.CustomValidators
{
    [TestClass]
    public class FileSizeValidatorTests
    {
        private const int MAX_FILE_SIZE = 1024 * 1024 * 10;
        private FileSizeValidator<CreateProductViewModel> _validator;

        public FileSizeValidatorTests()
        {
            _validator = new FileSizeValidator<CreateProductViewModel>(MAX_FILE_SIZE);
        }

        [TestMethod]
        public void IsValid_NullFile_ReturnsTrue()
        {
            var result = _validator.IsValid(null, null);

            Assert.AreEqual(true, result);
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(MAX_FILE_SIZE)]
        public void IsValid_NotOverSize_ReturnsTrue(long fileSize)
        {
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(file => file.Length).Returns(fileSize);

            var result = _validator.IsValid(null, mockFile.Object);

            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public void IsValid_OverSize_ReturnsFalse()
        {
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(file => file.Length).Returns(MAX_FILE_SIZE + 1);

            var result = _validator.IsValid(null, mockFile.Object);

            Assert.AreEqual(false, result);
        }
    }
}