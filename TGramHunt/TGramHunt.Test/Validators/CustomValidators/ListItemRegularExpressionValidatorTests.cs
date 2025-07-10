using Microsoft.VisualStudio.TestTools.UnitTesting;
using TGramHunt.Validators.CustomValidators;
using TGramHunt.ViewModels;

namespace TGramHunt.Test.Validators.CustomValidators
{
    [TestClass]
    public class ListItemRegularExpressionValidatorTests
    {
        private ListItemRegularExpressionValidator<CreateProductViewModel, string> _validator;

        public ListItemRegularExpressionValidatorTests()
        {
            _validator = new ListItemRegularExpressionValidator<CreateProductViewModel, string>("^[a-zA-Z][a-zA-Z’ -]+$");
        }

        [TestMethod]
        public void IsValid_NullList_ReturnsTrue()
        {
            var result = _validator.IsValid(null, null);

            Assert.AreEqual(true, result);
        }

        [TestMethod]
        [DataRow()]
        [DataRow(1)]
        public void IsValid_EmptyListOrNotStringList_ReturnsTrue(params int[] items)
        {
            var intValidator = new ListItemRegularExpressionValidator<CreateProductViewModel, int>("^[a-zA-Z][a-zA-Z’ -]+$");

            var result = intValidator.IsValid(null, items.ToList());

            Assert.AreEqual(true, result);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow(" ")]
        public void IsValid_NullOrEmptyItem_ReturnsFalse(string item)
        {
            var items = new List<string>();
            items.Add(item);

            var result = _validator.IsValid(null, items);

            Assert.AreEqual(false, result);
        }

        [TestMethod]
        [DataRow(true, "AA")]
        [DataRow(false, " A")]
        public void IsValid_SingleItem_ReturnsCorrectResult(bool expectedResult, string item)
        {
            var items = new List<string>();
            items.Add(item);

            var result = _validator.IsValid(null, items);

            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        [DataRow(false, "", "AA")]
        [DataRow(true, "AA", "BB")]
        [DataRow(false, "AA", "BB", " A")]
        public void IsValid_MultiplyItems_ReturnsCorrectResults(bool expectResult, params string[] items)
        {
            var result = _validator.IsValid(null, items.ToList());

            Assert.AreEqual(expectResult, result);
        }
    }
}