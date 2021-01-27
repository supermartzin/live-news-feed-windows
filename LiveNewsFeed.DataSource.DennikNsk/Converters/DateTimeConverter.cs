using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LiveNewsFeed.DataSource.DennikNsk.Converters
{
    public class DateTimeConverter : JsonConverter<DateTime>
    {
        private readonly string _dateTimeFormat;

        internal DateTimeConverter(string dateTimeFormat)
        {
            _dateTimeFormat = dateTimeFormat ?? throw new ArgumentNullException(nameof(dateTimeFormat));
        }

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (typeToConvert != typeof(DateTime))
                throw new JsonException($"Invalid type '{typeToConvert.FullName}' used in DateTimeConverter.");

            if (DateTime.TryParseExact(reader.GetString(),
                                        _dateTimeFormat,
                                        CultureInfo.InvariantCulture,
                                        DateTimeStyles.AssumeUniversal,
                                        out var parsed))
            {
                return parsed;
            }

            throw new JsonException("Cannot parse provided DateTime string - invalid format.");
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(_dateTimeFormat, CultureInfo.InvariantCulture));
        }
    }
}