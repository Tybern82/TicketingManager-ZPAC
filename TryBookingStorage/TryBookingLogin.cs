using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using RestSharp.Authenticators;
using System.Text.Json;
using TryBookingStorage.DTO;

namespace TryBookingStorage {

    public class TryBookingLogin {

        private static TryBookingRegionCode DefaultRegion = TryBookingRegionCode.AU;
        private static string DefaultUsername = "38EEA69009B21382ACB25CD982BC5C9C";
        private static string DefaultPassword = "2424EAAC1DDC6D2C560E2D853C44E6CD";

        public static TryBookingRegionCode CurrentRegion { get; set; } = DefaultRegion;
        public static string CurrentUsername { get; set; } = DefaultUsername;
        public static string CurrentPassword { get; set; } = DefaultPassword;


        public enum TryBookingRegionCode {
            AU, NZ, UK, US
            // AU = Australia, NZ = New Zealand, UK = United Kingdom, US = United States
        }

        public static string regionName(TryBookingRegionCode regionCode) {
            switch (regionCode) {
                case TryBookingRegionCode.AU: return "Australia";
                case TryBookingRegionCode.NZ: return "New Zealand";
                case TryBookingRegionCode.UK: return "United Kingdom";
                case TryBookingRegionCode.US:
                default:
                    return "United States";
            }
        }

        public static string regionCodeID(TryBookingRegionCode regionCode) {
            switch (regionCode) {
                case TryBookingRegionCode.AU: return "AU";
                case TryBookingRegionCode.NZ: return "NZ";
                case TryBookingRegionCode.UK: return "UK";
                case TryBookingRegionCode.US:
                default:
                    return "US";
            }
        }

        public enum TryBookingDatePeriod {
            DAY = 1,
            WEEK = 2,
            MONTH = 3,
            YEAR = 4
        }
    }
}
