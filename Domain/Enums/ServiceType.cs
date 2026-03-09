using System.Text.Json.Serialization;

namespace BasicBilling.API.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ServiceType
{
    Water,
    Electricity,
    Sewer
}
