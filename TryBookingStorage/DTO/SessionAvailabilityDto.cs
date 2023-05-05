using System;
using System.Text.Json.Serialization;

namespace TryBookingStorage.DTO {
    public class SessionAvailabilityDto {
        [JsonConverter(typeof(TryBookingDateTimeConverter))] public DateTime eventStartDate { get; set; }
        [JsonConverter(typeof(TryBookingDateTimeConverter))] public DateTime eventEndDate { get; set; }
        [JsonConverter(typeof(TryBookingDateTimeConverter))] public DateTime bookingStartDate { get; set; }
        [JsonConverter(typeof(TryBookingDateTimeConverter))] public DateTime bookingEndDate { get; set; }
        public string description { get; set; }
        public Int32 id { get; set; }
        public Int32 eventId { get; set; }
        public string sessionStatus { get; set; }
        public Int32 sessionCapacity { get; set; }
        public Int32 sessionAvailability { get; set; }
        public string sessionBookingUrl { get; set; }
    }
}