using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ReportPrinter;
using TryBookingStorage;
using TryBookingStorage.DTO;

namespace TicketingManager {
    public partial class dlgPrintReport : Form {

        private IList<AccountEventListDto> eventList = TryBookingAPI.EventGetByAccountId();

        public dlgPrintReport() {
            InitializeComponent();
            List<string> eventNames = new List<string>(eventList.Count);
            foreach (AccountEventListDto item in eventList) eventNames.Add(item.name);
            // Sort the event names
            eventNames.Sort(delegate (string s1, string s2) {
                if (s1 == null && s2 == null) return 0;
                else if (s1 == null) return -1;
                else if (s2 == null) return 1;
                else return String.Compare(s1, s2, StringComparison.CurrentCultureIgnoreCase);
            });
            cmbEvents.Items.AddRange(eventNames.ToArray());
        }

        private void cmbEvents_SelectedIndexChanged(object sender, EventArgs e) {
            cmbSessions.SelectedItem = null;
            cmbSessions.Items.Clear();
            List<EventSessionDto> sessions = new List<EventSessionDto>();
            string name = cmbEvents.Text;
            bool found = false;
            foreach (AccountEventListDto item in eventList) {
                if (!found && (item.name == name)) {
                    // cmbSessions.Items.AddRange(item.sessionList.ToArray());
                    sessions.AddRange(item.sessionList.ToArray());
                    found = true;
                }
            }
            // Sort Session list by start date
            sessions.Sort(delegate (EventSessionDto s1, EventSessionDto s2) {
                if (s1 == null && s2 == null) return 0;
                else if (s1 == null) return -1;
                else if (s2 == null) return 1;
                else return s1.eventStartDate.CompareTo(s2.eventStartDate);
            });
            cmbSessions.Items.AddRange(sessions.ToArray());
            cmbSessions.Enabled = true;
        }

        private void cmbSessions_SelectedIndexChanged(object sender, EventArgs e) {
            btnPrint.Enabled = (cmbSessions.SelectedItem != null);
        }

        private void btnCancel_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void btnPrint_Click(object sender, EventArgs e) {
            EventSessionDto eventSession = cmbSessions.SelectedItem as EventSessionDto;
            if (eventSession == null) return;
            btnPrint.Enabled = false;
            btnCancel.Enabled = false;
            Console.WriteLine("Printing session: " + eventSession.description + " " + eventSession.eventStartDate.ToShortDateString());
            DateTime currentDate = eventSession.bookingStartDate;
            DateTime endDate = (eventSession.bookingEndDate > DateTime.Today) ? DateTime.Today : eventSession.bookingEndDate;
            endDate = endDate.AddDays(1);
            Console.WriteLine("Scanning between: " + currentDate.ToShortDateString() + " - " + endDate.ToShortDateString());
            IList<BookingDto> bookings = new List<BookingDto>();
            // TODO: Remove - ONLY FOR TESTING
            // currentDate = endDate.AddDays(-10);
            int daysBetween = (int)(endDate.Date - currentDate.Date).TotalDays;
            int daysProcessed = 0;
            prgLoading.Maximum = daysBetween+1;
            while (currentDate.Date <= endDate.Date) {
                IList<BookingDto> dailyTransactions = TryBookingAPI.TransactionsGetByDate(currentDate);
                Console.WriteLine("Analyzing " + (int)(daysProcessed * 100 / daysBetween) + "% - " + currentDate.ToShortDateString());
                foreach (BookingDto currItem in dailyTransactions) {
                    bool hasTicket = false;
                    foreach (BookingTicketDto ticket in currItem.bookingTickets) {
                        if (ticket.sessionId == eventSession.id) {
                            hasTicket = true;
                        }
                    }
                    if (hasTicket) bookings.Add(currItem);
                }
                currentDate = currentDate.AddDays(1);
                daysProcessed++;
                prgLoading.Value++;
            }
            try {
                DoorListReportPrinter.printReport(this, eventSession, bookings);
            } catch (Exception) {
                MessageBox.Show("Unable to generate this report.", "Reporting Error");
            }
            prgLoading.Value = 0;
            btnPrint.Enabled = true;
            btnCancel.Enabled = true;
        }
    }
}
