using OpenQA.Selenium;
using OutreachAutomation.SeleniumBot.DTO.Mappings;
using OutreachAutomation.SeleniumBot.DTO.SessionLogs;

namespace OutreachAutomation.SeleniumBot.DTO
{
    public class GeneralDto
    {
        public WebDriver Driver { get; set; } 
        public string Url { get; set; } 
        public bool IsRandom { get; set; } 
        public MappingsDto Mappings { get; set; }
        public LogDto LogInfo { get; set; }
        public string Path { get; set; }
    }
}