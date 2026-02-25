namespace YACTR.Domain.Model.Climbing.Grade.GradeConverters;

public class PolishGradeConverter : GradeConverter
{
    public PolishGradeConverter()
    {
        GradeSystem = GradeSystemEnum.Polish;
        GradeRanges = [
            (0, 9, "I-"), (10, 17, "I"), (18, 25, "I+"), (26, 34, "II-"), (35, 42, "II"), (43, 51, "II+"),
            (52, 59, "III-"), (60, 67, "III"), (68, 75, "III+"), (76, 84, "IV-"), (85, 92, "IV"), (93, 107, "IV+"),
            (108, 122, "V-"), (123, 137, "V"), (138, 153, "V+"), (154, 167, "VI-"), (168, 183, "VI"), (184, 195, "VI+"),
            (196, 209, "VI.1"), (210, 222, "VI.1+"), (223, 235, "VI.2"), (236, 247, "VI.2+"), (248, 261, "VI.3"), (262, 274, "VI.3+"),
            (275, 289, "VI.4"), (290, 304, "VI.4+"), (305, 319, "VI.5"), (320, 335, "VI.5+"), (336, 349, "VI.6"), (350, 365, "VI.6+"),
            (366, 380, "VI.7"), (381, 395, "VI.7+"), (396, 410, "VI.8"), (411, 425, "VI.8+"), (426, 440, "VI.9"), (441, 500, "VI.9+")
        ];
    }
}

