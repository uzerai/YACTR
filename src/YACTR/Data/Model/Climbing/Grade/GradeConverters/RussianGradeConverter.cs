namespace YACTR.Data.Model.Climbing.Grade.Converter;

public class RussianGradeConverter : GradeConverter
{
    public RussianGradeConverter()
    {
        GradeSystem = GradeSystemEnum.Russian;
        GradeRanges = [
            (0, 16, "1A"), (17, 31, "1B"), (32, 46, "2A"), (47, 62, "2B"), (63, 76, "3A"), (77, 92, "3B"),
            (93, 137, "4A"), (138, 183, "4B"), (184, 228, "5A"), (229, 274, "5B"), (275, 319, "6A"), (320, 365, "6B"),
            (366, 410, "7A"), (411, 500, "7B")
        ];
    }
}

