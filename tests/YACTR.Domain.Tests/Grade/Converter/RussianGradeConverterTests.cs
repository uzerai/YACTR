using Shouldly;
using YACTR.Domain.Model.Climbing.Grade.GradeConverters;

namespace YACTR.Domain.Tests.Grade.Converter;

public class RussianGradeConverterTests : GradeConverterTests<RussianGradeConverter>
{
    [Theory]
    [InlineData(2, "1A")]
    [InlineData(17, "1B")]
    [InlineData(32, "2A")]
    [InlineData(47, "2B")]
    [InlineData(63, "3A")]
    [InlineData(77, "3B")]
    [InlineData(93, "4A")]
    [InlineData(138, "4B")]
    [InlineData(184, "5A")]
    [InlineData(229, "5B")]
    [InlineData(275, "6A")]
    [InlineData(320, "6B")]
    [InlineData(366, "7A")]
    [InlineData(411, "7B")]
    public void GradeConverter_GetGradeString_ConvertsCorrectly(int numericalGrade, string gradeString)
    {
        var outputGrade = Sut.Convert(numericalGrade);

        outputGrade.GradeString.ShouldBeEquivalentTo(gradeString);
    }
}

