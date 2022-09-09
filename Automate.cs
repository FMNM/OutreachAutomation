using System;
using System.Collections.Generic;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;

namespace OutreachAutomation
{
    public static class Automate
    {
        public static void Script()
        {
            try
            {
                var driver = EdgeDriver();

                // Hit the generated link
                driver.Navigate().GoToUrl("https://outreach.ophs.io/9bIh6ZVD");
                Thread.Sleep(3000);
                
                if (driver.Url == null)
                {
                    Automate.Script();
                }

                // Find the Swipe Up body
                var ele = driver.FindElement(By.Id("upArrow"));
                ele.Click();
                Thread.Sleep(2000);

                // Find Country code element
                var ele1 = driver.FindElement(By.Name("country_code"));
                ele1.Click();
                ele1.SendKeys("+880");
                ele1.Click();
                Thread.Sleep(1000);

                // Find Phone number input element
                var ele2 = driver.FindElement(By.Name("phone_number"));
                ele2.SendKeys(GetDummyNumber());
                Thread.Sleep(1000);

                // Click for Send OTP
                var ele3 = driver.FindElement(By.ClassName("btn-sendotp"));
                ele3.Click();
                Thread.Sleep(2000);

                // Enter Dummy OTP
                var ele4 = driver.FindElement(By.Name("otp"));
                ele4.SendKeys("123456");
                Thread.Sleep(1000);

                // Verify OTP
                var ele5 = driver.FindElement(By.ClassName("btn-sendotp"));
                ele5.Click();
                Thread.Sleep(3000);

                // In the case that the dummy number is unregistered, the bot will register into the system
                if (driver.Url == "https://outreach.ophs.io/register")
                {
                    var elementName = driver.FindElement(By.Name("name"));
                    elementName.SendKeys("Ophs Selenium Bot - " + new Random().Next(50000).ToString());
                    Thread.Sleep(1500);

                    var elementAge = driver.FindElement(By.Name("age"));
                    elementAge.SendKeys("999");
                    Thread.Sleep(1500);

                    var elementGender = driver.FindElement(By.Id("dont_disclose"));
                    elementGender.Click();
                    Thread.Sleep(2000);

                    var elementNext = driver.FindElement(By.ClassName("btn-next"));
                    elementGender.Click();
                    Thread.Sleep(3000);
                }

                // Click Enter Experience
                var ele6 = driver.FindElement(By.Id("enterExperience"));
                ele6.Click();

                // Delay added to sync with Matchmaker delay
                Thread.Sleep(70000);

                // Tap to continue
                var ele7 = driver.FindElement(By.Id("main"));
                ele7.Click();
                Thread.Sleep(5000);

                // Pull up hidden panel
                var ele8 = driver.FindElement(By.Id("chevronSlider"));
                ele8.Click();
                Thread.Sleep(2000);

                // Exit stream
                var ele9 = driver.FindElement(By.Id("outreach-back-url"));
                ele9.Click();
                Thread.Sleep(3000);

                Console.Write("SUCCESS");
            }
            catch (Exception ex)
            {
                Console.Write("FAILED");
                throw new Exception(ex.Message);
            }
        }

        // Random test script to try out
        public static void TestScript()
        {
            Console.Write("Test case started");

            var driver = EdgeDriver();

            Thread.Sleep(1000);
            driver.Navigate().GoToUrl("https://www.google.com");
            Thread.Sleep(1000);
            var ele = driver.FindElement(By.Name("q"));
            ele.SendKeys("Openhaus");
            Thread.Sleep(1000);
            var ele1 = driver.FindElement(By.Name("btnK"));
            ele1.Click();
            Thread.Sleep(1000);
            driver.Close();

            Console.Write("Test case ended");
        }

        private static EdgeDriver EdgeDriver()
        {
            // Chrome
            new DriverManager().SetUpDriver(new EdgeConfig());
            var driver = new EdgeDriver();

            return driver;
        }

        private static ChromeDriver ChromeDriver()
        {
            new DriverManager().SetUpDriver(new ChromeConfig());
            var driver = new ChromeDriver();

            return driver;
        }

        private static FirefoxDriver FirefoxDriver()
        {
            new DriverManager().SetUpDriver(new FirefoxConfig());
            var driver = new FirefoxDriver();

            return driver;
        }

        private static string GetDummyNumber()
        {
            var numbers = new List<string>()
            {
                "1231231231",
                "1234561231",
                "1231234561",
                "4561231231",
                "1234564561",
                "4564564561",
            };

            var random = new Random();
            var index = random.Next(numbers.Count);

            return numbers[index];
        }
    }
}