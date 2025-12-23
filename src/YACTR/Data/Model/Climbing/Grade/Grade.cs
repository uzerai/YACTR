namespace YACTR.Data.Model.Climbing.Grade;

public record Grade
{
    public required int NumericalGrade { get; init; }
    public required string StringGrade { get; init; }
    public required GradeBand GradeBand { get; init; }
    public required GradeSystemEnum GradeSystem { get; init; }
}