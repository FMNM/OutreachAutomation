#nullable enable
using System;
using System.Collections.Generic;
using System.Threading;
using OutreachAutomation.Automation;

namespace OutreachAutomation
{
    public static class Program
    {
        public static void Main()
        {
            try
            {
                Console.WriteLine("Specify browser? (y/n)");
                var isBrowserSpecific = Console.ReadLine() ?? "n";

                var pickedBrowser = string.Empty;
                if (isBrowserSpecific?.ToLower() == "y")
                {
                    Console.WriteLine(
                        "Pick browser (a/b/c): \n(a) Google Chrome \n(b) Microsoft Edge \n(c) Mozilla Firefox");
                    pickedBrowser = Console.ReadLine() ?? "a";
                }

                Console.WriteLine("Enter number of threads: ");
                var instances = Convert.ToInt32(Console.ReadLine());

                Console.WriteLine("Have an invitation link already? (y/n)");
                var isInvite = Console.ReadLine();

                var threads = new List<Thread>();
                var link = "https://outreach.ophs.io/9bIh6ZVD";

                if (isInvite?.ToLower() == "y")
                {
                    Console.WriteLine("Enter invitation link: ");
                    var input = Console.ReadLine();
                    if (input != null)
                    {
                        link = input;
                    }
                }

                while (instances > 0)
                {
                    threads.Add(new Thread(_ => { StartInstance(link, isBrowserSpecific, pickedBrowser); }));
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

        private static void StartInstance(string url, string isSpecified, string browserDriver)
        {
            var random = new Random();

            if (isSpecified.ToLower() == "y")
            {
                var driver = browserDriver switch
                {
                    "a" => BrowserDrivers.GetDriver(0),
                    "b" => BrowserDrivers.GetDriver(1),
                    _ => BrowserDrivers.GetDriver(2)
                };
                Automate.Script(driver, url);
            }
            else
            {
                var driver = BrowserDrivers.GetDriver(random.Next(0, 3));
                Automate.Script(driver, url);
            }
        }
    }
}