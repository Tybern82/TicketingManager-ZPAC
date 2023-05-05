using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TryBookingStorage;
using TryBookingStorage.DTO;

namespace ReportPrinter {
    public class DoorListReportPrinter {

        public static PrintDialog printDialog = new PrintDialog();

        public static bool MarkThroughSeats = false;


        private static string FIRSTNAME = "Attendee First Name";
        private static string LASTNAME = "Attendee Last Name";
        private static string ADDRESS = "Address";
        private static string TELEPHONE = "Telephone No.";
        private static string EMAIL = "Email";

        public static void printReport(Form wndMain, EventSessionDto eventSession, IList<BookingDto> bookings) {
            int ticketCount = 0;
            List<DoorListEntry> doorList = new List<DoorListEntry>();
            foreach(BookingDto currBooking in bookings) {
                DoorListEntry bookingEntry = new DoorListEntry();
                bookingEntry.sessionTime = eventSession.eventStartDate;
                bookingEntry.firstName = currBooking.bookingFirstName;
                bookingEntry.lastName = currBooking.bookingLastName;
                bookingEntry.contactNumber = currBooking.bookingPhone;
                foreach (BookingTicketDto ticket in currBooking.bookingTickets) {
                    if ((ticket.sessionId == eventSession.id) && (!ticket.isVoid)) {
                        DoorListEntry ticketEntry = bookingEntry.copy();
                        // valid ticket for this event
                        ticketEntry.eventName = ticket.eventName;
                        ticketEntry.ticketPrice = new CurrencyAUD(ticket.totalTicketPrice);
                        ticketEntry.ticketType = TicketTypeHelper.getTicketType(ticket.ticketName);
                        foreach (TicketSeatDto seat in ticket.ticketSeats) {
                            DoorListEntry currSeat = ticketEntry.copy();
                            currSeat.seatRow = seat.rowName;
                            currSeat.seatNumber = seat.seatNumber;
                            currSeat.ticketNumber = seat.ticketBarcode;

                            IDictionary<string, string> data = seat.getDataCollection();
                            if (data.ContainsKey(FIRSTNAME)) {
                                currSeat.firstName = data[FIRSTNAME];
                                currSeat.lastName = data[LASTNAME];
                                currSeat.contactNumber = data[TELEPHONE];
                            }

                            doorList.Add(currSeat);
                            ticketCount++;
                        }
                    }
                }                
            }
            DoorListPrinter printer = new DoorListPrinter();
            printDialog.Document = printer;
            if (((wndMain != null) ? printDialog.ShowDialog(wndMain) : printDialog.ShowDialog()) == DialogResult.OK) {

                printer.DocumentName = "doorList-bySurname";
                printer.listTitle = "Door List by Surname";
                printer.doorList = Helper.sortByName(doorList);
                printer.Print();

                printer.DocumentName = "doorList-bySeat";
                printer.listTitle = "Door List by Seat";
                DoorListEntrySizes sz = printer.doorListSizes;  // cache the already calculated sizes, since these won't change simply by reordering
                printer.doorList = Helper.sortBySeat(doorList);
                printer.doorListSizes = sz; // restore already calculated sizes
                printer.Print();

                SeatingMapPrinter seatingMapPrinter = new SeatingMapPrinter();
                seatingMapPrinter.PrinterSettings = printDialog.PrinterSettings;
                printDialog.Document = seatingMapPrinter;
                seatingMapPrinter.doorList = printer.doorList;
                seatingMapPrinter.Print();
            }

            MessageBox.Show(ticketCount + " tickets located");
        }
    }
}
