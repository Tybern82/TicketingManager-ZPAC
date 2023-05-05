using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TryBookingStorage.DTO {
    public class BookingTicketDto {
        public string ticketName { get; set; }
        public Int32 seatQuantity { get; set; }
        public double discountAmount { get; set; }
        public double appliedGCOnUnitPrice { get; set; }

        public Int32 sessionId { get; set; }
        [JsonConverter(typeof(TryBookingDateTimeConverter))] public DateTime eventEndDate { get; set; }
        [JsonConverter(typeof(TryBookingDateTimeConverter))] public DateTime eventStartDate { get; set; }
        public string alternateLabel { get; set; }
        public string eventName { get; set; }
        public string sectionName { get; set; }
        public string eventCode { get; set; }
        public double totalTicketPrice { get; set; }
        public double refundedAmount { get; set; }
        public double cardFee { get; set; }
        public double processingFee { get; set; }
        public double ticketFee { get; set; }
        public double refundedCardFee { get; set; }
        public double refundedProcessingFee { get; set; }
        public double tax { get; set; }
        public double refundedTax { get; set; }
        public bool isVoid { get; set; }
        [JsonConverter(typeof(ListConverter<BookingTicketDataCollectionDto>))] public List<BookingTicketDataCollectionDto> bookingTicketDataCollections { get; set; }
        [JsonConverter(typeof(ListConverter<TicketSeatDto>))] public List<TicketSeatDto> ticketSeats { get; set; }
    }
}
