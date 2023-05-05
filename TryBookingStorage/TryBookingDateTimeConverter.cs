using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using RestSharp;

namespace TryBookingStorage {
    class TryBookingDateTimeConverter : JsonConverter<DateTime> {
        public static string DateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'";
        public static string DateFormat = "yyyy'-'MM'-'dd'T00:00:00'";

        /*public override DateTime Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options) =>
                DateTime.ParseExact(reader.GetString(),
                    DateTimeFormat, CultureInfo.InvariantCulture); */

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
            string str = reader.GetString();
            if (str.EndsWith("Z")) {
                return DateTime.ParseExact(str, DateTimeFormat, CultureInfo.InvariantCulture);
            } else {
                return DateTime.ParseExact(str, DateFormat, CultureInfo.InvariantCulture);
            }
        }

        public override void Write(
            Utf8JsonWriter writer,
            DateTime dateTimeValue,
            JsonSerializerOptions options) =>
                writer.WriteStringValue(dateTimeValue.ToString(
                    DateTimeFormat, CultureInfo.InvariantCulture));
    }

    class ListConverter<T> : JsonConverter<List<T>> {

        public override List<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
            return JsonSerializer.Deserialize<List<T>>(ref reader, options);
        }

        public override void Write(Utf8JsonWriter writer, List<T> value, JsonSerializerOptions options) {
            JsonSerializer.Serialize<List<T>>(writer, value, options);
        }
    }
}
