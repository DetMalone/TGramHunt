using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TGramHunt.Contract.Enums;
using TGramHunt.Validators;
using TGramHunt.ViewModels;

namespace TGramHunt.Test.Validators
{
    [TestClass]
    public class CreateProductViewModelValidatorTests
    {
        private CreateProductViewModelValidator _validator;
        private CreateProductViewModel _correctModel;

        public CreateProductViewModelValidatorTests()
        {
            _validator = new CreateProductViewModelValidator();

            var mockCorrectImage = new Mock<IFormFile>();
            mockCorrectImage.Setup(image => image.Length).Returns(4);
            mockCorrectImage.Setup(image => image.OpenReadStream()).Returns(new MemoryStream(new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 }));
            _correctModel = new CreateProductViewModel()
            {
                Tag = "@TagTag",
                Name = "Name",
                Description = "Description",
                Category = ProductCategory.Bot,
                Makers = new List<string>(),
                Cover = mockCorrectImage.Object
            };
        }

        [TestMethod]
        public void Validate_CorrectModel_ReturnsTrue()
        {
            var result = _validator.Validate(_correctModel);

            Assert.AreEqual(true, result.IsValid);
        }

        [TestMethod]
        public void Validate_OverMaxCountMakers_ReturnsFalse()
        {
            _correctModel.Makers = new List<string>() { "A", "B", "C", "D", "E", "F", "G" };
            var result = _validator.Validate(_correctModel);

            Assert.AreEqual(false, result.IsValid);
        }
    }
}
