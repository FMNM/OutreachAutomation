using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using OutreachAutomation.Automation.DTO;

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

        public static List<AmenityMap> GetAmenityMappings()
        {
            var filePath = Path.Combine(Environment.CurrentDirectory, "Config JSONs/AmenityMaps.json");
            var file = File.ReadAllText(filePath);

            var map = JsonConvert.DeserializeObject<List<AmenityMap>>(file);
            
            return map;
        }
    }
}