using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TryBookingStorage.DTO {
    public class BookingDto {
        public string bookingUrlId { get; set; }
        [JsonConverter(typeof(TryBookingDateTimeConverter))] public DateTime date { get; set; }
        public string bookingFirstName { get; set; }
        public string bookingLastName { get; set; }
        public string bookingEmail { get; set; }
        public string bookingPhone { get; set; }
        public bool permissionToContact { get; set; }
        public string bookingAddress1 { get; set; }
        public string bookingAddress2 { get; set; }
        public string bookingCity { get; set; }
        public string bookingState { get; set; }
        public string bookingPostCode { get; set; }
        public string bookingCountry { get; set; }
        public double totalAmount { get; set; }
        public double totalCardFee { get; set; }
        public double totalProcessingFee { get; set; }
        public double totalBoxOfficeFee { get; set; }
        public double totalRefundedAmount { get; set; }
        public string urlReferrer { get; set; }
        public string customId { get; set; }
        [JsonConverter(typeof(ListConverter<BookingDataCollectionDto>))] public List<BookingDataCollectionDto> bookingDataCollections { get; set; }
        [JsonConverter(typeof(ListConverter<BookingTicketDto>))] public List<BookingTicketDto> bookingTickets { get; set; }
        [JsonConverter(typeof(ListConverter<BookingDonationDto>))] public List<BookingDonationDto> bookingDonations { get; set; }
        [JsonConverter(typeof(ListConverter<BookingGiftCertificateDto>))] public List<BookingGiftCertificateDto> bookingGiftCertificates { get; set; }
        [JsonConverter(typeof(ListConverter<BookingBoxOfficeFeeDto>))] public List<BookingBoxOfficeFeeDto> bookingBoxOfficeFees { get; set; }
    }
}