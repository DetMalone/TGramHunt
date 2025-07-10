using Microsoft.VisualStudio.TestTools.UnitTesting;
using TGramHunt.Contract.ViewModels.EditProfile;
using TGramHunt.Validators;

namespace TGramHunt.Test.Validators
{
    [TestClass]
    public class ProfileViewModelBaseValidatorTests
    {
        private ProfileViewModelBaseValidator _validator;

        public ProfileViewModelBaseValidatorTests()
        {
            _validator = new ProfileViewModelBaseValidator();
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow(" ")]
        public void Validate_NullOrEmpty_ReturnsFalse(string name)
        {
            var model = new ProfileViewModelBase() { Name = name };

            var result = _validator.Validate(model);

            Assert.AreEqual(false, result.IsValid);
            Assert.AreEqual("Enter Name.", result.ToString());
        }

        [TestMethod]
        [DataRow(false, "A")]
        [DataRow(false, "!")]
        [DataRow(false, "AAA!")]
        [DataRow(true, "AAA")]
        [DataRow(true, "111")]
        [DataRow(true, " AAA ")]
        public void Validate_SingleShortWord_ReturnsCorrectResults(bool expectedResult, string name)
        {
            var model = new ProfileViewModelBase() { Name = name };

            var result = _validator.Validate(model);

            Assert.AreEqual(expectedResult, result.IsValid);
            Assert.IsTrue(result.ToString() == ""
                       || result.ToString() == "One word of name should contain more than 2 and less than 256 symbols."
                       || result.ToString() == "Name should contain only latin characters and numeric, it can be consist of two or one word.");
        }

        [TestMethod]
        [DataRow(false, "A A")]
        [DataRow(false, "! A")]
        [DataRow(false, "AAA AAA!")]
        [DataRow(false, "AAA  AAA")]
        [DataRow(true, "AAA AAA")]
        [DataRow(true, "AAA 111")]
        [DataRow(true, " AAA 111  ")]
        public void Validate_MultiplyShortWords_ReturnsCorrectResults(bool expectedResult, string name)
        {
            var model = new ProfileViewModelBase() { Name = name };

            var result = _validator.Validate(model);

            Assert.AreEqual(expectedResult, result.IsValid);
            Assert.IsTrue(result.ToString() == ""
                       || result.ToString().Substring(6) == "part of name should contain more than 2 and less than 124 symbols."
                       || result.ToString() == "Name should contain only latin characters and numeric, it can be consist of two or one word.");
        }

        [TestMethod]
        [DataRow(true, 255)]
        [DataRow(true, 123, 123)]
        [DataRow(false, 256)]
        [DataRow(false, 124, 3)]
        [DataRow(false, 3, 3, 3)]
        public void Validate_LongWords_ReturnsCorrectResult(bool expectedResult, params int[] wordLengths)
        {
            var name = string.Join(" ", wordLengths.Select(length => new string('A', length)));
            var model = new ProfileViewModelBase() { Name = name };

            var result = _validator.Validate(model);

            Assert.AreEqual(expectedResult, result.IsValid);
            Assert.IsTrue(result.ToString() == ""
                       || result.ToString() == "One word of name should contain more than 2 and less than 256 symbols."
                       || result.ToString() == "Name should contain only latin characters and numeric, it can be consist of two or one word."
                       || result.ToString().Substring(6) == "part of name should contain more than 2 and less than 124 symbols.");
        }
    }
}
