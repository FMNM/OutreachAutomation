using System;
using System.Linq;

namespace OutreachAutomation.Automation
{
    public class Generator
    {
        public static string NumberGenerator(int count)
        {
            const string digits = "0123456789";

            var random = new Random();
            return new string(Enumerable.Repeat(digits, count).Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}