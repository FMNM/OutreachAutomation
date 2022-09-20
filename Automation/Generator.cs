using System;
using System.Collections.Generic;
using System.Linq;

namespace OutreachAutomation.Automation
{
    public static class Generator
    {
        public static string NumberGenerator(int count)
        {
            const string digits = "0123456789";

            var random = new Random();
            return new string(Enumerable.Repeat(digits, count).Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}