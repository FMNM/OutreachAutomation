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
    public static class Program
    {
        public static void Main()
        {
            try
            {
                Console.WriteLine("Enter number of instances: ");
                var instances = Convert.ToInt32(Console.ReadLine());

                var threads = new List<Thread>();
                while (instances > 0)
                {
                    threads.Add(new Thread(Func));
                    instances--;
                }

                foreach (var threading in threads)
                {
                    threading.Start();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private static void Func()
        {
            Automate.Script();
        }
    }
}