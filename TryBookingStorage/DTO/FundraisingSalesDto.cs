using System;
using System.Text.Json.Serialization;

namespace TryBookingStorage.DTO {
    public class FundraisingSalesDto {
        public string fundraisingPageName { get; set; }
        public Int32 totalTransactions { get; set; }
        public double totalSales { get; set; }
        [JsonConverter(typeof(TryBookingDateTimeConverter))] public DateTime transactionDate { get; set; }
        public string trend { get; set; }
    }
}