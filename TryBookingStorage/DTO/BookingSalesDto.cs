using System;
using System.Text.Json.Serialization;

namespace TryBookingStorage.DTO {
    public class BookingSalesDto {
        [JsonConverter(typeof(TryBookingDateTimeConverter))] public DateTime transactionDate { get; set; }
        public Int32 totalBookings { get; set; }
        public Int32 totalSold { get; set; }
        public double totalSales { get; set; }
        public string trend { get; set; }
    }
}