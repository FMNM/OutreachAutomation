using System;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using OutreachAutomation.SeleniumBot.DTO;
using OutreachAutomation.SeleniumBot.DTO.Browsers;
using static System.Threading.Thread;
using static OutreachAutomation.SeleniumBot.Generator;

namespace OutreachAutomation.SeleniumBot
{
    public static class Automate
    {
        public static void Script(GeneralDto data)
        {
            try
            {
                var browser = new BrowserInfoDto
                {
                    Name = data.Driver.Capabilities.GetCapability("browserName").ToString(),
                    Version = data.Driver.Capabilities.GetCapability("browserVersion").ToString()
                };

                AddLog($"Selenium WebDriver Bot, Log - {data.LogInfo.LogId}", data.Path);
                AddLog("---------------------------------------------", data.Path);
                AddLog($"BROWSER : {browser.Name}", data.Path);
                AddLog($"VERSION : {browser.Version}", data.Path);
                AddLog("---------------------------------------------", data.Path);
                AddLog($"[{DateTime.Now}] STARTING AUTOMATION", data.Path);

                if (data.Url == null) throw new Exception("No invitation link found");

                WebSegment(data);

                StreamSegment(data);

                Console.WriteLine("SUCCESS");

                AddLog($"[{DateTime.Now}] ENDING AUTOMATION", data.Path);
                AddLog("---------------------------------------------", data.Path);
                AddLog("RESULT : SUCCESS", data.Path);
                AddLog("---------------------------------------------", data.Path);

                data.Driver.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("FAILED");
                AddLog($"[{DateTime.Now}] ENDING AUTOMATION", data.Path);
                AddLog("---------------------------------------------", data.Path);
                AddLog("RESULT : FAILED", data.Path);
                AddLog($"REASON : {ex.Message}", data.Path);
                AddLog("---------------------------------------------", data.Path);

                data.Driver.Close();
            }
        }

        private static void WebSegment(GeneralDto data)
        {
            var random = new Random();
            var wait = new WebDriverWait(data.Driver, TimeSpan.FromSeconds(210));

            data.Driver.Navigate().GoToUrl(data.Url);
            AddLog($"[{DateTime.Now}] ACTION - Connected to invitation link, {data.Url}", data.Path);
            Sleep(5000);

            // Find the Swipe Up body
            var ele = wait.Until(_ => data.Driver.FindElement(By.Id("upArrow")));
            ele.Click();
            Sleep(2000);
            AddLog($"[{DateTime.Now}] ACTION - Swipe Up", data.Path);

            // Find Country code element
            var ele1 = data.Driver.FindElement(By.Name("country_code"));
            ele1.Click();
            ele1.SendKeys("+880");
            ele1.Click();
            Sleep(2000);
            AddLog($"[{DateTime.Now}] ACTION - Selected country code", data.Path);

            // Find Phone number input element
            var ele2 = data.Driver.FindElement(By.Name("phone_number"));

            var number = GetPhoneNumber();
            var savedNumber = data.Mappings.Logins[random.Next(data.Mappings.Logins.Count)].phone_number;
            if (random.Next(0, 2) == 1) number = savedNumber;

            ele2.SendKeys(number);
            Sleep(2000);
            AddLog($"[{DateTime.Now}] ACTION - Entered phone number, {number}", data.Path);

            // Click for Send OTP
            var ele3 = data.Driver.FindElement(By.ClassName("btn-sendotp"));
            ele3.Click();
            Sleep(2000);
            AddLog($"[{DateTime.Now}] ACTION - Requested OTP", data.Path);

            // Enter Dummy OTP
            var ele4 = data.Driver.FindElement(By.Name("otp"));
            ele4.SendKeys("123456");
            Sleep(2000);
            AddLog($"[{DateTime.Now}] ACTION - Entered dummy OTP, 123456", data.Path);

            // Verify OTP
            var ele5 = data.Driver.FindElement(By.ClassName("btn-sendotp"));
            ele5.Click();
            Sleep(3000);
            AddLog($"[{DateTime.Now}] ACTION - Click button to verify OTP", data.Path);

            switch (data.Driver.Url)
            {
                // In the case that the dummy number is unregistered, the bot will register into the system
                case "https://outreach.ophs.io/register":
                {
                    AddLog($"[{DateTime.Now}] DETECTED Unregistered user. PROCEEDING Registration", data.Path);
                    var elementName = data.Driver.FindElement(By.Name("name"));
                    var dName = $"Ophs Selenium Bot - {random.Next(50000)}";
                    elementName.SendKeys(dName);
                    AddLog($"[{DateTime.Now}] ACTION - Enter dummy name, {dName}", data.Path);
                    Sleep(1500);

                    var elementAge = data.Driver.FindElement(By.Name("age"));
                    var dAge = random.Next(18, 100).ToString();
                    elementAge.SendKeys(dAge);
                    AddLog($"[{DateTime.Now}] ACTION - Enter dummy age, {dAge}", data.Path);
                    Sleep(1500);

                    var elementGender = data.Driver.FindElement(By.Id("dont_disclose"));
                    elementGender.Click();
                    AddLog($"[{DateTime.Now}] ACTION - Selected undisclosed gender", data.Path);
                    Sleep(1500);

                    var elementNext = data.Driver.FindElement(By.ClassName("btn-next"));
                    elementNext.Click();
                    AddLog($"[{DateTime.Now}] ACTION - Click button to register", data.Path);
                    Sleep(3000);
                    break;
                }
                default:
                    AddLog($"[{DateTime.Now}] DETECTED Registered user, {data.Mappings.Logins.FirstOrDefault(x => x.phone_number == savedNumber)?.name}", data.Path);
                    break;
            }

            // Click Enter Experience
            var o = wait.Until(_ => data.Driver.Url.Contains("home"));
            if (!o) throw new Exception("Could not find home page");

            AddLog($"[{DateTime.Now}] REACHED Home Page", data.Path);
            Sleep(1500);
            switch (data.Driver.Url)
            {
                case "https://outreach.ophs.io/home-buyer":
                    var ele6 = data.Driver.FindElement(By.Id("enter-ex-1"));
                    try
                    {
                        ele6.Click();
                        Sleep(1500);
                    }
                    catch (Exception)
                    {
                        ele6 = data.Driver.FindElement(By.Id("enter-ex-2"));
                        Sleep(1500);
                        ele6.Click();
                    }

                    break;
                default:
                    ele6 = data.Driver.FindElement(By.Id("enterExperience"));
                    Sleep(1500);
                    ele6.Click();
                    break;
            }

            AddLog($"[{DateTime.Now}] ACTION - Click to Enter Experience", data.Path);
            Sleep(2000);

            if (!data.Driver.Url.ToLower().Contains("experience")) throw new Exception("Could not connect to server");
        }

        private static void StreamSegment(GeneralDto data)
        {
            var random = new Random();
            var wait = new WebDriverWait(data.Driver, TimeSpan.FromSeconds(210));

            // Find instance
            wait.Until(_ => data.Driver.Url.Contains("stream"));

            // Tap to continue
            Sleep(5000);
            var retry = 3;
            try
            {
                while (retry > 0)
                {
                    try
                    {
                        var ele7 = wait.Until(_ => data.Driver.FindElement(By.Id("main"))) ?? throw new Exception("Could not enter instance");
                        ele7.Click();
                        AddLog($"[{DateTime.Now}] ACTION - Click to enter", data.Path);
                        Sleep(2000);

                        // Skip tutorial (if any)
                        data.Driver.ExecuteScript("hideAllTutorials()");
                        Sleep(2000);

                        break;
                    }
                    catch (Exception)
                    {
                        data.Driver.Navigate().Refresh();
                        Sleep(6000);

                        retry--;
                    }
                }
            }
            catch (Exception)
            {
                throw new Exception("Failed to connect to stream");
            }

            Swipe(data.Driver);

            // Click 'Nearby'
            data.Driver.ExecuteScript("nearbyV2()");
            Sleep(2000);
            AddLog($"[{DateTime.Now}] ACTION - Clicked on 'Nearby'", data.Path);
            var eleNearby = data.Driver.FindElement(By.Id("nearbyListAdapter2")) ?? throw new Exception("No nearby elements found");

            var nearbyNames = eleNearby.Text.Split("\r\n").Where(x => !x.ToLower().Contains("minute")).ToHashSet().ToList();

            // Click on nearby elements
            if (data.IsRandom)
            {
                var randomCount = random.Next(nearbyNames.Count);
                retry = 0;

                while (retry < randomCount)
                {
                    var randomId = random.Next(randomCount);
                    var eleNearbyData = data.Driver.FindElement(By.Id($"activeNearbyId{randomId}"));
                    Sleep(2000);
                    eleNearbyData.Click();
                    Sleep(6000);
                    AddLog($"[{DateTime.Now}] ACTION - Clicked on nearby unit, '{nearbyNames[randomId]}'", data.Path);

                    retry++;
                }
            }
            else
            {
                for (var i = 0; i < nearbyNames.Count; i++)
                {
                    var eleNearbyData = data.Driver.FindElement(By.Id($"activeNearbyId{i}"));
                    Sleep(2000);
                    eleNearbyData.Click();
                    Sleep(6000);
                    AddLog($"[{DateTime.Now}] ACTION - Clicked on nearby unit, '{nearbyNames[i]}'", data.Path);
                }
            }

            // Reset view
            data.Driver.ExecuteScript("resetView('true')");
            Sleep(2000);
            AddLog($"[{DateTime.Now}] ACTION - Reset view", data.Path);

            // Click 'Amenities'
            data.Driver.ExecuteScript("amenityV2()");
            Sleep(2000);
            AddLog($"[{DateTime.Now}] ACTION - Clicked on 'Amenities'", data.Path);
            var eleAmenities = data.Driver.FindElement(By.Id("amenityListAdapter2")) ?? throw new Exception("No amenity elements found");

            var amenityNames = eleAmenities.Text.Split("\r\n").Where(x => !x.ToLower().Contains("take") && !x.ToLower().Contains("tap"))
                .ToHashSet()
                .Where((_, i) => i % 2 == 0)
                .ToList();

            // Parse by all amenities
            if (data.IsRandom)
            {
                var randomCount = random.Next(data.Mappings.Amenities.Count);
                retry = 0;

                while (retry < randomCount)
                {
                    var randomId = random.Next(randomCount);
                    if (!amenityNames.Contains(data.Mappings.Amenities[randomId].Title)) continue;
                    var eleAmenitiesData = data.Driver.FindElement(By.Id($"{data.Mappings.Amenities[randomId].HtmlId}"));
                    eleAmenitiesData.Click();
                    Sleep(2000);
                    data.Driver.ExecuteScript($"goToCommonAmenity('{data.Mappings.Amenities[randomId].AmenityId}')");
                    Sleep(6000);
                    AddLog(
                        $"[{DateTime.Now}] ACTION - Clicked on amenity unit, '{data.Mappings.Amenities[randomId].Title}'", data.Path);

                    Swipe(data.Driver);

                    retry++;
                }
            }
            else
            {
                foreach (var amenity in data.Mappings.Amenities)
                {
                    if (!amenityNames.Contains(amenity.Title)) continue;
                    var eleAmenitiesData = data.Driver.FindElement(By.Id($"{amenity.HtmlId}"));
                    eleAmenitiesData.Click();
                    Sleep(2000);
                    data.Driver.ExecuteScript($"goToCommonAmenity('{amenity.AmenityId}')");
                    Sleep(6000);
                    AddLog($"[{DateTime.Now}] ACTION - Clicked on amenity unit, '{amenity.Title}'", data.Path);

                    Swipe(data.Driver);
                }
            }

            // Reset view
            data.Driver.ExecuteScript("resetView('true')");
            Sleep(2000);
            AddLog($"[{DateTime.Now}] ACTION - Reset view", data.Path);

            // Click 'Apartments'
            data.Driver.ExecuteScript("apartmentV2()");
            AddLog($"[{DateTime.Now}] ACTION - Clicked on 'Apartments'", data.Path);
            Sleep(2000);
            wait.Until(_ => data.Driver.FindElement(By.Id("unitsListAdapter2")) ?? throw new Exception("No apartments found"));

            if (data.IsRandom)
            {
                var randomCount = random.Next(data.Mappings.Apartments.Count);
                retry = 0;

                while (retry < randomCount)
                {
                    var randomId = random.Next(randomCount);
                    // Click on apartment
                    data.Driver.ExecuteScript($"goToLevel('{data.Mappings.Apartments[randomId].LevelId}')");
                    AddLog($"[{DateTime.Now}] ACTION - Clicked on apartment, '{data.Mappings.Apartments[randomId].LevelName}'", data.Path);
                    Sleep(12000);

                    wait.Until(_ => data.Driver.FindElement(By.Id("roomMenuList")) ?? throw new Exception("Could not load apartment menu"));

                    Swipe(data.Driver);

                    foreach (var room in data.Mappings.Apartments[randomId].Rooms)
                    {
                        // Click on kitchen in dollhouse
                        var apartmentElement = wait.Until(_ => data.Driver.FindElement(By.Id(room.Id)));
                        apartmentElement.Click();
                        AddLog($"[{DateTime.Now}] ACTION - Click on '{room.Name}'", data.Path);
                        Sleep(4000);
                    }

                    // Reset view
                    data.Driver.ExecuteScript("resetView('true')");
                    AddLog($"[{DateTime.Now}] ACTION - Reset view", data.Path);
                    Sleep(3000);

                    // Pull up slider menu
                    data.Driver.ExecuteScript("slideHalfUpDollhouse()");
                    AddLog($"[{DateTime.Now}] ACTION - Pull up apartment slider menu", data.Path);
                    Sleep(2000);

                    retry++;
                }
            }
            else
            {
                foreach (var apartment in data.Mappings.Apartments)
                {
                    // Click on apartment
                    data.Driver.ExecuteScript($"goToLevel('{apartment.LevelId}')");
                    AddLog($"[{DateTime.Now}] ACTION - Clicked on apartment, '{apartment.LevelName}'", data.Path);
                    Sleep(12000);

                    // Detect apartment menu
                    wait.Until(_ => data.Driver.FindElement(By.Id("roomMenuList")) ?? throw new Exception("Could not load apartment menu"));

                    Swipe(data.Driver);

                    foreach (var room in apartment.Rooms)
                    {
                        // Click on element in dollhouse
                        var apartmentElement = wait.Until(_ => data.Driver.FindElement(By.Id(room.Id)));
                        apartmentElement.Click();
                        Sleep(4000);
                        AddLog($"[{DateTime.Now}] ACTION - Click on apartment room, '{room.Name}'", data.Path);
                    }

                    // Reset view
                    data.Driver.ExecuteScript("resetView('true')");
                    Sleep(3000);
                    AddLog($"[{DateTime.Now}] ACTION - Reset view", data.Path);

                    // Pull up slider menu
                    data.Driver.ExecuteScript("slideHalfUpDollhouse()");
                    Sleep(2000);
                    AddLog($"[{DateTime.Now}] ACTION - Pull up apartment slider menu", data.Path);
                }
            }

            // Exit apartment
            data.Driver.ExecuteScript("goToLevel('Exterior')");
            Sleep(1500);
            AddLog($"[{DateTime.Now}] ACTION - Exit to exterior view", data.Path);

            // Pull up slider menu
            Sleep(15000);
            var eleMenu = data.Driver.FindElement(By.Id("chevronSlider"));
            eleMenu.Click();
            Sleep(2000);
            AddLog($"[{DateTime.Now}] ACTION - Pull up slider menu", data.Path);

            // Exit stream
            var eleExit = data.Driver.FindElement(By.Id("outreach-back-url"));
            Sleep(2000);
            eleExit.Click();
            AddLog($"[{DateTime.Now}] ACTION - Click to exit experience", data.Path);
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