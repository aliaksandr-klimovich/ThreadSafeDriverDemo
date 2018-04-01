using NUnit.Framework;
using OpenQA.Selenium;

namespace WebAutomationFramework
{
    public class BaseTest 
    {
        protected IWebDriver driver;
        protected string driverName;

        [SetUp]
        public void SetUp()
        {
            driver = Driver.Instance.GetWebDriver(driverName);
        }

        [TearDown]
        public void TearDown()
        {
            Driver.Instance.StopWebDriver();
        }
    }
}
