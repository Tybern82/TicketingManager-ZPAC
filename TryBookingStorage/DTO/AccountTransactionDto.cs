using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TryBookingStorage.DTO {
    public class AccountTransactionDto {
        public Int32 accountId { get; set; }
        public double pendingFundTransfer { get; set; }
        public double balance { get; set; }
        [JsonConverter(typeof(ListConverter<TransactionDto>))] public List<TransactionDto> transactions { get; set; }
    }
}
