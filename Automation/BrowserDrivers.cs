using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;

namespace OutreachAutomation.Automation
{
    public static class BrowserDrivers
    {
        public static EdgeDriver EdgeDriver()
        {
            // Chrome
            new DriverManager().SetUpDriver(new EdgeConfig());
            var driver = new EdgeDriver();

            return driver;
        }

        public static ChromeDriver ChromeDriver()
        {
            new DriverManager().SetUpDriver(new ChromeConfig());
            var driver = new ChromeDriver();

            return driver;
        }

        public static FirefoxDriver FirefoxDriver()
        {
            new DriverManager().SetUpDriver(new FirefoxConfig());
            var driver = new FirefoxDriver();

            return driver;
        }
    }
}