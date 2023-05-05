using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TryBookingStorage {

    public class PromotionCode {
        public static PromotionCode NONE = new PromotionCode("", "");

        public string promoCode { get; set; }
        public string promoName { get; set; }
        public bool hasFreeDrink { get; set; }

        public PromotionCode(string code, string name, bool freeDrink) {
            this.promoCode = code;
            this.promoName = (name == null) ? code : name;
            this.hasFreeDrink = freeDrink;
        }

        public PromotionCode(string code, string name) : this(code, name, false) {
            // TODO: Update to request free-drink status of unknown promo codes
            switch (code) {
                case "door":
                case "Comp1":
                case "Prepaid":
                case "":
                    hasFreeDrink = false;
                    break;

                default:
                    hasFreeDrink = true;
                    break;
            }
        }

        public static void preloadPromotionCodes() {
            // TODO: Store the preloaded promotion codes
            PromotionCode code;
            code = new PromotionCode("door", "Pay at Door");
            code = new PromotionCode("Comp1", "Complimentary Ticket");
            code = new PromotionCode("zpac100", "ZPac Member Ticket");
            code = new PromotionCode("GIFT201", "Gift Certificate");
            code = new PromotionCode("BNIGIFT2015", "Gift Certificate BNI Members");
            code = new PromotionCode("SK2015", "Storage King Comp Tickets");
            code = new PromotionCode("CHS2015", "Campbell Hearing Solutions");
            code = new PromotionCode("Prepaid", "Prepaid");
            code = new PromotionCode("SCAGIFT", "Southern Cross Gift");
            code = new PromotionCode("GPSGIFT", "Guaranteed Plumbing Solutions");
        }
    }
}
