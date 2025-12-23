using System.Collections.Immutable;

namespace YACTR.Data.Model.Climbing.Grade;

public abstract class GradeConverter
{
    protected GradeSystemEnum GradeSystem { get; init;}
    protected List<(int rangeMin, int rangeMax, string stringGrade)> GradeRanges { get; init; } = [];
    private readonly ImmutableList<(int rangeMin, int rangeMax, GradeBand band)> GradeBands =
    [
        (0, 100, GradeBand.Beginner),
        (101, 200, GradeBand.Intermediate),
        (201, 300, GradeBand.Experienced),
        (301, 400, GradeBand.Expert),
        (401, 700, GradeBand.Elite)
    ];
    
    public Grade ConvertToGrade(int numericalGrade)
    {
        return new()
        {
            NumericalGrade = numericalGrade,
            StringGrade = GetGradeString(numericalGrade),
            GradeBand = GetGradeBand(numericalGrade),
            GradeSystem = GradeSystem
        };
    }

    private string GetGradeString(int numericalGrade)
    {
        foreach (var (rangeMin, rangeMax, stringGrade) in GradeRanges)
        {
            // Effectively doing a comparison for if the number is equal or in range of the defined min/max.
            // This works since if it is outside of the range, the number will always be _negative_, and when
            // equal to 0, it is inclusively inside the range (as one of the parentheses will be 0).
            if ((numericalGrade - rangeMin) * (rangeMax - numericalGrade) >= 0)
            {
                return stringGrade;
            }
        }

        // If the foreach loop completes without a return; we cannot determine the grade string of the grade.
        throw new ArgumentOutOfRangeException(nameof(numericalGrade), "Numerical grade is out of bounds.");
    }

    private GradeBand GetGradeBand(int numericalGrade)
    {
        foreach (var (rangeMin, rangeMax, gradeBand) in GradeBands)
        {
            var debug = (numericalGrade - rangeMin) * (rangeMax - numericalGrade);
            if (debug >= 0)
            {
                return gradeBand;
            }
        }

        // If the foreach loop completes without a return; we cannot determine the grade string of the grade.
        throw new ArgumentOutOfRangeException(nameof(numericalGrade), "Numerical grade is out of bounds.");
    }
}