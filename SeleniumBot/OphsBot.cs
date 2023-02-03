using System;
using System.Collections.Generic;
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
                var mappings = Generator.GetMappings() ?? throw new Exception("Failed to load");

                Console.WriteLine("Type of instances (a/b): \n(a) Full \n(b) Random");
                var isRandomInstance = Console.ReadLine() ?? "a";                

                Console.WriteLine("Specify browser? (y/n)");
                var isBrowserSelected = Console.ReadLine() == "y";

                var pickedBrowser = string.Empty;
                if (isBrowserSelected)
                {
                    Console.WriteLine("Pick browser (a/b): \n(a) Google Chrome \n(b) Microsoft Edge");
                    pickedBrowser = Console.ReadLine() ?? "a";
                }

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
                    threads.Add(new Thread(_ => StartInstance(link, isBrowserSelected, pickedBrowser, isRandomInstance, mappings)));
                    instances--;
                }

                foreach (var threading in threads)
                {
                    threading.Start();
                    Thread.Sleep(5000);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private static void StartInstance(string url, bool isBrowserSelected, string pickedBrowser, string isRandomInstance, MappingsDto mappings)
        {
            try
            {
                var sessionLog = Generator.GetLogInfo();
                WebDriver driver = null;

                if (isBrowserSelected)
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