using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TryBookingStorage.DTO {
    public class AccountEventListDto {

        public string name { get; set; }
        public string eventCode { get; set; }
        public string description { get; set; }
        public string venue { get; set; }
        public string venueLatitude { get; set; }
        public string venueLongitude { get; set; }
        public string onlineEventLink { get; set; }

        public string contactName { get; set; }
        public string contactEmail { get; set; }
        public string contactNumber { get; set; }
        public bool isPublic { get; set; } = true;
        public bool allowWaitingList { get; set; } = true;
        public string timeZone { get; set; }
        public string bookingUrl { get; set; }
        public string homepageTemplate { get; set; }
        public bool isOpen { get; set; } = true;

        [JsonConverter(typeof(ListConverter<EventImageDto>))] public List<EventImageDto> listOfImages { get; set; }

        [JsonConverter(typeof(ListConverter<EventSessionDto>))] public List<EventSessionDto> sessionList { get; set; }
    }
}
