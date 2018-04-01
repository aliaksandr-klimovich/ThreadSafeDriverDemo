using NUnit.Framework;

namespace WebAutomationFramework
{
    public class BaseTest 
    {
        [SetUp]
        public void SetUp()
        {
            
        }

        [TearDown]
        public void TearDown()
        {
            Driver.Instance.StopWebDriver();
        }
    }
}
