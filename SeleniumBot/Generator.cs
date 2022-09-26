using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using OutreachAutomation.SeleniumBot.DTO.Mappings;
using OutreachAutomation.SeleniumBot.DTO.SessionLogs;

namespace OutreachAutomation.SeleniumBot
{
    public static class Generator
    {
        // Generate log info
        public static LogDto GenerateLogs()
        {
            var logs = new StringBuilder();
            var logId = Guid.NewGuid().ToString().ToUpper();
            var filePath = Path.Combine(Environment.CurrentDirectory, Directory.CreateDirectory("TempLogs").ToString(), $"LOG-{logId}.txt");

            return new LogDto()
            {
                FilePath = filePath,
                LogId = logId,
                Logs = logs
            };
        }

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
            var map = new MappingsDto()
            {
                Amenities = GetAmenityMappings(),
                Logins = GetLoginMappings()
            };

            return map;
        }

        #region Individual Mappings
        // Get amenity info from JSON
        private static List<AmenityDto> GetAmenityMappings()
        {
            var filePath = Path.Combine(Environment.CurrentDirectory, "Assets/JSONs/AmenityMaps.json");
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
        #endregion
    }
}