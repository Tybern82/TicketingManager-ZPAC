using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TryBookingStorage.DTO {
    public class TicketScanListDto {
        public Int32 sessionId { get; set; }
        [JsonConverter(typeof(ListConverter<TicketScanDto>))] public List<TicketScanDto> listOfTicketScans { get; set; }
    }
}