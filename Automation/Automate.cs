using System;
using System.IO;
using System.Text;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using OutreachAutomation.Automation.DriverBrowser;

namespace OutreachAutomation.Automation
{
    public static class Automate
    {
        public static void Script(WebDriver driver, string url)
        {
            var logs = new StringBuilder();
            var logId = Guid.NewGuid().ToString().ToUpper();
            var filePath = Path.Combine(Environment.CurrentDirectory, Directory.CreateDirectory("TempLogs").ToString(),
                $"LOG-{logId}.txt");

            try
            {
                var browser = new BrowserInfo
                {
                    Name = driver.Capabilities.GetCapability("browserName").ToString(),
                    Version = driver.Capabilities.GetCapability("browserVersion").ToString(),
                };

                logs.AppendLine($"Selenium WebDriver Bot, Log - {logId}");
                logs.AppendLine("---------------------------------------------");
                logs.AppendLine($"BROWSER : {browser.Name}");
                logs.AppendLine($"VERSION : {browser.Version}");
                logs.AppendLine("---------------------------------------------");
                logs.AppendLine($"[{DateTime.Now}] STARTING AUTOMATION");

                var retry = 0;

                if (url == null)
                {
                    throw new Exception("No invitation link found");
                }

                driver.Navigate().GoToUrl(url);
                Thread.Sleep(3000);
                // Retries 3 times
                while (driver.Url == null && retry < 3)
                {
                    logs.AppendLine($"[{DateTime.Now}] ACTION - Connection failed to invitation link, {url}");
                    driver.Navigate().Refresh();
                    logs.AppendLine($"[{DateTime.Now}] ACTION - Retrying...");
                    retry++;
                }

                if (driver.Url == null)
                {
                    throw new Exception("Could not connect");
                }

                logs.AppendLine($"[{DateTime.Now}] ACTION - Connected to invitation link, {url}");

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
                var number = Generator.NumberGenerator(10);
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
                    var dAge = new Random().Next(18, 100).ToString();
                    elementAge.SendKeys(dAge);
                    Thread.Sleep(1000);

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
                logs.AppendLine($"[{DateTime.Now}] REACHED Home Page");
                Thread.Sleep(1500);
                if (driver.Url == "https://outreach.ophs.io/home-buyer")
                {
                    var ele6 = driver.FindElement(By.Id("enter-ex-1"));
                    try
                    {
                        ele6.Click();
                        Thread.Sleep(1500);
                    }
                    catch (Exception)
                    {
                        ele6 = driver.FindElement(By.Id("enter-ex-2"));
                        Thread.Sleep(1500);
                        ele6.Click();
                    }
                }
                else
                {
                    var ele6 = driver.FindElement(By.Id("enterExperience"));
                    Thread.Sleep(1500);
                    ele6.Click();
                }

                logs.AppendLine($"[{DateTime.Now}] ACTION - Click to Enter Experience");

                // Find instance
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(210));
                var res = wait.Until(x => x.Url.Contains("stream"));

                if (!res)
                {
                    throw new Exception("Could not find any instances");
                }

                // Tap to continue
                Thread.Sleep(5000);
                try
                {
                    var ele7 = driver.FindElement(By.Id("main"));
                    if (ele7 == null)
                    {
                        driver.Navigate().Refresh();
                        Thread.Sleep(8000);

                        ele7 = driver.FindElement(By.Id("main"));
                        if (ele7 == null)
                        {
                            throw new Exception("Could not enter instance");
                        }
                    }

                    ele7.Click();
                    Thread.Sleep(2000);
                    logs.AppendLine($"[{DateTime.Now}] ACTION - Click to enter");

                    // Skip tutorial (if any)
                    var ele8 = driver.FindElement(By.ClassName("text-cyan"));
                    if (ele8 != null)
                    {
                        driver.ExecuteScript("hideAllTutorials()");
                        Thread.Sleep(2000);
                        logs.AppendLine($"[{DateTime.Now}] ACTION - Skip tutorial");
                    }
                }
                catch (Exception)
                {
                    driver.Navigate().Refresh();
                    Thread.Sleep(6000);
                    try
                    {
                        var ele7 = driver.FindElement(By.Id("main"));
                        if (ele7 == null)
                        {
                            driver.Navigate().Refresh();
                            Thread.Sleep(8000);

                            ele7 = driver.FindElement(By.Id("main"));
                            if (ele7 == null)
                            {
                                throw new Exception("Could not enter instance");
                            }
                        }

                        ele7.Click();
                        Thread.Sleep(2000);
                        logs.AppendLine($"[{DateTime.Now}] ACTION - Click to enter");

                        // Skip tutorial (if any)
                        var ele8 = driver.FindElement(By.ClassName("text-cyan"));
                        if (ele8 != null)
                        {
                            driver.ExecuteScript("hideAllTutorials()");
                            Thread.Sleep(2000);
                            logs.AppendLine($"[{DateTime.Now}] ACTION - Skip tutorial");
                        }
                    }
                    catch (Exception)
                    {
                        throw new Exception("Could not enter instance properly");
                    }
                }

                // Click 'Nearby'
                driver.ExecuteScript("nearbyV2()");
                Thread.Sleep(2000);
                logs.AppendLine($"[{DateTime.Now}] ACTION - Clicked on 'Nearby'");
                var eleNearby = driver.FindElement(By.Id("nearbyListAdapter2")) ?? throw new Exception("No nearby elements found");

                // Click second element
                var eleNearbyData = driver.FindElement(By.Id("activeNearbyId1"));
                Thread.Sleep(2000);
                eleNearbyData.Click();
                Thread.Sleep(6000);
                logs.AppendLine($"[{DateTime.Now}] ACTION - Clicked on second element");

                // Reset view
                driver.ExecuteScript("resetView('true')");
                Thread.Sleep(2000);
                logs.AppendLine($"[{DateTime.Now}] ACTION - Reset view");

                // Click 'Amenities'
                driver.ExecuteScript("amenityV2()");
                Thread.Sleep(2000);
                logs.AppendLine($"[{DateTime.Now}] ACTION - Clicked on 'Amenities'");
                var eleAmenities = driver.FindElement(By.Id("amenityListAdapter2")) ?? throw new Exception("No amenity elements found");

                // Click second element
                var eleAmenitiesData = driver.FindElement(By.Id("activeAmenity(1)"));
                Thread.Sleep(2000);
                eleAmenitiesData.Click();
                if (eleAmenitiesData != null)
                {
                    Thread.Sleep(2000);
                    driver.ExecuteScript("goToCommonAmenity('AMN-05')");
                }
                Thread.Sleep(6000);
                logs.AppendLine($"[{DateTime.Now}] ACTION - Clicked on second element");

                // Reset view
                driver.ExecuteScript("resetView('true')");
                Thread.Sleep(2000);
                logs.AppendLine($"[{DateTime.Now}] ACTION - Reset view");

                // Click 'Apartments'
                driver.ExecuteScript("apartmentV2()");
                Thread.Sleep(2000);
                logs.AppendLine($"[{DateTime.Now}] ACTION - Clicked on 'Apartments'");
                var eleApartments = driver.FindElement(By.Id("unitsListAdapter2")) ?? throw new Exception("No apartments found");

                // Click second apartment
                driver.ExecuteScript("goToLevel('XX02')");
                logs.AppendLine($"[{DateTime.Now}] ACTION - Clicked on second apartment");
                Thread.Sleep(10000);

                // Detect apartment
                var apartmentMenu = driver.FindElement(By.Id("roomMenuList")) ?? throw new Exception("Could not load apartment menu");

                // Click on kitchen in dollhouse
                var apartmentElement = driver.FindElement(By.Id("kitchen"));
                apartmentElement.Click();
                Thread.Sleep(4000);
                logs.AppendLine($"[{DateTime.Now}] ACTION - Click in dollhouse");

                // Reset view
                driver.ExecuteScript("resetView('true')");
                Thread.Sleep(3000);
                logs.AppendLine($"[{DateTime.Now}] ACTION - Reset view");

                // Pull up slider menu
                driver.ExecuteScript("slideHalfUpDollhouse()");
                Thread.Sleep(2000);
                logs.AppendLine($"[{DateTime.Now}] ACTION - Pull up apartment slider menu");

                // Exit apartment
                driver.ExecuteScript("goToLevel('Exterior')");
                Thread.Sleep(1500);
                logs.AppendLine($"[{DateTime.Now}] ACTION - Exit to exterior view");

                // Pull up slider menu
                Thread.Sleep(15000);
                var eleMenu = driver.FindElement(By.Id("chevronSlider"));
                eleMenu.Click();
                Thread.Sleep(2000);
                logs.AppendLine($"[{DateTime.Now}] ACTION - Pull up slider menu");

                // Exit stream
                var eleExit = driver.FindElement(By.Id("outreach-back-url"));
                Thread.Sleep(5000);
                eleExit.Click();
                logs.AppendLine($"[{DateTime.Now}] ACTION - Click to exit experience");

                Console.WriteLine("SUCCESS");
                logs.AppendLine($"[{DateTime.Now}] ENDING AUTOMATION");
                logs.AppendLine("---------------------------------------------");
                logs.AppendLine("RESULT : SUCCESS");

                File.AppendAllText(filePath, logs.ToString());
                logs.Clear();

                driver.Quit();
            }
            catch (Exception ex)
            {
                Console.WriteLine("FAILED");
                logs.AppendLine($"[{DateTime.Now}] ENDING AUTOMATION");
                logs.AppendLine("---------------------------------------------");
                logs.AppendLine("RESULT : FAILED");
                logs.AppendLine($"REASON : {ex.Message}");

                File.AppendAllText(filePath, logs.ToString());
                logs.Clear();

                driver.Quit();
            }
        }
    }
}