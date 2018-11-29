using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Application.UnitTests
{
    [TestClass]
    public class ServiceTests
    {
        [TestMethod, Owner(@"IvanY")]
        public void Add()
        {
            var service = new Service();

            Assert.AreEqual(3, service.Add(1, 2));
        }
    }
}