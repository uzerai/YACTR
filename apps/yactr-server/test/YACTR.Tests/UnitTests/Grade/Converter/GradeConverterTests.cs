using Shouldly;
using YACTR.Data.Model.Climbing.Grade;

namespace YACTR.Tests.UnitTests.Grade.Converter;

public abstract class GradeConverterTests<T> where T : GradeConverter, new()
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
        var outputGrade = sut.Convert(numericalGrade);

        outputGrade.GradeBand.ShouldBe(expectedBand);
    }

    [Fact]
    public void GradeConverter_ForGradeSystem_ReturnsCorrectGradeSystem()
    {
        object outputConverter = GradeConverter.ForGradeSystem(sut.GradeSystem);

        outputConverter.ShouldBeOfType(sut.GetType());
    }
}