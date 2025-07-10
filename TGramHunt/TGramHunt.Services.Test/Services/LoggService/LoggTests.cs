using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace TGramHunt.Services.Test.Services.LoggService
{
    [TestClass]
    public class LoggTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            Assert.AreNotEqual(1, 2);
        }

        [TestMethod]
        public void TestMethod2()
        {
            Assert.AreEqual(1, 1);
        }
    }
}
