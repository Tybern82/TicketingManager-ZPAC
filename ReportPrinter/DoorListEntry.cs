using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TryBookingStorage;

namespace ReportPrinter {

    public class DoorListEntrySizes {
        public int firstNameWidth { get; set; } = 0;
        public int lastNameWidth { get; set; } = 0;
        public int contactNumberWidth { get; set; } = 0;
        public int ticketTypeWidth { get; set; } = 0;
        public int ticketPriceWidth { get; set; } = 0;
        public int promoCodeWidth { get; set; } = 0;
        public int seatWidth { get; set; } = 0;
        public int freeDrinkWidth { get; set; } = 0;

        public int tableWidth {
            get { return firstNameWidth + lastNameWidth + contactNumberWidth + ticketTypeWidth + ticketPriceWidth + promoCodeWidth + seatWidth + freeDrinkWidth; }
        }
    }

    [Serializable]
    public class DoorListEntry {
        // "Event Name","Session Date" (GMT+10:00),"Session Time",
        // "Booking First Name","Booking Last Name","Booking Telephone",
        // "Ticket Type","Ticket Price" (AUD),
        // Promotion[Discount] Code,
        // Ticket Number
        // "Seat Row","Seat Number",

        public string eventName { get; set; }
        public DateTime sessionTime { get; set; }

        private string _firstName;
        public string firstName {
            get { return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(CultureInfo.CurrentCulture.TextInfo.ToLower(_firstName)); }
            set { _firstName = value; }
        }

        private string _lastName;
        public string lastName {
            get {
                string _result = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(CultureInfo.CurrentCulture.TextInfo.ToLower(_lastName));
                char[] _nStr = _result.ToCharArray();
                // APPLY Special Cases
                if (_result.StartsWith("O'")) _nStr[2] = CultureInfo.CurrentCulture.TextInfo.ToUpper(_result[2]);
                else if (_result.StartsWith("Mc")) _nStr[2] = CultureInfo.CurrentCulture.TextInfo.ToUpper(_result[2]);
                return new string(_nStr);
            }
            set { _lastName = value; }
        }

        private string _contactNumber;
        public string contactNumber {
            get { return Helper.formatPhone(_contactNumber); }
            set { _contactNumber = value; }
        }

        public TicketType ticketType { get; set; }
        public CurrencyAUD ticketPrice { get; set; }

        public PromotionCode promoCode { get; set; } = PromotionCode.NONE;

        public string ticketNumber { get; set; }

        public Seat seat { get { return SeatHelper.getSeat(seatRow, seatNumber); } }
        public string seatRow { get; set; }
        public int seatNumber { get; set; }

        public override String ToString() {
            return firstName + " " + lastName + "\n" +
                    contactNumber + "\n" +
                    eventName + " @ " + sessionTime + " [" + seat + "]" + "\n" +
                    ticketType + " [" + promoCode.promoName + "] = " + ticketPrice;
        }

        public DoorListEntry copy() {
            DoorListEntry result = new DoorListEntry();
            result.eventName = eventName;
            result.sessionTime = sessionTime;
            result.firstName = firstName;
            result.lastName = lastName;
            result.contactNumber = contactNumber;
            result.ticketType = ticketType;
            result.ticketPrice = ticketPrice;
            result.promoCode = promoCode;
            result.ticketNumber = ticketNumber;
            result.seatRow = seatRow;
            result.seatNumber = seatNumber;
            return result;
        }
    }

    public class Helper {
        public static void printDoorLists(List<DoorListEntry> list) {
            DoorListPrinter printer = new DoorListPrinter();
            printer.listTitle = "Door List by Surname";
            printer.doorList = sortByName(list);
            printer.Print();

            printer.listTitle = "Door List by Seat";
            DoorListEntrySizes sz = printer.doorListSizes;  // cache the already calculated sizes, since these won't change simply by reordering
            printer.doorList = sortBySeat(list);
            printer.doorListSizes = sz; // restore already calculated sizes
            printer.Print();
        }

        public static List<DoorListEntry> sortByName(IEnumerable<DoorListEntry> lst) {
            SortedList<string, List<DoorListEntry>> sList = new SortedList<string, List<DoorListEntry>>(lst.Count());
            foreach (DoorListEntry d in lst) {
                string key = d.lastName + ", " + d.firstName;
                if (sList.ContainsKey(key)) {
                    List<DoorListEntry> v = sList[key];
                    v.Add(d);
                } else {
                    sList.Add(key, new List<DoorListEntry>(new DoorListEntry[] { d }));
                }
            }
            List<DoorListEntry> _result = new List<DoorListEntry>(lst.Count());
            foreach (List<DoorListEntry> i in sList.Values) {
                if (i.Count() == 1) {
                    _result.Add(i[0]);
                } else if (i.Count() > 1) {
                    if (i[0].seat == Seat.XTD) {
                        foreach (DoorListEntry d in i) _result.Add(d);
                    } else {
                        foreach (DoorListEntry d in sortBySeat(i)) _result.Add(d);
                    }
                }
            }
            return _result;
        }

        public static List<DoorListEntry> sortBySeat(IEnumerable<DoorListEntry> lst) {
            SortedList<Seat, DoorListEntry> sList = new SortedList<Seat, DoorListEntry>(lst.Count());
            List<DoorListEntry> xList = new List<DoorListEntry>();
            foreach (DoorListEntry d in lst) {
                if (d.seat == Seat.XTD) xList.Add(d);
                else sList.Add(d.seat, d);
            }
            List<DoorListEntry> _result = new List<DoorListEntry>(sList.Count + xList.Count);
            _result.AddRange(sList.Values);
            _result.AddRange(sortByName(xList));
            return _result;
        }

        public static string formatPhone(string phone) {
            if (String.IsNullOrWhiteSpace(phone)) return "";
            StringBuilder str = new StringBuilder(phone.Length);
            char[] digits = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '+' };
            foreach (char ch in phone) {
                if (digits.Contains(ch)) str.Append(ch);
            }
            phone = str.ToString();
            char sep = '-';
            string comboPatt = @"^[0-9]{18}?$";
            string mobilePatt = @"^[0][4][0-9]{8}?$";
            string localPatt = @"^[0-9]{8}?$";
            string stdPatt = @"^[0][0-9]{9}?$";
            string intlMPatt = @"^[+][6][1][4][0-9]{8}?$";
            string intlPatt = @"^[+][6][1][0-9]{9}?$";
            string errPatt = @"^[0-9]{9}?$";
            if (Regex.Match(phone, comboPatt).Value != String.Empty) {
                return phone.Substring(0, 4) + sep + phone.Substring(4, 4) + " / " + phone.Substring(8, 4) + sep + phone.Substring(8 + 4, 3) + sep + phone.Substring(8 + 7, 3);
            }
            if (Regex.Match(phone, mobilePatt).Value != String.Empty) {
                return phone.Substring(0, 4) + sep + phone.Substring(4, 3) + sep + phone.Substring(7, 3);
            } else if (Regex.Match(phone, localPatt).Value != String.Empty) {
                return phone.Substring(0, 4) + sep + phone.Substring(4, 4);
            } else if (Regex.Match(phone, stdPatt).Value != String.Empty) {
                return phone.Substring(0, 2) + " " + phone.Substring(2, 4) + sep + phone.Substring(6, 4);
            } else if (Regex.Match(phone, intlMPatt).Value != String.Empty) {
                return "0" + phone.Substring(3, 3) + sep + phone.Substring(6, 3) + sep + phone.Substring(9, 3);
            } else if (Regex.Match(phone, intlPatt).Value != String.Empty) {
                return "0" + phone.Substring(3, 1) + " " + phone.Substring(4, 4) + sep + phone.Substring(8, 4);
            } else if (Regex.Match(phone, errPatt).Value != String.Empty) {
                if (phone[0] == '4') {
                    return "0" + phone.Substring(0, 3) + sep + phone.Substring(3, 3) + sep + phone.Substring(6, 3);
                } else {
                    return "0" + phone.Substring(0, 1) + " " + phone.Substring(1, 4) + sep + phone.Substring(5, 4);
                }
            }
            return phone;
        }

        /*public static List<DoorListEntry> loadCSV(string fname) {
            List<DoorListEntry> _result = new List<DoorListEntry>();
            using (CsvReader reader = new CsvReader(new StreamReader(fname), false)) {

                // foreach (string s in reader.GetFieldHeaders()) Console.WriteLine("[" + s + "]");

                int eventNameI = 0;     // reader.GetFieldIndex("Event Name");
                int sessionDateI = 1;   // reader.GetFieldIndex("Session Date");
                int sessionTimeI = 2;   // reader.GetFieldIndex("Session Time");

                int firstNameI = 3;     // reader.GetFieldIndex("Booking First Name");
                int lastNameI = 4;      // reader.GetFieldIndex("Booking Last Name");
                int contactNumberI = 5; // reader.GetFieldIndex("Booking Telephone");

                int ticketTypeI = 6;    // reader.GetFieldIndex("Ticket Type");
                int ticketPriceI = 7;   // reader.GetFieldIndex("Ticket Price");

                int promoCodeI = 8;     // reader.GetFieldIndex("Promotion[Discount] Code");

                int ticketNumberI = 9;

                int seatRowI = 10;       // reader.GetFieldIndex("Seat Row");
                int seatNumberI = 11;   // reader.GetFieldIndex("Seat Number");

                while (reader.ReadNextRecord()) {
                    DoorListEntry entry = new DoorListEntry();

                    entry.eventName = reader[eventNameI];
                    DateTime d = DateTime.Parse(reader[sessionDateI]);
                    DateTime t = DateTime.Parse(reader[sessionTimeI]);
                    entry.sessionTime = new DateTime(d.Year, d.Month, d.Day, t.Hour, t.Minute, t.Second, DateTimeKind.Local);

                    entry.firstName = reader[firstNameI];
                    entry.lastName = reader[lastNameI];
                    entry.contactNumber = reader[contactNumberI];

                    entry.ticketType = TicketTypeHelper.getTicketType(reader[ticketTypeI]);
                    entry.ticketPrice = (CurrencyAUD)("$" + reader[ticketPriceI]);

                    entry.promoCode = new PromotionCode(reader[promoCodeI], null);    // TODO: Lookup promotion codes

                    entry.ticketNumber = reader[ticketNumberI];

                    entry.seatRow = reader[seatRowI];
                    if (entry.seatRow == "I-") entry.seatRow = "I";     // TODO: Fix the TryBooking name....
                    entry.seatNumber = reader[seatNumberI];

                    _result.Add(entry);
                }
            }
            return _result;
        }*/
    }
}
