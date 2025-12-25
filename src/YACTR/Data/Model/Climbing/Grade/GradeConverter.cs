using System.Collections.Immutable;
using System.Reflection.Metadata.Ecma335;
using YACTR.Data.Model.Climbing.Grade.Converter;

namespace YACTR.Data.Model.Climbing.Grade;

public abstract class GradeConverter
{
    public GradeSystemEnum GradeSystem { get; init;}
    protected List<(int rangeMin, int rangeMax, string stringGrade)> GradeRanges { get; init; } = [];

    // These are currently hard-set independent of the GradeSystem used; 
    // and are effectively the common grade-buckets across all grade systems.
    private readonly ImmutableList<(int rangeMin, int rangeMax, GradeBand band)> GradeBands =
    [
        (0, 100, GradeBand.Beginner),
        (101, 200, GradeBand.Intermediate),
        (201, 300, GradeBand.Experienced),
        (301, 400, GradeBand.Expert),
        (401, 700, GradeBand.Elite)
    ];

    /// <summary>
    /// General factory-method for retrieving the GradeConverter implementation
    /// for a given GradeSystemEnum.
    /// </summary>
    /// <param name="gradeSystem"></param>
    /// <returns>GradeConverter implementation for the provided GradeSystem </returns>
    /// <exception cref="NotImplementedException"></exception>
    public static GradeConverter ForGradeSystem(GradeSystemEnum gradeSystem) => gradeSystem switch
    {
        GradeSystemEnum.Ewbanks => new EwbanksGradeConverter(),
        GradeSystemEnum.YDS => new YDSGradeConverter(),
        GradeSystemEnum.NCCSScale => new NCCSScaleGradeConverter(),
        GradeSystemEnum.French => new FrenchGradeConverter(),
        GradeSystemEnum.BritishTechnical => new BritishTechnicalGradeConverter(),
        GradeSystemEnum.UIAA => new UIAAGradeConverter(),
        GradeSystemEnum.SouthAfrican => new SouthAfricanGradeConverter(),
        GradeSystemEnum.OldSouthAfrican => new OldSouthAfricanGradeConverter(),
        GradeSystemEnum.Saxon => new SaxonGradeConverter(),
        GradeSystemEnum.Finnish => new FinnishGradeConverter(),
        GradeSystemEnum.Norwegian => new NorwegianGradeConverter(),
        GradeSystemEnum.Polish => new PolishGradeConverter(),
        GradeSystemEnum.BrazilTechnical => new BrazilTechnicalGradeConverter(),
        GradeSystemEnum.Swedish => new SwedishGradeConverter(),
        GradeSystemEnum.Russian => new RussianGradeConverter(),
        _ => throw new NotImplementedException($"GradeConverter for '{gradeSystem}' not implemented."),
    };
    
    /// <summary>
    /// Converts a single integer grade indicator to a Grade object in 
    /// the GradeSystem of the converter.
    /// </summary>
    /// <param name="gradeInt"></param>
    /// <returns>Grade object equivalent to gradeInt provided</returns>
    public Grade Convert(int gradeInt)
    {
        return new()
        {
            GradeInt = gradeInt,
            GradeString = ToGradeString(gradeInt),
            GradeBand = ToGradeBand(gradeInt),
            GradeSystem = GradeSystem
        };
    }

    /// <summary>
    /// Converts a grade string to the median integer value representation 
    /// of that grade in the GradeSystem of the converter.
    /// </summary>
    /// <param name="gradeString"></param>
    /// <returns></returns>
    public Grade Convert(string gradeString)
    {
        var gradeInt = ToGradeInt(gradeString);
        return new()
        {
            GradeInt = gradeInt,
            GradeString = gradeString,
            GradeBand = ToGradeBand(gradeInt),
            GradeSystem = GradeSystem
        };
    }

    /// <summary>
    /// Returns the median integer value for the grade "bucket" which 
    /// matches the provided gradeString.
    /// </summary>
    /// <param name="gradeString"></param>
    /// <returns>median grade int equivalent for grade string</returns>
    private int ToGradeInt(string gradeString)
    {
        var (rangeMin, rangeMax, stringGrade) = GradeRanges.Find(gradeRange => gradeRange.stringGrade.Equals(gradeString));

        return (rangeMin + rangeMax) / 2;
    }

    private string ToGradeString(int gradeInt)
    {
        foreach (var (rangeMin, rangeMax, stringGrade) in GradeRanges)
        {
            // Effectively doing a comparison for if the number is equal or in range of the defined min/max.
            // This works since if it is outside of the range, the number will always be _negative_, and when
            // equal to 0, it is inclusively inside the range (as one of the parentheses will be 0).
            if ((gradeInt - rangeMin) * (rangeMax - gradeInt) >= 0)
            {
                return stringGrade;
            }
        }

        // If the foreach loop completes without a return; we cannot determine the grade string of the grade.
        throw new ArgumentOutOfRangeException(nameof(gradeInt), "Numerical grade is out of bounds.");
    }

    private GradeBand ToGradeBand(int gradeInt)
    {
        foreach (var (rangeMin, rangeMax, gradeBand) in GradeBands)
        {
            if ((gradeInt - rangeMin) * (rangeMax - gradeInt)>= 0)
            {
                return gradeBand;
            }
        }

        // If the foreach loop completes without a return; we cannot determine the grade string of the grade.
        throw new ArgumentOutOfRangeException(nameof(gradeInt), "Numerical grade is out of bounds.");
    }
}