namespace YACTR.Data.Model.Climbing.Grade.Converter;

public class BrazilTechnicalGradeConverter : GradeConverter
{
    public BrazilTechnicalGradeConverter()
    {
        GradeSystem = GradeSystemEnum.BrazilTechnical;
        GradeRanges = [
            (0, 19, "I"), (20, 37, "Isup"), (38, 55, "II"), (56, 74, "IIsup"), (75, 92, "III"), (93, 115, "IIIsup"),
            (116, 137, "IV"), (138, 160, "IVsup"), (161, 183, "V"), (184, 195, "Vsup"), (196, 209, "VI"), (210, 222, "VIsup"),
            (223, 235, "VIIa"), (236, 248, "VIIb"), (249, 261, "VIIc"), (262, 274, "VIIIa"), (275, 285, "VIIIb"), (286, 296, "VIIIc"),
            (297, 308, "IXa"), (309, 319, "IXb"), (320, 331, "IXc"), (332, 342, "Xa"), (343, 354, "Xb"), (355, 365, "Xc"),
            (366, 377, "XIa"), (378, 391, "XIb"), (392, 404, "XIc"), (405, 416, "XIIa"), (417, 430, "XIIb"), (431, 500, "XIIc")
        ];
    }
}

