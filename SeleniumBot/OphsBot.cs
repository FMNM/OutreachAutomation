using System;
using System.Collections.Generic;
using System.Threading;
using OutreachAutomation.SeleniumBot.DTO.Mappings;

namespace OutreachAutomation.SeleniumBot
{
    public static class OphsBot
    {
        public static void Begin()
        {
            try
            {
                Console.WriteLine("Type of instances (a/b): \n(a) Full \n(b) Random");
                var isRandomInstance = Console.ReadLine() ?? "a";

                Console.WriteLine("Specify browser? (y/n)");
                var isBrowserSelected = Console.ReadLine() == "y";

                var pickedBrowser = string.Empty;
                if (isBrowserSelected)
                {
                    Console.WriteLine("Pick browser (a/b/c): \n(a) Google Chrome \n(b) Microsoft Edge \n(c) Mozilla Firefox");
                    pickedBrowser = Console.ReadLine() ?? "a";
                }

                Console.WriteLine("Enter number of threads: ");
                var instances = Convert.ToInt32(Console.ReadLine());
                if (instances == 0) instances = 1;

                Console.WriteLine("Have an invitation link already? (y/n)");
                var isInvite = Console.ReadLine();

                var threads = new List<Thread>();
                var link = "https://outreach.ophs.io/9bIh6ZVD";

                if (isInvite?.ToLower() == "y")
                {
                    Console.WriteLine("Enter invitation link: ");
                    var input = Console.ReadLine();
                    if (input != null) link = input;
                }
                
                while (instances > 0)
                {
                    threads.Add(new Thread(_ => StartInstance(link, isBrowserSelected, pickedBrowser, isRandomInstance)));
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

        private static void StartInstance(string url, bool isSpecified, string browserDriver, string isRandom)
        {
            var mappings = Generator.GetMappings() ?? throw new Exception("Failed to load");
            var isRandomInstance = isRandom == "b";

            if (isSpecified)
            {
                var driver = browserDriver switch
                {
                    "a" => BrowserDrivers.GetDriver(0),
                    "b" => BrowserDrivers.GetDriver(1),
                    _ => BrowserDrivers.GetDriver(2)
                };

                Automate.Script(url, driver, isRandomInstance, mappings);
            }
            else
            {
                var driver = BrowserDrivers.GetDriver(new Random().Next(0, 3));

                Automate.Script(url, driver, isRandomInstance, mappings);
            }
        }
    }
}