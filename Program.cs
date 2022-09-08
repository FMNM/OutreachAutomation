using System;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;

namespace OutreachAutomation
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Automate.TestScript();
            
            Automate.Script();
        }
    }
}