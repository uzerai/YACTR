using Shouldly;
using YACTR.Data.Model.Climbing.Grade;

namespace YACTR.Tests.UnitTests.GradeConverter;

public abstract class GradeConverterTests<T> where T : Data.Model.Climbing.Grade.GradeConverter, new()
{
    public T sut = new();

    [Theory]
    [InlineData(1, GradeBand.Beginner)]
    [InlineData(101, GradeBand.Intermediate)]
    [InlineData(201, GradeBand.Experienced)]
    [InlineData(301, GradeBand.Expert)]
    [InlineData(401, GradeBand.Elite)]
    public void GradeConverter_GetGradeBand_ConvertsCorrectly(int numericalGrade, GradeBand expectedBand)
    {
        var outputGrade = sut.ConvertToGrade(numericalGrade);

        outputGrade.GradeBand.ShouldBe(expectedBand);
    }
}