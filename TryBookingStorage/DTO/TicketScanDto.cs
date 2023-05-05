using System;
using System.Text.Json.Serialization;

namespace TryBookingStorage.DTO {
    public class TicketScanDto {
        public string ticketBarcode { get; set; }
        public string ticketScanId { get; set; }
        [JsonConverter(typeof(TryBookingDateTimeConverter))] public DateTime scanDateTime { get; set; }
        public string scanEvent { get; set; }
        public string scanDeviceName { get; set; }
        public string scanDeviceLocation { get; set; }
        public Int32 ruleId { get; set; }
    }
}