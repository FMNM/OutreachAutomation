using System;
using System.IO;
using System.Linq;
using System.Text;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using OutreachAutomation.SeleniumBot.DTO.Browsers;
using OutreachAutomation.SeleniumBot.DTO.Mappings;
using static System.Threading.Thread;

namespace OutreachAutomation.SeleniumBot
{
    public static class Automate
    {
        public static void Script(WebDriver driver, string url, bool isRandom, MappingsDto mappings)
        {
            var logs = new StringBuilder();
            var logId = Guid.NewGuid().ToString().ToUpper();
            var filePath = Path.Combine(Environment.CurrentDirectory, Directory.CreateDirectory("TempLogs").ToString(), $"LOG-{logId}.txt");

            var retry = 0;

            var random = new Random();
            try
            {
                var browser = new BrowserInfoDto
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

                if (url == null) throw new Exception("No invitation link found");

                driver.Navigate().GoToUrl(url);
                Sleep(3000);
                // Retries 3 times
                while (driver.Url == null && retry < 3)
                {
                    logs.AppendLine($"[{DateTime.Now}] ACTION - Connection failed to invitation link, {url}");
                    driver.Navigate().Refresh();
                    Sleep(3000);
                    logs.AppendLine($"[{DateTime.Now}] ACTION - Retrying...");
                    retry++;
                }

                if (driver.Url == null) throw new Exception("Could not connect");

                logs.AppendLine($"[{DateTime.Now}] ACTION - Connected to invitation link, {url}");

                // Find the Swipe Up body
                var ele = driver.FindElement(By.Id("upArrow"));
                ele.Click();
                Sleep(2000);
                logs.AppendLine($"[{DateTime.Now}] ACTION - Swipe Up");

                // Find Country code element
                var ele1 = driver.FindElement(By.Name("country_code"));
                ele1.Click();
                ele1.SendKeys("+880");
                ele1.Click();
                Sleep(1000);
                logs.AppendLine($"[{DateTime.Now}] ACTION - Selected country code");

                // Find Phone number input element
                var ele2 = driver.FindElement(By.Name("phone_number"));

                var number = Generator.GetPhoneNumber();
                var SavedNumber = mappings.Logins[random.Next(mappings.Logins.Count)].phone_number;
                if (random.Next(0, 2) == 1)
                {
                    number = SavedNumber;
                }
                ele2.SendKeys(number);
                Sleep(1000);
                logs.AppendLine($"[{DateTime.Now}] ACTION - Entered phone number, {number}");

                // Click for Send OTP
                var ele3 = driver.FindElement(By.ClassName("btn-sendotp"));
                ele3.Click();
                Sleep(2000);
                logs.AppendLine($"[{DateTime.Now}] ACTION - Requested OTP");

                // Enter Dummy OTP
                var ele4 = driver.FindElement(By.Name("otp"));
                ele4.SendKeys("123456");
                Sleep(1000);
                logs.AppendLine($"[{DateTime.Now}] ACTION - Entered dummy OTP, 123456");

                // Verify OTP
                var ele5 = driver.FindElement(By.ClassName("btn-sendotp"));
                ele5.Click();
                Sleep(3000);
                logs.AppendLine($"[{DateTime.Now}] ACTION - Click button to verify OTP");

                switch (driver.Url)
                {
                    // In the case that the dummy number is unregistered, the bot will register into the system
                    case "https://outreach.ophs.io/register":
                        {
                            logs.AppendLine($"[{DateTime.Now}] DETECTED Unregistered user. PROCEEDING Registration");
                            var elementName = driver.FindElement(By.Name("name"));
                            var dName = $"Ophs Selenium Bot - {random.Next(50000)}";
                            elementName.SendKeys(dName);
                            Sleep(1500);
                            logs.AppendLine($"[{DateTime.Now}] ACTION - Enter dummy name, {dName}");

                            var elementAge = driver.FindElement(By.Name("age"));
                            var dAge = random.Next(18, 100).ToString();
                            elementAge.SendKeys(dAge);
                            Sleep(1000);

                            Sleep(1500);
                            logs.AppendLine($"[{DateTime.Now}] ACTION - Enter dummy age, {dAge}");

                            var elementGender = driver.FindElement(By.Id("dont_disclose"));
                            elementGender.Click();
                            Sleep(2000);
                            logs.AppendLine($"[{DateTime.Now}] ACTION - Selected undisclosed gender");

                            var elementNext = driver.FindElement(By.ClassName("btn-next"));
                            elementNext.Click();
                            Sleep(3000);
                            logs.AppendLine($"[{DateTime.Now}] ACTION - Click button to register");
                            break;
                        }
                    default:
                        logs.AppendLine($"[{DateTime.Now}] DETECTED Registered user");
                        break;
                }

                // Click Enter Experience
                logs.AppendLine($"[{DateTime.Now}] REACHED Home Page");
                Sleep(1500);
                switch (driver.Url)
                {
                    case "https://outreach.ophs.io/home-buyer":
                        var ele6 = driver.FindElement(By.Id("enter-ex-1"));
                        try
                        {
                            ele6.Click();
                            Sleep(1500);
                        }
                        catch (Exception)
                        {
                            ele6 = driver.FindElement(By.Id("enter-ex-2"));
                            Sleep(1500);
                            ele6.Click();
                        }

                        break;
                    default:
                        ele6 = driver.FindElement(By.Id("enterExperience"));
                        Sleep(1500);
                        ele6.Click();
                        break;
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
                Sleep(5000);
                try
                {
                    var ele7 = driver.FindElement(By.Id("main"));
                    if (ele7 == null)
                    {
                        driver.Navigate().Refresh();
                        Sleep(8000);

                        ele7 = driver.FindElement(By.Id("main"));
                        if (ele7 == null)
                        {
                            throw new Exception("Could not enter instance");
                        }
                    }

                    ele7.Click();
                    Sleep(2000);
                    logs.AppendLine($"[{DateTime.Now}] ACTION - Click to enter");

                    // Skip tutorial (if any)
                    var ele8 = driver.FindElement(By.ClassName("text-cyan"));
                    if (ele8 != null)
                    {
                        driver.ExecuteScript("hideAllTutorials()");
                        Sleep(2000);
                        logs.AppendLine($"[{DateTime.Now}] ACTION - Skip tutorial");
                    }
                }
                catch (Exception)
                {
                    driver.Navigate().Refresh();

                    Sleep(6000);
                    try
                    {
                        var ele7 = driver.FindElement(By.Id("main"));
                        if (ele7 == null)
                        {
                            driver.Navigate().Refresh();
                            Sleep(8000);

                            ele7 = driver.FindElement(By.Id("main"));
                            if (ele7 == null)
                            {
                                throw new Exception("Could not enter instance");
                            }
                        }

                        ele7.Click();
                        Sleep(2000);
                        logs.AppendLine($"[{DateTime.Now}] ACTION - Click to enter");

                        // Skip tutorial (if any)
                        var ele8 = driver.FindElement(By.ClassName("text-cyan"));
                        if (ele8 != null)
                        {
                            driver.ExecuteScript("hideAllTutorials()");
                            Sleep(2000);
                            logs.AppendLine($"[{DateTime.Now}] ACTION - Skip tutorial");
                        }
                    }
                    catch (Exception)
                    {
                        throw new Exception("Failed to connect to stream");
                    }
                }

                // Click 'Nearby'
                driver.ExecuteScript("nearbyV2()");
                Sleep(2000);
                logs.AppendLine($"[{DateTime.Now}] ACTION - Clicked on 'Nearby'");
                var eleNearby = driver.FindElement(By.Id("nearbyListAdapter2")) ??
                                throw new Exception("No nearby elements found");

                var nearbyNames = eleNearby.Text.Split("\r\n")
                    .Where(x => !x.ToLower().Contains("minute"))
                    .ToHashSet()
                    .ToList();

                // Click on nearby elements
                for (var i = 0; i < nearbyNames.Count; i++)
                {
                    var eleNearbyData =
                        driver.FindElement(By.Id($"activeNearbyId{i}"));
                    Sleep(2000);
                    eleNearbyData.Click();
                    Sleep(6000);
                    logs.AppendLine($"[{DateTime.Now}] ACTION - Clicked on '{nearbyNames[i]}'");
                }

                // Reset view
                driver.ExecuteScript("resetView('true')");
                Sleep(2000);
                logs.AppendLine($"[{DateTime.Now}] ACTION - Reset view");

                // Click 'Amenities'
                driver.ExecuteScript("amenityV2()");
                Sleep(2000);
                logs.AppendLine($"[{DateTime.Now}] ACTION - Clicked on 'Amenities'");
                var eleAmenities = driver.FindElement(By.Id("amenityListAdapter2")) ??
                                   throw new Exception("No amenity elements found");

                var amenityNames = eleAmenities.Text.Split("\r\n")
                    .Where(x => !x.ToLower().Contains("take") && !x.ToLower().Contains("tap"))
                    .ToHashSet()
                    .Where((x, i) => i % 2 == 0)
                    .ToList();

                // Parse by all amenities
                foreach (var amenity in mappings.Amenities)
                {
                    if (!amenityNames.Contains(amenity.Title)) continue;
                    var eleAmenitiesData = driver.FindElement(By.Id($"{amenity.HtmlId}"));
                    eleAmenitiesData.Click();
                    Sleep(2000);
                    driver.ExecuteScript($"goToCommonAmenity('{amenity.AmenityId}')");
                    Sleep(6000);
                    logs.AppendLine($"[{DateTime.Now}] ACTION - Clicked on amenity '{amenity.Title}'");
                }

                // Reset view
                driver.ExecuteScript("resetView('true')");
                Sleep(2000);
                logs.AppendLine($"[{DateTime.Now}] ACTION - Reset view");

                // Click 'Apartments'
                driver.ExecuteScript("apartmentV2()");
                Sleep(2000);
                logs.AppendLine($"[{DateTime.Now}] ACTION - Clicked on 'Apartments'");
                var eleApartments = driver.FindElement(By.Id("unitsListAdapter2")) ??
                                    throw new Exception("No apartments found");

                var apartmentNames = eleApartments.Text.Split("\r\n")
                    .Where(x => !x.ToLower().Contains("|"))
                    .ToHashSet()
                    .ToList();

                for (var i = 1; i <= apartmentNames.Count; i++)
                {
                    // Click on apartment
                    driver.ExecuteScript($"goToLevel('XX{i:D2}')");
                    logs.AppendLine($"[{DateTime.Now}] ACTION - Clicked on apartment, {apartmentNames[i - 1]} ");
                    Sleep(12000);

                    // Detect apartment menu
                    var apartmentMenu = driver.FindElement(By.Id("roomMenuList")) ??
                                        throw new Exception("Could not load apartment menu");

                    // Click on kitchen in dollhouse
                    var apartmentElement = driver.FindElement(By.Id("kitchen"));
                    apartmentElement.Click();
                    Sleep(4000);
                    logs.AppendLine($"[{DateTime.Now}] ACTION - Click on 'Kitchen'");

                    // Reset view
                    driver.ExecuteScript("resetView('true')");
                    Sleep(3000);
                    logs.AppendLine($"[{DateTime.Now}] ACTION - Reset view");

                    // Pull up slider menu
                    driver.ExecuteScript("slideHalfUpDollhouse()");
                    Sleep(2000);
                    logs.AppendLine($"[{DateTime.Now}] ACTION - Pull up apartment slider menu");
                }

                // Exit apartment
                driver.ExecuteScript("goToLevel('Exterior')");
                Sleep(1500);
                logs.AppendLine($"[{DateTime.Now}] ACTION - Exit to exterior view");

                // Pull up slider menu
                Sleep(15000);
                var eleMenu = driver.FindElement(By.Id("chevronSlider"));
                eleMenu.Click();
                Sleep(2000);
                logs.AppendLine($"[{DateTime.Now}] ACTION - Pull up slider menu");

                // Exit stream
                var eleExit = driver.FindElement(By.Id("outreach-back-url"));
                Sleep(2000);
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