using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TryBookingStorage.DTO {
    public class EventSessionDto {
        [JsonConverter(typeof(TryBookingDateTimeConverter))] public DateTime eventStartDate { get; set; }
        [JsonConverter(typeof(TryBookingDateTimeConverter))] public DateTime eventEndDate { get; set; }
        [JsonConverter(typeof(TryBookingDateTimeConverter))] public DateTime bookingStartDate { get; set; }
        [JsonConverter(typeof(TryBookingDateTimeConverter))] public DateTime bookingEndDate { get; set; }
        public string alternateLabel { get; set; }
        public string description { get; set; }
        public Int32 id { get; set; }
        public string sessionStatus { get; set; }
        public Int32 sessionCapacity { get; set; }
        public Int32 sessionAvailability { get; set; }
        public string sessionBookingUrl { get; set; }

        public override string ToString() {
            string eventTimeString = eventStartDate.ToLocalTime().ToString("f");
            return (!String.IsNullOrWhiteSpace(description)) ? description + " - " + eventTimeString : eventTimeString;
        }
    }
}
