using System.Text;

namespace OutreachAutomation.SeleniumBot.DTO.SessionLogs
{
    public class LogDto
    {
        public StringBuilder Logs { get; set; }
        public string LogId { get; set; }
        public string FilePath { get; set; }
    }
}
