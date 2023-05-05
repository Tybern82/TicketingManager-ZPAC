using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace TryBookingStorage.DTO {
    public class BookingBoxOfficeFeeDto {
        public double boxOfficeFee { get; set; }
        public double appliedGCValue { get; set; }
        public double cardFee { get; set; }
        public double processingFee { get; set; }
        public double tax { get; set; }
        public double refundedAmount { get; set; }
        public double refundedCardFee { get; set; }
        public double refundedProcessingFee { get; set; }
        public double refundedTax { get; set; }
        public string eventName { get; set; }
    }
}