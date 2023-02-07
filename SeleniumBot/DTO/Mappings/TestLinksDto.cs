using System.Collections.Generic;

namespace OutreachAutomation.SeleniumBot.DTO.Mappings
{
    public class TestLinksDto
    {
        public string Environment { get; set; }
        public List<Unit> Units { get; set; }
    }

    public class Unit
    {
        public string UnitName { get; set; }
        public string UnitLink { get; set; }
    }
}
