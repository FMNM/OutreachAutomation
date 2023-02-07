﻿using System.Collections.Generic;

namespace OutreachAutomation.SeleniumBot.DTO.Mappings
{
    public class MappingsDto
    {
        public List<AmenityDto> Amenities { get; set; }
        public List<UserDto> Logins { get; set; }
        public List<ApartmentDto> Apartments { get; set; }
        public List<TestLinksDto> TestLinks { get; set; }
    }
}