using Shouldly;
using YACTR.Domain.Model.Climbing.Grade;

namespace YACTR.Domain.Tests.Grade.Converter;

public abstract class GradeConverterTests<T> where T : GradeConverter, new()
{
    protected readonly T Sut = new();

    [Theory]
    [InlineData(1, GradeBand.Beginner)]
    [InlineData(101, GradeBand.Intermediate)]
    [InlineData(201, GradeBand.Experienced)]
    [InlineData(301, GradeBand.Expert)]
    [InlineData(401, GradeBand.Elite)]
    public void GradeConverter_GetGradeBand_ConvertsCorrectly(int numericalGrade, GradeBand expectedBand)
    {
        var outputGrade = Sut.Convert(numericalGrade);

        outputGrade.GradeBand.ShouldBe(expectedBand);
    }

    [Fact]
    public void GradeConverter_ForGradeSystem_ReturnsCorrectGradeSystem()
    {
        object outputConverter = GradeConverter.ForGradeSystem(Sut.GradeSystem);

        outputConverter.ShouldBeOfType(Sut.GetType());
    }
}