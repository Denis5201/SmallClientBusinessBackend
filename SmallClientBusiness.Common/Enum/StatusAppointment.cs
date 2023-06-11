using System.Text.Json.Serialization;

namespace SmallClientBusiness.Common.Enum;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum StatusAppointment
{
    New,
    Completed,
    Cancelled
}