using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TryBookingStorage.DTO {
    public class TicketSeatDto {
        public string ticketBarcode { get; set; }
        public string rowName { get; set; }
        public Int32 seatNumber { get; set; }
        public string doorName { get; set; }
        [JsonConverter(typeof(ListConverter<BookingTicketDataCollectionDto>))] public List<BookingTicketDataCollectionDto> bookingTicketDataCollection { get; set; }

        public IDictionary<string, string> getDataCollection() {
            IDictionary<string, string> result = new Dictionary<string, string>(bookingTicketDataCollection.Count);
            foreach (BookingTicketDataCollectionDto item in bookingTicketDataCollection) {
                result.Add(item.label, item.value);
            }
            return result;
        }
    }
}
