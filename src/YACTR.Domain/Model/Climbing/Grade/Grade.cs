namespace YACTR.Domain.Model.Climbing.Grade;

public record Grade
{
    public required int GradeInt { get; init; }
    public required string GradeString { get; init; }
    public required GradeBand GradeBand { get; init; }
    public required GradeSystemEnum GradeSystem { get; init; }

    public static Grade From(GradeSystemEnum gradeSystem, int gradeInt)
    {
        return GradeConverter.ForGradeSystem(gradeSystem)
            .Convert(gradeInt);
    }

    public static Grade From(GradeSystemEnum gradeSystem, string gradeString)
    {
        return GradeConverter.ForGradeSystem(gradeSystem)
            .Convert(gradeString);
    }
}