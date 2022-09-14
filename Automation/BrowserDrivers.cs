using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;

namespace OutreachAutomation.Automation
{
    public static class BrowserDrivers
    {
        // Random browser to call
        public static WebDriver GetDriver(int i)
        {
            return i switch
            {
                0 => EdgeDriver(),
                1 => ChromeDriver(),
                _ => FirefoxDriver()
            };
        }
        
        // Microsoft Edge
        private static EdgeDriver EdgeDriver()
        {
            new DriverManager().SetUpDriver(new EdgeConfig());
            var driver = new EdgeDriver();

            return driver;
        }

        // Google Chrome
        private static ChromeDriver ChromeDriver()
        {
            new DriverManager().SetUpDriver(new ChromeConfig());
            var driver = new ChromeDriver();

            return driver;
        }

        // Mozilla Firefox
        private static FirefoxDriver FirefoxDriver()
        {
            new DriverManager().SetUpDriver(new FirefoxConfig());
            var driver = new FirefoxDriver();

            return driver;
        }
    }
}