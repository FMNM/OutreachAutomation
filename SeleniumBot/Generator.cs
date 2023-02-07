using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using OutreachAutomation.SeleniumBot.DTO.Mappings;
using OutreachAutomation.SeleniumBot.DTO.SessionLogs;

namespace OutreachAutomation.SeleniumBot
{
    public static class Generator
    {
        // Random number generator
        public static string GetPhoneNumber()
        {
            const string digits = "0123456789";

            var random = new Random();
            return new string(Enumerable.Repeat(digits, 10).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        // Mappings
        public static MappingsDto GetMappings()
        {
            var map = new MappingsDto
            {
                Amenities = GetAmenityMappings(),
                Logins = GetLoginMappings(),
                Apartments = GetApartmentMappings(),
                TestLinks = GetTestLinksMappings()
            };

            return map;
        }

        // Generate log info
        public static LogDto GetLogInfo()
        {
            var logId = Guid.NewGuid().ToString().ToUpper();
            var filePath = Path.Combine(Environment.CurrentDirectory, Directory.CreateDirectory("TempLogs").ToString(),
                $"[{DateTime.Now.Day}.{DateTime.Now.Month}.{DateTime.Now.Year} {DateTime.Now.ToShortTimeString().Replace(":", ".")}]-LOG-{logId}.txt");

            return new LogDto
            {
                FilePath = filePath,
                LogId = logId
            };
        }

        // Add to log file
        public static void AddLog(string data, string path)
        {
            using var writer = File.AppendText(path);
            writer.WriteLine(data);
        }

        // Create new log info files
        public static TimeSpan GetTotalAutomationDuration(string filePath)
        {
            try
            {
                var timeStamps = File.ReadLines(filePath).Where(x => x.Contains("[") && x.Contains("]")).ToList();

                var startTime = DateTime.Parse(timeStamps[0].Substring(timeStamps[0].IndexOf("[") + 1, timeStamps[0].IndexOf("]") - 1));
                var endTime = DateTime.Parse(timeStamps[^1].Substring(timeStamps[^1].IndexOf("[") + 1, timeStamps[^1].IndexOf("]") - 1));

                return endTime - startTime;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #region Individual Mappings

        // Get amenity info from JSON
        private static List<AmenityDto> GetAmenityMappings()
        {
            var filePath = Path.Combine(Environment.CurrentDirectory, "Assets/JSONs/AmenityData.json");
            var file = File.ReadAllText(filePath);

            var map = JsonConvert.DeserializeObject<List<AmenityDto>>(file);

            return map;
        }

        // Get login info from JSON
        private static List<UserDto> GetLoginMappings()
        {
            var filePath = Path.Combine(Environment.CurrentDirectory, "Assets/JSONs/UserData.json");
            var file = File.ReadAllText(filePath);

            var map = JsonConvert.DeserializeObject<List<UserDto>>(file);

            return map;
        }

        // Get apartment info from JSON
        private static List<ApartmentDto> GetApartmentMappings()
        {
            var filePath = Path.Combine(Environment.CurrentDirectory, "Assets/JSONs/ApartmentData.json");
            var file = File.ReadAllText(filePath);

            var map = JsonConvert.DeserializeObject<List<ApartmentDto>>(file);

            return map;
        }

        // Get link info from JSON
        private static List<TestLinksDto> GetTestLinksMappings()
        {
            var filePath = Path.Combine(Environment.CurrentDirectory, "Assets/JSONs/TestLinks.json");
            var file = File.ReadAllText(filePath);

            var map = JsonConvert.DeserializeObject<List<TestLinksDto>>(file);

            return map;
        }

        #endregion
    }
}