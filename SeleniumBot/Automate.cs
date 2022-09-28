using System;
using System.IO;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using OutreachAutomation.SeleniumBot.DTO.Browsers;
using OutreachAutomation.SeleniumBot.DTO.Mappings;
using OutreachAutomation.SeleniumBot.DTO.SessionLogs;
using static System.Threading.Thread;

namespace OutreachAutomation.SeleniumBot
{
    public static class Automate
    {
        public static void Script(WebDriver driver, string url, bool isRandom, MappingsDto mappings)
        {
            var sessionLog = Generator.GenerateLogs();
            try
            {
                var browser = new BrowserInfoDto
                {
                    Name = driver.Capabilities.GetCapability("browserName").ToString(),
                    Version = driver.Capabilities.GetCapability("browserVersion").ToString()
                };

                sessionLog.Logs.AppendLine($"Selenium WebDriver Bot, Log - {sessionLog.LogId}");
                sessionLog.Logs.AppendLine("---------------------------------------------");
                sessionLog.Logs.AppendLine($"BROWSER : {browser.Name}");
                sessionLog.Logs.AppendLine($"VERSION : {browser.Version}");
                sessionLog.Logs.AppendLine("---------------------------------------------");
                sessionLog.Logs.AppendLine($"[{DateTime.Now}] STARTING AUTOMATION");

                if (url == null) throw new Exception("No invitation link found");

                WebSegment(driver, url, mappings, sessionLog);

                StreamSegment(driver, isRandom, mappings, sessionLog);

                Console.WriteLine("SUCCESS");

                sessionLog.Logs.AppendLine($"[{DateTime.Now}] ENDING AUTOMATION");
                sessionLog.Logs.AppendLine("---------------------------------------------");
                sessionLog.Logs.AppendLine("RESULT : SUCCESS");
                sessionLog.Logs.AppendLine("---------------------------------------------");

                File.AppendAllText(sessionLog.FilePath, sessionLog.Logs.ToString());
                sessionLog.Logs.Clear();

                driver.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("FAILED");
                sessionLog.Logs.AppendLine($"[{DateTime.Now}] ENDING AUTOMATION");
                sessionLog.Logs.AppendLine("---------------------------------------------");
                sessionLog.Logs.AppendLine("RESULT : FAILED");
                sessionLog.Logs.AppendLine($"REASON : {ex.Message}");
                sessionLog.Logs.AppendLine("---------------------------------------------");

                File.AppendAllText(sessionLog.FilePath, sessionLog.Logs.ToString());
                sessionLog.Logs.Clear();

                driver.Close();
            }
        }

        private static void WebSegment(WebDriver driver, string url, MappingsDto mappings, LogDto sessionLog)
        {
            var random = new Random();
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(210));

            driver.Navigate().GoToUrl(url);
            sessionLog.Logs.AppendLine($"[{DateTime.Now}] ACTION - Connected to invitation link, {url}");
            Sleep(5000);

            // Find the Swipe Up body
            var ele = wait.Until(_ => driver.FindElement(By.Id("upArrow")));
            ele.Click();
            Sleep(2000);
            sessionLog.Logs.AppendLine($"[{DateTime.Now}] ACTION - Swipe Up");

            // Find Country code element
            var ele1 = driver.FindElement(By.Name("country_code"));
            ele1.Click();
            ele1.SendKeys("+880");
            ele1.Click();
            Sleep(1000);
            sessionLog.Logs.AppendLine($"[{DateTime.Now}] ACTION - Selected country code");

            // Find Phone number input element
            var ele2 = driver.FindElement(By.Name("phone_number"));

            var number = Generator.GetPhoneNumber();
            var savedNumber = mappings.Logins[random.Next(mappings.Logins.Count)].phone_number;
            if (random.Next(0, 2) == 1) number = savedNumber;

            ele2.SendKeys(number);
            Sleep(1000);
            sessionLog.Logs.AppendLine($"[{DateTime.Now}] ACTION - Entered phone number, {number}");

            // Click for Send OTP
            var ele3 = driver.FindElement(By.ClassName("btn-sendotp"));
            ele3.Click();
            Sleep(2000);
            sessionLog.Logs.AppendLine($"[{DateTime.Now}] ACTION - Requested OTP");

            // Enter Dummy OTP
            var ele4 = driver.FindElement(By.Name("otp"));
            ele4.SendKeys("123456");
            Sleep(1000);
            sessionLog.Logs.AppendLine($"[{DateTime.Now}] ACTION - Entered dummy OTP, 123456");

            // Verify OTP
            var ele5 = driver.FindElement(By.ClassName("btn-sendotp"));
            ele5.Click();
            Sleep(3000);
            sessionLog.Logs.AppendLine($"[{DateTime.Now}] ACTION - Click button to verify OTP");

            switch (driver.Url)
            {
                // In the case that the dummy number is unregistered, the bot will register into the system
                case "https://outreach.ophs.io/register":
                {
                    sessionLog.Logs.AppendLine($"[{DateTime.Now}] DETECTED Unregistered user. PROCEEDING Registration");
                    var elementName = driver.FindElement(By.Name("name"));
                    var dName = $"Ophs Selenium Bot - {random.Next(50000)}";
                    elementName.SendKeys(dName);
                    sessionLog.Logs.AppendLine($"[{DateTime.Now}] ACTION - Enter dummy name, {dName}");
                    Sleep(1500);

                    var elementAge = driver.FindElement(By.Name("age"));
                    var dAge = random.Next(18, 100).ToString();
                    elementAge.SendKeys(dAge);
                    sessionLog.Logs.AppendLine($"[{DateTime.Now}] ACTION - Enter dummy age, {dAge}");
                    Sleep(1500);

                    var elementGender = driver.FindElement(By.Id("dont_disclose"));
                    elementGender.Click();
                    sessionLog.Logs.AppendLine($"[{DateTime.Now}] ACTION - Selected undisclosed gender");
                    Sleep(1500);

                    var elementNext = driver.FindElement(By.ClassName("btn-next"));
                    elementNext.Click();
                    sessionLog.Logs.AppendLine($"[{DateTime.Now}] ACTION - Click button to register");
                    Sleep(3000);
                    break;
                }
                default:
                    sessionLog.Logs.AppendLine(
                        $"[{DateTime.Now}] DETECTED Registered user, {mappings.Logins.FirstOrDefault(x => x.phone_number == savedNumber)?.name}");
                    break;
            }

            // Click Enter Experience
            var o = wait.Until(_ => driver.Url.Contains("home"));
            if (!o) throw new Exception("Could not find home page");

            sessionLog.Logs.AppendLine($"[{DateTime.Now}] REACHED Home Page");
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

            sessionLog.Logs.AppendLine($"[{DateTime.Now}] ACTION - Click to Enter Experience");
        }

        private static void StreamSegment(WebDriver driver, bool isRandom, MappingsDto mappings, LogDto sessionLog)
        {
            var random = new Random();
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(210));

            // Find instance
            wait.Until(_ => driver.Url.Contains("stream"));

            // Tap to continue
            Sleep(5000);
            var retry = 3;
            try
            {
                while (retry > 0)
                {
                    try
                    {
                        var ele7 = wait.Until(_ => driver.FindElement(By.Id("main"))) ??
                                   throw new Exception("Could not enter instance");
                        ele7.Click();
                        sessionLog.Logs.AppendLine($"[{DateTime.Now}] ACTION - Click to enter");
                        Sleep(2000);

                        // Skip tutorial (if any)
                        driver.ExecuteScript("hideAllTutorials()");
                        Sleep(2000);

                        break;
                    }
                    catch (Exception)
                    {
                        driver.Navigate().Refresh();
                        Sleep(6000);

                        retry--;
                    }
                }
            }
            catch (Exception)
            {
                throw new Exception("Failed to connect to stream");
            }

            Swipe(driver);

            // Click 'Nearby'
            driver.ExecuteScript("nearbyV2()");
            Sleep(2000);
            sessionLog.Logs.AppendLine($"[{DateTime.Now}] ACTION - Clicked on 'Nearby'");
            var eleNearby = driver.FindElement(By.Id("nearbyListAdapter2")) ??
                            throw new Exception("No nearby elements found");

            var nearbyNames = eleNearby.Text.Split("\r\n").Where(x => !x.ToLower().Contains("minute")).ToHashSet()
                .ToList();

            // Click on nearby elements
            if (isRandom)
            {
                var randomCount = random.Next(nearbyNames.Count);
                retry = 0;

                while (retry < randomCount)
                {
                    var randomId = random.Next(randomCount);
                    var eleNearbyData = driver.FindElement(By.Id($"activeNearbyId{randomId}"));
                    Sleep(2000);
                    eleNearbyData.Click();
                    Sleep(6000);
                    sessionLog.Logs.AppendLine($"[{DateTime.Now}] ACTION - Clicked on '{nearbyNames[randomId]}'");

                    retry++;
                }
            }
            else
            {
                for (var i = 0; i < nearbyNames.Count; i++)
                {
                    var eleNearbyData = driver.FindElement(By.Id($"activeNearbyId{i}"));
                    Sleep(2000);
                    eleNearbyData.Click();
                    Sleep(6000);
                    sessionLog.Logs.AppendLine($"[{DateTime.Now}] ACTION - Clicked on '{nearbyNames[i]}'");
                }
            }

            // Reset view
            driver.ExecuteScript("resetView('true')");
            Sleep(2000);
            sessionLog.Logs.AppendLine($"[{DateTime.Now}] ACTION - Reset view");

            // Click 'Amenities'
            driver.ExecuteScript("amenityV2()");
            Sleep(2000);
            sessionLog.Logs.AppendLine($"[{DateTime.Now}] ACTION - Clicked on 'Amenities'");
            var eleAmenities = driver.FindElement(By.Id("amenityListAdapter2")) ??
                               throw new Exception("No amenity elements found");

            var amenityNames = eleAmenities.Text.Split("\r\n")
                .Where(x => !x.ToLower().Contains("take") && !x.ToLower().Contains("tap"))
                .ToHashSet()
                .Where((_, i) => i % 2 == 0)
                .ToList();

            // Parse by all amenities
            if (isRandom)
            {
                var randomCount = random.Next(mappings.Amenities.Count);
                retry = 0;

                while (retry < randomCount)
                {
                    var randomId = random.Next(randomCount);
                    if (!amenityNames.Contains(mappings.Amenities[randomId].Title)) continue;
                    var eleAmenitiesData = driver.FindElement(By.Id($"{mappings.Amenities[randomId].HtmlId}"));
                    eleAmenitiesData.Click();
                    Sleep(2000);
                    driver.ExecuteScript($"goToCommonAmenity('{mappings.Amenities[randomId].AmenityId}')");
                    Sleep(6000);
                    sessionLog.Logs.AppendLine(
                        $"[{DateTime.Now}] ACTION - Clicked on amenity '{mappings.Amenities[randomId].Title}'");

                    Swipe(driver);

                    retry++;
                }
            }
            else
            {
                foreach (var amenity in mappings.Amenities)
                {
                    if (!amenityNames.Contains(amenity.Title)) continue;
                    var eleAmenitiesData = driver.FindElement(By.Id($"{amenity.HtmlId}"));
                    eleAmenitiesData.Click();
                    Sleep(2000);
                    driver.ExecuteScript($"goToCommonAmenity('{amenity.AmenityId}')");
                    Sleep(6000);
                    sessionLog.Logs.AppendLine($"[{DateTime.Now}] ACTION - Clicked on amenity '{amenity.Title}'");

                    Swipe(driver);
                }
            }

            // Reset view
            driver.ExecuteScript("resetView('true')");
            Sleep(2000);
            sessionLog.Logs.AppendLine($"[{DateTime.Now}] ACTION - Reset view");

            // Click 'Apartments'
            driver.ExecuteScript("apartmentV2()");
            sessionLog.Logs.AppendLine($"[{DateTime.Now}] ACTION - Clicked on 'Apartments'");
            Sleep(2000);
            wait.Until(_ => driver.FindElement(By.Id("unitsListAdapter2")) ??
                            throw new Exception("No apartments found"));

            if (isRandom)
            {
                var randomCount = random.Next(mappings.Apartments.Count);
                retry = 0;

                while (retry < randomCount)
                {
                    var randomId = random.Next(randomCount);
                    // Click on apartment
                    driver.ExecuteScript($"goToLevel('{mappings.Apartments[randomId].LevelId}')");
                    sessionLog.Logs.AppendLine(
                        $"[{DateTime.Now}] ACTION - Clicked on apartment, '{mappings.Apartments[randomId].LevelName}'");
                    Sleep(12000);

                    wait.Until(_ =>
                        driver.FindElement(By.Id("roomMenuList")) ??
                        throw new Exception("Could not load apartment menu"));

                    Swipe(driver);

                    foreach (var room in mappings.Apartments[randomId].Rooms)
                    {
                        // Click on kitchen in dollhouse
                        var apartmentElement = wait.Until(_ => driver.FindElement(By.Id(room.Id)));
                        apartmentElement.Click();
                        sessionLog.Logs.AppendLine($"[{DateTime.Now}] ACTION - Click on '{room.Name}'");
                        Sleep(4000);
                    }

                    // Reset view
                    driver.ExecuteScript("resetView('true')");
                    sessionLog.Logs.AppendLine($"[{DateTime.Now}] ACTION - Reset view");
                    Sleep(3000);

                    // Pull up slider menu
                    driver.ExecuteScript("slideHalfUpDollhouse()");
                    sessionLog.Logs.AppendLine($"[{DateTime.Now}] ACTION - Pull up apartment slider menu");
                    Sleep(2000);

                    retry++;
                }
            }
            else
            {
                foreach (var apartment in mappings.Apartments)
                {
                    // Click on apartment
                    driver.ExecuteScript($"goToLevel('{apartment.LevelId}')");
                    sessionLog.Logs.AppendLine(
                        $"[{DateTime.Now}] ACTION - Clicked on apartment, '{apartment.LevelName}'");
                    Sleep(12000);

                    // Detect apartment menu
                    wait.Until(_ =>
                        driver.FindElement(By.Id("roomMenuList")) ??
                        throw new Exception("Could not load apartment menu"));

                    Swipe(driver);

                    foreach (var room in apartment.Rooms)
                    {
                        // Click on element in dollhouse
                        var apartmentElement = wait.Until(_ => driver.FindElement(By.Id(room.Id)));
                        apartmentElement.Click();
                        Sleep(4000);
                        sessionLog.Logs.AppendLine($"[{DateTime.Now}] ACTION - Click on '{room.Name}'");
                    }

                    // Reset view
                    driver.ExecuteScript("resetView('true')");
                    Sleep(3000);
                    sessionLog.Logs.AppendLine($"[{DateTime.Now}] ACTION - Reset view");

                    // Pull up slider menu
                    driver.ExecuteScript("slideHalfUpDollhouse()");
                    Sleep(2000);
                    sessionLog.Logs.AppendLine($"[{DateTime.Now}] ACTION - Pull up apartment slider menu");
                }
            }

            // Exit apartment
            driver.ExecuteScript("goToLevel('Exterior')");
            Sleep(1500);
            sessionLog.Logs.AppendLine($"[{DateTime.Now}] ACTION - Exit to exterior view");

            // Pull up slider menu
            Sleep(15000);
            var eleMenu = driver.FindElement(By.Id("chevronSlider"));
            eleMenu.Click();
            Sleep(2000);
            sessionLog.Logs.AppendLine($"[{DateTime.Now}] ACTION - Pull up slider menu");

            // Exit stream
            var eleExit = driver.FindElement(By.Id("outreach-back-url"));
            Sleep(2000);
            eleExit.Click();
            sessionLog.Logs.AppendLine($"[{DateTime.Now}] ACTION - Click to exit experience");
        }

        private static void Swipe(WebDriver driver)
        {
            var action = new Actions(driver);
            action.DragAndDropToOffset(driver.FindElement(By.Id("streamingVideo")), 200, 0);
            action.Build().Perform();
            Sleep(3000);
        }
    }
}