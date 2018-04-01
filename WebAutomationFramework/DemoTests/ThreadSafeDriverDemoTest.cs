using NUnit.Framework;
using OpenQA.Selenium;

namespace WebAutomationFramework
{
    [Parallelizable]
    [TestFixture("Chrome")]
    [TestFixture("Firefox")]
    public class ThreadSafeDriverDemoTest : BaseTest
    {
        protected string driverName;

        public ThreadSafeDriverDemoTest(string driverName)
        {
            this.driverName = driverName;
        }

        [Parallelizable]
        [Test]
        public void Test1()
        {
            IWebDriver driver = Driver.Instance.GetWebDriver(driverName);
            driver.Url = "http://www.tut.by";
            StringAssert.Contains("TUT.BY", driver.Title);
        }

        [Parallelizable]
        [Test]
        public void Test2()
        {
            IWebDriver driver = Driver.Instance.GetWebDriver(driverName);
            driver.Url = "http://www.onliner.by";
            StringAssert.Contains("Onliner.by", driver.Title);
        }

        [Parallelizable]
        [Test]
        public void Test3()
        {
            IWebDriver driver = Driver.Instance.GetWebDriver(driverName);
            driver.Url = "http://www.dev.by";
            StringAssert.Contains("dev.by", driver.Title);
        }

        [Parallelizable]
        [Test]
        public void Test4()
        {
            IWebDriver driver = Driver.Instance.GetWebDriver(driverName);
            driver.Url = "http://www.vk.com";
            StringAssert.Contains("VK", driver.Title);
        }
    }
}
