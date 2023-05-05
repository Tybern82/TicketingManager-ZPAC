using System;
using System.Collections.Generic;
using System.Text.Json;
using RestSharp;
using RestSharp.Authenticators;
using TryBookingStorage.DTO;

namespace TryBookingStorage {
    public class TryBookingAPI {

        private static void doUnauthorizedResponse() {
            // TODO: Inform user
            Console.WriteLine("ERROR: Unauthorized");
        }

        private static void doInternalServerErrorResponse() {
            // TODO: Inform user
            Console.WriteLine("ERROR: Internal Server Error");
        }

        private static IRestResponse doTransaction(string command) {
            RestClient client = new RestClient("https://api.trybooking.com/" + TryBookingLogin.regionCodeID(TryBookingLogin.CurrentRegion) + command);
            client.Authenticator = new HttpBasicAuthenticator(TryBookingLogin.CurrentUsername, TryBookingLogin.CurrentPassword);
            var request = new RestRequest(Method.GET);
            request.AddHeader("Accept", "application/json");
            return client.Execute(request);
        }

        private static string getRequestDate(DateTime dt) {
            return dt.ToUniversalTime().ToString("yyyy'-'MM'-'dd");
        }

        public static IList<AccountTransactionDto> AccountGetByDate(DateTime startDate, DateTime endDate) {
            // "https://api.trybooking.com/{region}/reporting/v1/account?startDate=string&endDate=string"
            IRestResponse response = doTransaction("/reporting/v1/account?startDate=" + getRequestDate(startDate) + "&endDate=" + getRequestDate(endDate));

            if (response.StatusCode == System.Net.HttpStatusCode.OK) {
                return JsonSerializer.Deserialize<IList<AccountTransactionDto>>(response.Content);
            } else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized) {
                doUnauthorizedResponse();
            } else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError) {
                doInternalServerErrorResponse();
            }
            return new List<AccountTransactionDto>();
        }

        public static BookingDto TransactionsGetByTransactionId(string transactionID) {
            // "https://api.trybooking.com/{region}/reporting/v1/bookings/{transactionId}"
            IRestResponse response = doTransaction("/reporting/v1/bookings/" + transactionID);
            if (response.StatusCode == System.Net.HttpStatusCode.OK) {
                return JsonSerializer.Deserialize<BookingDto>(response.Content);
            } else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized) {
                doUnauthorizedResponse();
            } else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError) {
                doInternalServerErrorResponse();
            }
            return new BookingDto();
        }

        private static IDictionary<string, List<BookingDto>> fetchedTransactions = new Dictionary<string, List<BookingDto>>();

        public static List<BookingDto> TransactionsGetByDate(DateTime date) {
            // "https://api.trybooking.com/{region}/reporting/v1/bookings?date=string"
            string requestDate = getRequestDate(date);
            string todayDate = getRequestDate(DateTime.Today);
            // Always reload today's bookings - may have changed since last load
            if ((!String.Equals(requestDate, todayDate)) && fetchedTransactions.ContainsKey(requestDate)) return fetchedTransactions[requestDate];
            IRestResponse response = doTransaction("/reporting/v1/bookings?date=" + requestDate);
            // Console.WriteLine(response.Content);
            if (response.StatusCode == System.Net.HttpStatusCode.OK) {
                List<BookingDto> result = JsonSerializer.Deserialize<List<BookingDto>>(response.Content);
                fetchedTransactions.Add(requestDate, result);
                return result;
            } else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized) {
                doUnauthorizedResponse();
            } else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError) {
                doInternalServerErrorResponse();
            }
            return new List<BookingDto>();
        }

        public static IList<AccountEventListDto> EventGetByAccountId() {
            // "https://api.trybooking.com/{region}/reporting/v1/event"
            IRestResponse response = doTransaction("/reporting/v1/event");
            Console.WriteLine(response.Content);
            if (response.StatusCode == System.Net.HttpStatusCode.OK) {
                return JsonSerializer.Deserialize<IList<AccountEventListDto>>(response.Content);
            } else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized) {
                doUnauthorizedResponse();
            } else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError) {
                doInternalServerErrorResponse();
            }
            return new List<AccountEventListDto>();
        }

        public static SessionAvailabilityDto EventGetBySessionId(Int32 sessionId) {
            // "https://api.trybooking.com/{region}/reporting/v1/event/session?sesionId=int32"
            IRestResponse response = doTransaction("/reporting/v1/event/session?sessionId=" + sessionId);
            if (response.StatusCode == System.Net.HttpStatusCode.OK) {
                return JsonSerializer.Deserialize<SessionAvailabilityDto>(response.Content);
            } else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized) {
                doUnauthorizedResponse();
            } else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError) {
                doInternalServerErrorResponse();
            }
            return new SessionAvailabilityDto();
        }

        public static IList<FundraisingDto> FundraisingGetByAccountId() {
            // "https://api.trybooking.com/{region}/reporting/v1/fundraising"
            IRestResponse response = doTransaction("/reporting/v1/fundraising");
            if (response.StatusCode == System.Net.HttpStatusCode.OK) {
                return JsonSerializer.Deserialize<IList<FundraisingDto>>(response.Content);
            } else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized) {
                doUnauthorizedResponse();
            } else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError) {
                doInternalServerErrorResponse();
            }
            return new List<FundraisingDto>();
        }

        public static IList<TicketSalesDto> GetAccountTicketSalesByTimePeriod(DateTime fromDate, DateTime toDate, TryBookingLogin.TryBookingDatePeriod datePeriod) {
            // "https://api.trybooking.com/{region}/reporting/v1/sales/ticket?fromDate=string&toDate=string&datePeriod=1"
            IRestResponse response = doTransaction("/reporting/v1/sales/ticket?fromDate=" + getRequestDate(fromDate) + "&toDate=" + getRequestDate(toDate) + "&datePeriod=" + datePeriod);
            if (response.StatusCode == System.Net.HttpStatusCode.OK) {
                return JsonSerializer.Deserialize<IList<TicketSalesDto>>(response.Content);
            } else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized) {
                doUnauthorizedResponse();
            } else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError) {
                doInternalServerErrorResponse();
            }
            return new List<TicketSalesDto>();
        }

        public static IList<BookingSalesDto> GetAccountBookingSalesByTimePeriod(DateTime fromDate, DateTime toDate, TryBookingLogin.TryBookingDatePeriod datePeriod) {
            // "https://api.trybooking.com/{region}/reporting/v1/sales/booking?fromDate=string&toDate=string&datePeriod=1"
            IRestResponse response = doTransaction("/reporting/v1/sales/booking?fromDate=" + getRequestDate(fromDate) + "&toDate=" + getRequestDate(toDate) + "&datePeriod=" + datePeriod);
            if (response.StatusCode == System.Net.HttpStatusCode.OK) {
                return JsonSerializer.Deserialize<IList<BookingSalesDto>>(response.Content);
            } else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized) {
                doUnauthorizedResponse();
            } else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError) {
                doInternalServerErrorResponse();
            }
            return new List<BookingSalesDto>();
        }

        public static IList<EventSalesDto> GetAccountEventSalesByTimePeriod(DateTime fromDate, DateTime toDate, TryBookingLogin.TryBookingDatePeriod datePeriod) {
            // "https://api.trybooking.com/{region}/reporting/v1/sales/event?fromDate=string&toDate=string&datePeriod=1"
            IRestResponse response = doTransaction("/reporting/v1/sales/event?fromDate=" + getRequestDate(fromDate) + "&toDate=" + getRequestDate(toDate) + "&datePeriod=" + datePeriod);
            if (response.StatusCode == System.Net.HttpStatusCode.OK) {
                return JsonSerializer.Deserialize<IList<EventSalesDto>>(response.Content);
            } else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized) {
                doUnauthorizedResponse();
            } else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError) {
                doInternalServerErrorResponse();
            }
            return new List<EventSalesDto>();
        }

        public static IList<FundraisingSalesDto> GetAccountFundraisingSalesByTimePeriod(DateTime fromDate, DateTime toDate, TryBookingLogin.TryBookingDatePeriod datePeriod) {
            // "https://api.trybooking.com/{region}/reporting/v1/sales/fundraising?fromDate=string&toDate=string&datePeriod=1"
            IRestResponse response = doTransaction("/reporting/v1/sales/fundraising?fromDate=" + getRequestDate(fromDate) + "&toDate=" + getRequestDate(toDate) + "&datePeriod=" + datePeriod);
            if (response.StatusCode == System.Net.HttpStatusCode.OK) {
                return JsonSerializer.Deserialize<IList<FundraisingSalesDto>>(response.Content);
            } else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized) {
                doUnauthorizedResponse();
            } else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError) {
                doInternalServerErrorResponse();
            }
            return new List<FundraisingSalesDto>();
        }

        public static TicketAttendanceScanListDto GetAttendanceScans(Int32 sessionID) {
            // "https://api.trybooking.com/{region}/reporting/v1/scans/{sessionId}/attendance"
            IRestResponse response = doTransaction("/reporting/v1/scans/" + sessionID + "/attendance");
            if (response.StatusCode == System.Net.HttpStatusCode.OK) {
                return JsonSerializer.Deserialize<TicketAttendanceScanListDto>(response.Content);
            } else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized) {
                doUnauthorizedResponse();
            } else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError) {
                doInternalServerErrorResponse();
            }
            return new TicketAttendanceScanListDto();
        }

        public static TicketScanListDto GetAllScans(Int32 sessionID) {
            // "https://api.trybooking.com/{region}/reporting/v1/scans/{sessionId}/all"
            IRestResponse response = doTransaction("/reporting/v1/scans/" + sessionID + "/all");
            if (response.StatusCode == System.Net.HttpStatusCode.OK) {
                return JsonSerializer.Deserialize<TicketScanListDto>(response.Content);
            } else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized) {
                doUnauthorizedResponse();
            } else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError) {
                doInternalServerErrorResponse();
            }
            return new TicketScanListDto();
        }
    }
}
