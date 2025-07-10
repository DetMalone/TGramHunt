using Microsoft.VisualStudio.TestTools.UnitTesting;
using TGramHunt.Validators.CustomValidators;
using TGramHunt.ViewModels;

namespace TGramHunt.Test.Validators.CustomValidators
{
    [TestClass]
    public class ListItemMaxLengthValidatorTests
    {
        private const int ITEM_MAX_LENGTH = 100;
        private ListItemMaxLengthValidator<CreateProductViewModel, string> _validator;

        public ListItemMaxLengthValidatorTests()
        {
            _validator = new ListItemMaxLengthValidator<CreateProductViewModel, string>(ITEM_MAX_LENGTH);
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
            var intValidator = new ListItemMaxLengthValidator<CreateProductViewModel, int>(ITEM_MAX_LENGTH);

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
        [DataRow(1)]
        [DataRow(ITEM_MAX_LENGTH)]
        public void IsValid_ShortItem_ReturnsTrue(int itemLength)
        {
            var items = new List<string>();
            items.Add(new string('A', itemLength));

            var result = _validator.IsValid(null, items);

            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public void IsValid_OverMaxLength_ReturnsFalse()
        {
            var items = new List<string>();
            items.Add(new string('A', ITEM_MAX_LENGTH + 1));

            var result = _validator.IsValid(null, items);

            Assert.AreEqual(false, result);
        }


        [TestMethod]
        [DataRow(false, 0, 1)]
        [DataRow(true, 1, 2)]
        [DataRow(false, 1, 2, ITEM_MAX_LENGTH + 1)]
        public void IsValid_MultiplyItems_ReturnsCorrectResults(bool expectedResult, params int[] itemLengths)
        {
            var items = itemLengths.Select(length => new string('A', length)).ToList();

            var result = _validator.IsValid(null, items);

            Assert.AreEqual(expectedResult, result);
        }
    }
}