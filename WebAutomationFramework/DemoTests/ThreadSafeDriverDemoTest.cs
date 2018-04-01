using NUnit.Framework;

namespace WebAutomationFramework
{
    [Parallelizable]
    [TestFixture("Chrome")]
    [TestFixture("Firefox")]
    public class DriverDemoTests : BaseTest
    {
        public DriverDemoTests(string driverName)
        {
            this.driverName = driverName;
        }

        [Parallelizable]
        [Test]
        public void Test1()
        {
            driver.Url = "http://www.tut.by";
        }

        [Parallelizable]
        [Test]
        public void Test2()
        {
            driver.Url = "http://www.onliner.by";
        }

        [Parallelizable]
        [Test]
        public void Test3()
        {
            driver.Url = "http://www.dev.by";
        }

        [Parallelizable]
        [Test]
        public void Test4()
        {
            driver.Url = "http://www.vk.com";
        }
    }
}
