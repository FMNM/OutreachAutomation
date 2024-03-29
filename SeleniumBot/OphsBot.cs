using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using OpenQA.Selenium;
using OutreachAutomation.SeleniumBot.DTO;
using OutreachAutomation.SeleniumBot.DTO.Mappings;

namespace OutreachAutomation.SeleniumBot
{
    public static class OphsBot
    {
        public static void Begin()
        {
            try
            {
                // Fetches some data for the rest of the script
                var mappings = Generator.GetMappings() ?? throw new Exception("Failed to load");

                // Basic UI for the script
                Console.WriteLine("Is a specific environment/instance test? (y/n) \n 'y' - Custom number of instances for specific link can be applied \n 'n' - It will open a single instance of every project in selected environment");
                var isSpecificTest = Console.ReadLine().ToLower() ?? "y";

                var delay = 60000;
                if (isSpecificTest == "n")
                {
                    Console.WriteLine("Default delay is 60 seconds between each instance. Add a custom timer? (y/n)");
                    var isCustomDelay = Console.ReadLine().ToLower() ?? "n";

                    if (isCustomDelay == "y")
                    {
                        Console.WriteLine("Please enter custom delay timer, in seconds");
                        delay = Convert.ToInt32(Console.ReadLine()) * 1000;
                    }
                }

                Console.WriteLine("Testing environment (a/b): \n(a) Staging \n(b) Production");
                var selectedEnvironment = Console.ReadLine().ToLower() ?? "a";

                Console.WriteLine("Type of instances (a/b): \n (a) Full - it will run through everything in the stream \n (b) Random - it will randomly select options in the stream");
                var selectedInstanceType = Console.ReadLine().ToLower() ?? "a";

                Console.WriteLine("Specify a browser for instances? (y/n)");
                var isBrowserSelected = Console.ReadLine().ToLower() ?? "y";

                var pickedBrowser = string.Empty;
                if (isBrowserSelected == "y")
                {
                    Console.WriteLine("Pick browser (a/b): \n(a) Google Chrome \n(b) Microsoft Edge");
                    pickedBrowser = Console.ReadLine().ToLower() ?? "a";
                }

                // Is for testing all projects in automation
                if (isSpecificTest != "y")
                {
                    if (selectedEnvironment == "a") selectedEnvironment = "Staging";
                    else selectedEnvironment = "Production";

                    var unitInfo = mappings.TestLinks.Where(x => x.Environment == selectedEnvironment).FirstOrDefault();

                    var threads = new List<Thread>();

                    Console.WriteLine($"Using the following links:");
                    foreach (var unit in unitInfo.Units)
                    {
                        Console.WriteLine($" {unit.UnitLink}");
                        threads.Add(new Thread(_ => StartInstance(unit.UnitLink, isBrowserSelected, pickedBrowser, selectedInstanceType, mappings)));
                    }

                    // Multithreading for each instance
                    foreach (var threading in threads)
                    {
                        threading.Start();
                        Console.WriteLine("\n");
                        Thread.Sleep(delay);
                    }
                }
                // Is for specific testing a link
                else
                {
                    Console.WriteLine("Enter number of threads: ");
                    var instances = Convert.ToInt32(Console.ReadLine());
                    if (instances == 0) instances = 1;

                    Console.WriteLine("Have an invitation link already? (y/n)");
                    var isInvite = Console.ReadLine();

                    var link = "https://outreach.ophs.io/9bIh6ZVD";

                    if (isInvite?.ToLower() == "y")
                    {
                        Console.WriteLine("Enter invitation link: ");
                        var input = Console.ReadLine();
                        if (input != null) link = input;
                    }
                    else
                    {
                        Console.WriteLine($"Using testing link:, {link}\n");
                    }

                    var threads = new List<Thread>();
                    while (instances > 0)
                    {
                        threads.Add(new Thread(_ => StartInstance(link, isBrowserSelected, pickedBrowser, selectedInstanceType, mappings)));
                        instances--;
                    }

                    foreach (var threading in threads)
                    {
                        threading.Start();
                        Thread.Sleep(5000);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private static void StartInstance(string url, string isBrowserSelected, string pickedBrowser, string isRandomInstance, MappingsDto mappings)
        {
            try
            {
                var sessionLog = Generator.GetLogInfo();
                WebDriver driver = null;

                if (isBrowserSelected == "y")
                {
                    driver = pickedBrowser switch
                    {
                        "a" => BrowserDrivers.GetDriver(0),
                        _ => BrowserDrivers.GetDriver(1)
                    };
                }
                else
                {
                    driver = BrowserDrivers.GetDriver(new Random().Next(0, 3));
                }

                Automate.Script(new GeneralDto
                {
                    Driver = driver,
                    Mappings = mappings,
                    Url = url,
                    IsRandom = isRandomInstance == "b",
                    Path = sessionLog.FilePath,
                    LogInfo = sessionLog
                });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}