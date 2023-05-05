using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TryBookingStorage.DTO {
    public class TransactionDto {
        public string transactionTypeName { get; set; }
        public string description { get; set; }
        public double debitAmount { get; set; }
        public double creditAmount { get; set; }
        [JsonConverter(typeof(TryBookingDateTimeConverter))] public DateTime transactionDate { get; set; }
        public string customerName { get; set; }
        public string bookingURLId { get; set; }
    }
}
