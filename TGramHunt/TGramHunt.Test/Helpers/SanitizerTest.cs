using Microsoft.AspNetCore.Mvc.ModelBinding;
using TGramHunt.Contract.Exceptions;

namespace TGramHunt.Test.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TGramHunt.Helpers;

[TestClass]
public class SanitizerTests
{
    [TestMethod]
    public void Sanitize_Sanitizes_String()
    {
        // Arrange
        var sanitizer = new Sanitizer();
        var dirtyString = "<script></script>";
        var expectedString = "";

        // Act
        var result = sanitizer.Sanitize(dirtyString);

        // Assert
        Assert.AreEqual(expectedString, result);
    }

    [TestMethod]
    public void Sanitize_Returns_Null_If_Input_Is_Null()
    {
        // Arrange
        var sanitizer = new Sanitizer();

        // Act
        var result = sanitizer.Sanitize<string>(null, new ModelStateDictionary());

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public void Sanitize_Leaves_Empty_String_As_Is()
    {
        // Arrange
        var sanitizer = new Sanitizer();

        // Act
        var result = sanitizer.Sanitize("");

        // Assert
        Assert.AreEqual("", result);
    }

    [TestMethod]
    public void Sanitize_Adds_Exception_To_ModelState_If_Sanitized_String_Is_Empty()
    {
        // Arrange
        var sanitizer = new Sanitizer();
        var dirtyString = "<script></script>";
        var modelState = new ModelStateDictionary();
        var expectedException = new SanitizedException("", dirtyString);

        // Act
        var result = sanitizer.Sanitize(dirtyString, "str", modelState);

        // Assert
        Assert.IsTrue(result == "");
        Assert.AreEqual(1, modelState.ErrorCount);
        Assert.AreEqual(expectedException.Message, modelState["str"].Errors[0].ErrorMessage);
    }
}
