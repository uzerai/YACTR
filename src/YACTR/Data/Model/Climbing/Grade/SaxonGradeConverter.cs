namespace YACTR.Data.Model.Climbing.Grade;

public class SaxonGradeConverter : GradeConverter
{
    public SaxonGradeConverter()
    {
        GradeSystem = GradeSystemEnum.Saxon;
        GradeRanges = [
            (2, 31, "I"), (32, 62, "II"), (63, 92, "III"), (93, 107, "IV"), (108, 122, "V"), (123, 137, "VI"),
            (138, 153, "VIIa"), (154, 167, "VIIb"), (168, 183, "VIIc"), (184, 198, "VIIIa"), (199, 213, "VIIIb"), (214, 228, "VIIIc"),
            (229, 244, "IXa"), (245, 258, "IXb"), (259, 274, "IXc"), (275, 289, "Xa"), (290, 304, "Xb"), (305, 319, "Xc"),
            (320, 335, "XIa"), (336, 349, "XIb"), (350, 365, "XIc"), (366, 380, "XIIa"), (381, 395, "XIIb"), (396, 410, "XIIc"),
            (411, 425, "XIIIa"), (426, 440, "XIIIb"), (441, 455, "XIIIc")
        ];
    }
}

