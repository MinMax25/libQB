using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using libQB.ValueConverters;

namespace libQB.Enums;

[JsonConverter(typeof(JsonStringEnumConverter<Languages>))]
[TypeConverter(typeof(EnumDisplayTypeConverter))]
public enum Languages
{
    [Display(Name = "English")]
    English,

    [Display(Name = "日本語")]
    Japanese,
}
