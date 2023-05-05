using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TryBookingStorage.DTO {
    public class BookingGiftCertificateDto {
        public double value { get; set; }
        public string name { get; set; }
        public double cardFee { get; set; }
        public double processingFee { get; set; }
        public double ticketFee { get; set; }
        public double discountAmount { get; set; }
        public double refundedAmount { get; set; }
        public double refundedCardFee { get; set; }
        public double refundedProcessingFee { get; set; }
    }
}
