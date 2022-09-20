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
                0 => ChromeDriver(),
                1 => EdgeDriver(),
                _ => FirefoxDriver()
            };
        }

        // Microsoft Edge
        private static EdgeDriver EdgeDriver()
        {
            new DriverManager().SetUpDriver(new EdgeConfig());

            var op = new EdgeOptions();
            op.AddArgument("--window-size=390,844");

            var driver = new EdgeDriver(op);

            return driver;
        }

        // Google Chrome
        private static ChromeDriver ChromeDriver()
        {
            new DriverManager().SetUpDriver(new ChromeConfig());

            var op = new ChromeOptions();
            op.AddArgument("--window-size=390,844");

            var driver = new ChromeDriver(op);

            return driver;
        }

        // Mozilla Firefox
        private static FirefoxDriver FirefoxDriver()
        {
            new DriverManager().SetUpDriver(new FirefoxConfig());

            var op = new FirefoxOptions();
            op.AddArgument("--width=390");
            op.AddArgument("--height=844");
            
            var driver = new FirefoxDriver(op);

            return driver;
        }
    }
}