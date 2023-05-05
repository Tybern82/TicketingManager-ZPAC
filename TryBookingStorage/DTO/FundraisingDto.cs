using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TryBookingStorage.DTO {
    public class FundraisingDto {
        public string fundraisingPageName { get; set; }
        public Int32 fundraisingPageId { get; set; }
        public string description { get; set; }
        public string contactEmail { get; set; }
        public bool isActive { get; set; }
        [JsonConverter(typeof(ListConverter<Int32>))] public List<Int32> presetDonationValues { get; set; }
        [JsonConverter(typeof(ListConverter<FundraisingImageDto>))] public List<FundraisingImageDto> listOfImages { get; set; }
    }
}