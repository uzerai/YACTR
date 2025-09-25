using System.Runtime.Serialization;

namespace YACTR.Data.Model.Achievement;

public enum AscentType
{
    [EnumMember(Value = "Tick")]
    Tick,
    [EnumMember(Value = "Onsight")]
    Onsight,
    [EnumMember(Value = "Flash")]
    Flash,
    [EnumMember(Value = "Redpoint")]
    Redpoint,
    [EnumMember(Value = "Seconded")]
    Seconded,
    [EnumMember(Value = "Repeat")]
    Repeat,
    [EnumMember(Value = "Project")]
    Project,
    [EnumMember(Value = "Dab")]
    Dab,
}