namespace YACTR.Data.Model.Climbing.Grade.Converter;

public class OldSouthAfricanGradeConverter : GradeConverter
{
    public OldSouthAfricanGradeConverter()
    {
        GradeSystem = GradeSystemEnum.OldSouthAfrican;
        GradeRanges = [
            (0, 6, "A1"), (7, 13, "A2"), (14, 18, "A3"), (19, 24, "B1"), (25, 29, "B2"), (30, 35, "B3"),
            (36, 41, "C1"), (42, 46, "C2"), (47, 52, "C3"), (53, 58, "D1"), (59, 64, "D2"), (65, 69, "D3"),
            (70, 75, "E1"), (76, 81, "E2"), (82, 86, "E3"), (87, 92, "F1"), (93, 115, "F2"), (116, 137, "F3"),
            (138, 160, "G1"), (161, 183, "G2"), (184, 205, "G3"), (206, 228, "H1"), (229, 251, "H2"), (252, 274, "H3"),
            (275, 296, "I1"), (297, 319, "I2"), (320, 342, "I3"), (343, 365, "J1"), (366, 378, "J2"), (379, 500, "J3")
        ];
    }
}

