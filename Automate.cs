using System;
using System.IO;
using System.Linq;
using System.Text;
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
            var logs = new StringBuilder();

            var logId = Guid.NewGuid().ToString();
            var filePath = Path.Combine(Environment.CurrentDirectory, @"TempLogs\", $"LOG-{logId}");

            logs.AppendLine($"[{DateTime.Now}] STARTING AUTOMATION");
            var retry = 0;
            try
            {
                var driver = EdgeDriver();

                // Hit the generated link
                const string url = "https://outreach.ophs.io/9bIh6ZVD";
                driver.Navigate().GoToUrl(url);
                Thread.Sleep(3000);
                while (driver.Url == null && retry < 3)
                {
                    logs.AppendLine($"[{DateTime.Now}] FAILED CONNECTION TO Invitation link, {url}");
                    driver.Navigate().Refresh();
                    logs.AppendLine($"[{DateTime.Now}] ACTION - Retrying...");
                    retry++;
                }

                if (driver.Url == null)
                {
                    Console.Write("FAILED");
                    logs.AppendLine($"[{DateTime.Now}] ENDING AUTOMATION");
                    logs.AppendLine($"[{DateTime.Now}] RESULT : FAILED");

                    File.AppendAllText($"{filePath}.txt", logs.ToString());
                    logs.Clear();

                    driver.Quit();

                    throw new Exception("COULD NOT CONNECT");
                }

                logs.AppendLine($"[{DateTime.Now}] SUCCESSFUL CONNECTION TO Invitation link, {url}");

                // Find the Swipe Up body
                var ele = driver.FindElement(By.Id("upArrow"));
                ele.Click();
                Thread.Sleep(2000);
                logs.AppendLine($"[{DateTime.Now}] ACTION - Swipe Up");

                // Find Country code element
                var ele1 = driver.FindElement(By.Name("country_code"));
                ele1.Click();
                ele1.SendKeys("+880");
                ele1.Click();
                Thread.Sleep(1000);
                logs.AppendLine($"[{DateTime.Now}] ACTION - Selected country code");

                // Find Phone number input element
                var ele2 = driver.FindElement(By.Name("phone_number"));
                var number = RandomGenerator(10);
                ele2.SendKeys(number);
                Thread.Sleep(1000);
                logs.AppendLine($"[{DateTime.Now}] ACTION - Entered dummy phone number, {number}");

                // Click for Send OTP
                var ele3 = driver.FindElement(By.ClassName("btn-sendotp"));
                ele3.Click();
                Thread.Sleep(2000);
                logs.AppendLine($"[{DateTime.Now}] ACTION - Requested OTP");

                // Enter Dummy OTP
                var ele4 = driver.FindElement(By.Name("otp"));
                ele4.SendKeys("123456");
                Thread.Sleep(1000);
                logs.AppendLine($"[{DateTime.Now}] ACTION - Entered dummy OTP, 123456");

                // Verify OTP
                var ele5 = driver.FindElement(By.ClassName("btn-sendotp"));
                ele5.Click();
                Thread.Sleep(3000);
                logs.AppendLine($"[{DateTime.Now}] ACTION - Click button to verify OTP");

                // In the case that the dummy number is unregistered, the bot will register into the system
                if (driver.Url == "https://outreach.ophs.io/register")
                {
                    logs.AppendLine($"[{DateTime.Now}] DETECTED Unregistered user. PROCEEDING Registration");
                    var elementName = driver.FindElement(By.Name("name"));
                    var dName = $"Ophs Selenium Bot - {new Random().Next(50000)}";
                    elementName.SendKeys(dName);
                    Thread.Sleep(1500);
                    logs.AppendLine($"[{DateTime.Now}] ACTION - Enter dummy name, {dName}");

                    var elementAge = driver.FindElement(By.Name("age"));
                    var dAge = new Random().Next(18, 105).ToString();
                    elementAge.SendKeys(dAge);
                    Thread.Sleep(1500);
                    logs.AppendLine($"[{DateTime.Now}] ACTION - Enter dummy age, {dAge}");

                    var elementGender = driver.FindElement(By.Id("dont_disclose"));
                    elementGender.Click();
                    Thread.Sleep(2000);
                    logs.AppendLine($"[{DateTime.Now}] ACTION - Selected undisclosed gender");

                    var elementNext = driver.FindElement(By.ClassName("btn-next"));
                    elementNext.Click();
                    Thread.Sleep(3000);
                    logs.AppendLine($"[{DateTime.Now}] ACTION - Click button to register");
                }
                else
                {
                    logs.AppendLine($"[{DateTime.Now}] DETECTED Registered user");
                }

                // Click Enter Experience
                logs.AppendLine($"[{DateTime.Now}] REACHED Home Property");
                if (driver.Url == "https://outreach.ophs.io/home-buyer")
                {
                    var ele6 = driver.FindElement(By.ClassName("default-enter-experience"));
                    ele6.Click();
                }
                else
                {
                    var ele6 = driver.FindElement(By.Id("enterExperience"));
                    ele6.Click();
                }

                logs.AppendLine($"[{DateTime.Now}] ACTION - Click to Enter Experience");

                // Delay added to sync with Matchmaker delay
                logs.AppendLine($"[{DateTime.Now}] ACTION - Starting a 70s delay");
                Thread.Sleep(70000);
                // 3 retries
                retry = 0;
                while (driver.Url == "https://outreach.ophs.io/experience-loading" && retry < 1)
                {
                    driver.Navigate().Refresh();
                    logs.AppendLine($"[{DateTime.Now}] ACTION - Retrying...");
                    retry++;
                    Thread.Sleep(70000);
                }

                if (driver.Url == "https://outreach.ophs.io/experience-loading")
                {
                    Console.Write("FAILED");
                    logs.AppendLine($"[{DateTime.Now}] ENDING AUTOMATION");
                    logs.AppendLine($"[{DateTime.Now}] RESULT : FAILED");

                    File.AppendAllText($"{filePath}.txt", logs.ToString());
                    logs.Clear();

                    driver.Quit();

                    throw new Exception("COULD NOT FIND ANY INSTANCES");
                }

                logs.AppendLine($"[{DateTime.Now}] ACTION - Ending delay");

                // Tap to continue
                var ele7 = driver.FindElement(By.Id("main"));
                ele7.Click();
                Thread.Sleep(5000);
                logs.AppendLine($"[{DateTime.Now}] ACTION - Click to enter");
                
                // Skip tutorial
                var ele8 = driver.FindElement(By.ClassName("skip-ex"));
                if (ele8 != null)
                {
                    ele8.Click();
                    Thread.Sleep(2000);
                    logs.AppendLine($"[{DateTime.Now}] ACTION - Skip tutorial");
                }

                // Pull up hidden panel
                var ele9 = driver.FindElement(By.Id("chevronSlider"));
                ele9.Click();
                Thread.Sleep(2000);
                logs.AppendLine($"[{DateTime.Now}] ACTION - Pull up slider menu");

                // Exit stream
                var ele10 = driver.FindElement(By.Id("outreach-back-url"));
                ele10.Click();
                Thread.Sleep(3000);
                logs.AppendLine($"[{DateTime.Now}] ACTION - Click to exit experience");

                Console.Write("SUCCESS");
                logs.AppendLine($"[{DateTime.Now}] ENDING AUTOMATION");
                logs.AppendLine($"[{DateTime.Now}] RESULT : SUCCESS");
                
                File.AppendAllText($"{filePath}.txt", logs.ToString());
                logs.Clear();

                return;
            }
            catch (Exception ex)
            {
                Console.Write("FAILED");
                logs.AppendLine($"[{DateTime.Now}] ENDING AUTOMATION");
                logs.AppendLine($"[{DateTime.Now}] RESULT : FAILED");
                logs.AppendLine($"[{DateTime.Now}] REASON : {ex.Message}");

                File.AppendAllText($"{filePath}.txt", logs.ToString());
                logs.Clear();

                throw new Exception(ex.Message);
            }
        }

        #region Initialize Browser Drivers

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

        #endregion

        #region Randomized Generators

        private static string RandomGenerator(int count)
        {
            var digits = "0123456789";

            var random = new Random();
            return new string(Enumerable.Repeat(digits, count).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        #endregion
    }
}