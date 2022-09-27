using System.Collections.Generic;

namespace OutreachAutomation.SeleniumBot.DTO.Mappings
{
    public class ApartmentDto
    {
        public string LevelId { get; set; }
        public string LevelName { get; set; }
        public List<RoomDto> Rooms { get; set; }
    }
}