namespace YACTR.Data.Model.Climbing.Grade;

public class NCCSScaleGradeConverter : GradeConverter
{
    public NCCSScaleGradeConverter()
    {
        GradeSystem = GradeSystemEnum.NCCSScale;
        GradeRanges = [
            (2, 46, "F4"), (47, 92, "F5"), (93, 115, "F6"), (116, 137, "F7"), (138, 160, "F8"), (161, 183, "F9"),
            (184, 201, "F10"), (202, 219, "F11"), (220, 237, "F12"), (238, 255, "F13"), (256, 274, "F14"),
            (275, 296, "F15"), (297, 319, "F16")
        ];
    }
}

