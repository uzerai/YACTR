namespace YACTR.Domain.Model.Climbing.Grade.GradeConverters;

public class BritishTechnicalGradeConverter : GradeConverter
{
    public BritishTechnicalGradeConverter()
    {
        GradeSystem = GradeSystemEnum.BritishTechnical;
        GradeRanges = [
            (0, 10, "1a"), (11, 19, "1b"), (20, 28, "1c"), (29, 37, "2a"), (38, 46, "2b"), (47, 55, "2c"),
            (56, 65, "3a"), (66, 74, "3b"), (75, 83, "3c"), (84, 92, "4a"), (93, 122, "4b"), (123, 153, "4c"),
            (154, 183, "5a"), (184, 213, "5b"), (214, 244, "5c"), (245, 274, "6a"), (275, 304, "6b"), (305, 335, "6c"),
            (336, 365, "7a"), (366, 395, "7b"), (396, 500, "7c")
        ];
    }
}

