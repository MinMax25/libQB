using System.Text.Json.Serialization;

namespace libQB.Enums;

[JsonConverter(typeof(JsonStringEnumConverter<BaseTheme>))]
public enum BaseTheme
{
    Light,
    Dark,
}
