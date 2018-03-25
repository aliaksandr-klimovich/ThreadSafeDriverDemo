﻿using System;
using System.Threading;
using System.Collections.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using NUnit.Framework;

namespace ThreadSafeDriverDemo
{
    public sealed class Driver
    {
        private static int MAX_WEB_DRIVERS = 3;  // Max web drivers to run in parallel.
        private static int WAIT_FREE_WEB_DRIVER_COUNT = 1000;  // Each thread will sleep this time for creating a new instance of web driver.
        private readonly Dictionary<int, IWebDriver> webDrivers = new Dictionary<int, IWebDriver>();  // Key = thread ID. Value = web driver instance.

        private static Object syncDriver = new Object();  // To sync `Driver` class.
        private static Mutex syncWebDriver = new Mutex();  // To sync `webDrivers` dict.

        // Make `Driver` as singleton.
        private static volatile Driver instance;
        private Driver() { }
        public static Driver Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncDriver)
                    {
                        if (instance == null)
                        {
                            instance = new Driver();
                        }
                    }
                }
                return instance;
            }
        }

        /*
         * Return a new web driver for current thread or an existing one if it was previously created.
         */
        public IWebDriver GetWebDriver(string driverName = "Chrome")
        {
            IWebDriver currentWebDriver;
            int currentThreadId = Thread.CurrentThread.GetHashCode();

            while (true)  // TODO: Add MAX_RETRIES counter.
            {
                syncWebDriver.WaitOne();
                if (webDrivers.Count >= MAX_WEB_DRIVERS)
                {  // This part can be removed if --agents=MAX_WEB_DRIVERS is specified for nunit concole runner.
                    Thread.Sleep(WAIT_FREE_WEB_DRIVER_COUNT);
                    syncWebDriver.ReleaseMutex();
                }
                else
                {  // Have possibility to create new instance of web driver.
                    if (webDrivers.TryGetValue(currentThreadId, out currentWebDriver) == false)
                    {
                        switch (driverName)
                        {
                            case "Chrome":
                                currentWebDriver = new ChromeDriver();
                                break;
                            case "Firefox":
                                currentWebDriver = new FirefoxDriver();
                                break;
                            default:
                                throw new Exception($"Unknown browser! Got: {driverName}.");
                        }
                        webDrivers[currentThreadId] = currentWebDriver;
                    }  // Else the web driver exists and will be returned.
                    syncWebDriver.ReleaseMutex();
                    break;
                }
            }

            return currentWebDriver;
        }

        /*
         * Close web driver for current thread.
         */
        public void CloseWebDriver()
        {
            int currentThreadId = Thread.CurrentThread.GetHashCode();
            lock (syncDriver)
            {
                if (webDrivers.TryGetValue(currentThreadId, out IWebDriver currentWebDriver) == true)
                {
                    currentWebDriver.Quit();
                    webDrivers.Remove(currentThreadId);
                }
            }
        }

        ~Driver()
        {
            // Close all web drivers if any.
            IWebDriver currentWebDriver;
            foreach (KeyValuePair<int, IWebDriver> entry in webDrivers)
            {
                currentWebDriver = entry.Value;
                currentWebDriver.Quit();
            }
        }
    }

    [Parallelizable]
    [TestFixture("Chrome")]
    [TestFixture("Firefox")]
    public class DriverDemoTests
    {
        private IWebDriver driver;
        private string driverName;

        public DriverDemoTests(string driverName)
        {
            this.driverName = driverName;
        }

        [SetUp]
        public void Initialize()
        {
            driver = Driver.Instance.GetWebDriver(driverName);
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

        [TearDown]
        public void Cleanup()
        {
            Driver.Instance.CloseWebDriver();
        }
    }
}
