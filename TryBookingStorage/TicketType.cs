using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TryBookingStorage {

    /// <summary>
    /// Enumeration of standard ticket types
    /// </summary>
    public enum TicketType {
        Adult, MemberConcession, Student
    }

    /// <summary>
    /// Contains helper methods for manipulating the TicketType enumeration.
    /// </summary>
    public sealed class TicketTypeHelper {
        private TicketTypeHelper() { }

        /// <summary>
        /// Determine the TicketType from the TryBooking value.
        /// </summary>
        /// <param name="name">TryBooking name to lookup</param>
        /// <returns>TicketType for this name, defaulting to Adult for unknown types</returns>
        public static TicketType getTicketType(string name) {
            switch (name) {
                case "Member/Concession": return TicketType.MemberConcession;
                case "Student": return TicketType.Student;
                default: return TicketType.Adult;
            }
        }

        /// <summary>
        /// Retrieve a display value for a given ticket type.
        /// </summary>
        /// <param name="t">TicketType to lookup</param>
        /// <returns>String suitable for display for this type, or empty string for unknown types</returns>
        public static string getTicketTypeName(TicketType t) {
            switch (t) {
                case TicketType.Adult: return "Adult";
                case TicketType.Student: return "Student";
                case TicketType.MemberConcession: return "Member/Conc";
                default: return "";
            }
        }
    }
}
