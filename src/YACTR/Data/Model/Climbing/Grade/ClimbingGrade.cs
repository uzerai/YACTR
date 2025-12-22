namespace YACTR.Data.Model.Climbing.Grade;

/// <summary>
/// The climbing grade of a route or pitch, and the conversion to each system.
/// </summary>
public class ClimbingGrade
{
    public enum GradeBand
    {
        Beginner,
        Intermediate,
        Experienced,
        Expert,
        Elite,
    }

    public Dictionary<GradeSystemEnum, string> gradeStrings = [];

    public ClimbingGrade(ClimbingType type, int numericalGrade)
    {
        //TODO: Grade system conversions should go from integer 0 - 600 (for now); each band has a certain amount of range
        // The experienced and expert bands have some more leeway as the grades are more nuanced at those ranges, and more mixed in most systems.
        // eg: Beginner -> 0 - 100
        // Intermediate -> 101 -> 200
        // Experienced -> 201 -> 350,
        // Expert -> 351 -> 500
        // Elite -> 501+
        foreach (GradeSystemEnum grade in Enum.GetValues<GradeSystemEnum>())
        {
            gradeStrings[grade] = grade switch
            {
                GradeSystemEnum.VScale => throw new NotImplementedException(),
                GradeSystemEnum.BScale => throw new NotImplementedException(),
                GradeSystemEnum.SScale => throw new NotImplementedException(),
                GradeSystemEnum.PScale => throw new NotImplementedException(),
                GradeSystemEnum.JoshuaTreeScale => throw new NotImplementedException(),
                GradeSystemEnum.Fontainebleau => throw new NotImplementedException(),
                GradeSystemEnum.AnnotBScale => throw new NotImplementedException(),
                GradeSystemEnum.FontTraverse => throw new NotImplementedException(),
                GradeSystemEnum.Ewbanks => throw new NotImplementedException(),
                GradeSystemEnum.YDS => throw new NotImplementedException(),
                GradeSystemEnum.NCCSScale => throw new NotImplementedException(),
                GradeSystemEnum.French => FrenchGrade(numericalGrade),
                GradeSystemEnum.BritishTechnical => throw new NotImplementedException(),
                GradeSystemEnum.UIAA => throw new NotImplementedException(),
                GradeSystemEnum.SouthAfrican => throw new NotImplementedException(),
                GradeSystemEnum.OldSouthAfrican => throw new NotImplementedException(),
                GradeSystemEnum.Saxon => throw new NotImplementedException(),
                GradeSystemEnum.Finnish => throw new NotImplementedException(),
                GradeSystemEnum.Norwegian => throw new NotImplementedException(),
                GradeSystemEnum.Polish => throw new NotImplementedException(),
                GradeSystemEnum.BrazilTechnical => throw new NotImplementedException(),
                GradeSystemEnum.Swedish => throw new NotImplementedException(),
                GradeSystemEnum.Russian => throw new NotImplementedException(),
                GradeSystemEnum.AidMScale => throw new NotImplementedException(),
                GradeSystemEnum.AidAScale => throw new NotImplementedException(),
                GradeSystemEnum.AidCScale => throw new NotImplementedException(),
                GradeSystemEnum.AlpineIce => throw new NotImplementedException(),
                GradeSystemEnum.WaterIce => throw new NotImplementedException(),
                GradeSystemEnum.MixedRockIce => throw new NotImplementedException(),
                GradeSystemEnum.FerrataSchall => throw new NotImplementedException(),
                GradeSystemEnum.FerrataNum => throw new NotImplementedException(),
                GradeSystemEnum.FerrataFrench => throw new NotImplementedException(),
                GradeSystemEnum.ScottishWinterTechnical => throw new NotImplementedException(),
                _ => throw new NotImplementedException(),
            };
        }
    }

    private static string NorwegianGrade(int numericalGrade)
    {
        return numericalGrade switch
        {
            <= 5 => "1-",
            > 5 and <= 10 => "1",
            > 10 and <= 20 => "1+",
            > 20 and <= 30 => "2-",
            > 30 and <= 40 => "2",
            > 40 and <= 50 => "2+",
            > 50 and <= 60 => "3-",
            > 60 and <= 70 => "3",
            > 70 and <= 80 => "3+",
            > 80 and <= 90 => "4-",
            > 90 and <= 100 => "4",
            > 100 and <= 120 => "4+",
            > 120 and <= 140 => "5-",
            > 140 and <= 160 => "5",
            > 160 and <= 180 => "5+",
            > 180 and <= 210 => "6-",
            > 210 and <= 226 => "6",
            > 226 and <= 242 => "6+",
            > 242 and <= 258 => "7-",
            > 258 and <= 274 => "7",
            > 274 and <= 290 => "7+",
            > 290 and <= 306 => "8-",
            > 306 and <= 322 => "8",
            > 322 and <= 350 => "8+",
            > 350 and <= 368 => "9-",
            > 368 and <= 386 => "9",
            > 386 and <= 410 => "9+",
            > 410 and <= 430 => "10-",
            > 430 and <= 450 => "10",
            > 450 and <= 490 => "10+",
            > 490 => "11"
        };
    }

    private static string FrenchGrade(int numericalGrade)
    {
        return numericalGrade switch
        {
            <= 5 => "1a",
            > 5 and <= 10 => "1a+",
            > 10 and <= 15 => "1b",
            > 15 and <= 20 => "1b+",
            > 20 and <= 25 => "1c",
            > 25 and <= 30 => "1c+",
            > 30 and <= 35 => "2a",
            > 35 and <= 40 => "2a+",
            > 40 and <= 45 => "2b",
            > 45 and <= 50 => "2b+",
            > 50 and <= 55 => "2c",
            > 55 and <= 60 => "2c+",
            > 60 and <= 65 => "3a",
            > 65 and <= 70 => "3a+",
            > 70 and <= 75 => "3b",
            > 75 and <= 80 => "3b+",
            > 80 and <= 85 => "3c",
            > 85 and <= 100 => "3c+",
            > 100 and <= 105 => "4a",
            > 105 and <= 113 => "4a+",
            > 113 and <= 121 => "4b",
            > 121 and <= 129 => "4b+",
            > 129 and <= 137 => "4c",
            > 137 and <= 145 => "4c+",
            > 145 and <= 153 => "5a",
            > 153 and <= 161 => "5a+",
            > 161 and <= 169 => "5b",
            > 169 and <= 177 => "5b+",
            > 177 and <= 185 => "5c",
            > 185 and <= 193 => "5c+",
            > 193 and <= 200 => "6a",
            > 200 and <= 215 => "6a+",
            > 215 and <= 230 => "6b",
            > 230 and <= 245 => "6b+",
            > 245 and <= 260 => "6c",
            > 260 and <= 275 => "6c+",
            > 275 and <= 290 => "7a",
            > 290 and <= 300 => "7a+",
            > 300 and <= 313 => "7b",
            > 313 and <= 326 => "7b+",
            > 326 and <= 339 => "7c",
            > 339 and <= 352 => "7c+",
            > 352 and <= 365 => "8a",
            > 365 and <= 378 => "8a+",
            > 378 and <= 391 => "8b",
            > 391 and <= 400 => "8b+",
            > 400 and <= 415 => "8c",
            > 415 and <= 430 => "8c+",
            > 430 and <= 445 => "9a",
            > 445 and <= 460 => "9a+",
            > 460 and <= 475 => "9b",
            > 475 and <= 490 => "9b+",
            > 490 => "9c",
        };
    }
}