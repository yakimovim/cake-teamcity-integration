using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Application.UnitTests
{
    [TestClass]
    public class ServiceTests
    {
        [TestMethod, Owner("IvanY")]
        public void AddTest()
        {
            var service = new Service();

            Assert.AreEqual(3, service.Add(1, 2));
        }

        [TestMethod, Owner("IvanY")]
        public void Return3Test()
        {
            var service = new Service();

            Assert.AreEqual(3, service.Return3());
        }
    }
}