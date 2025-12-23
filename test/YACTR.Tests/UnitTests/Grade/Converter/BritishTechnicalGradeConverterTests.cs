using Shouldly;
using YACTR.Data.Model.Climbing.Grade.Converter;

namespace YACTR.Tests.UnitTests.Grade.Converter;

public class BritishTechnicalGradeConverterTests : GradeConverterTests<BritishTechnicalGradeConverter>
{
    [Theory]
    [InlineData(2, "1a")]
    [InlineData(11, "1b")]
    [InlineData(20, "1c")]
    [InlineData(29, "2a")]
    [InlineData(38, "2b")]
    [InlineData(47, "2c")]
    [InlineData(56, "3a")]
    [InlineData(66, "3b")]
    [InlineData(75, "3c")]
    [InlineData(84, "4a")]
    [InlineData(93, "4b")]
    [InlineData(123, "4c")]
    [InlineData(154, "5a")]
    [InlineData(184, "5b")]
    [InlineData(214, "5c")]
    [InlineData(245, "6a")]
    [InlineData(275, "6b")]
    [InlineData(305, "6c")]
    [InlineData(336, "7a")]
    [InlineData(366, "7b")]
    [InlineData(396, "7c")]
    public void GradeConverter_GetGradeString_ConvertsCorrectly(int numericalGrade, string gradeString)
    {
        var outputGrade = sut.ConvertToGrade(numericalGrade);

        outputGrade.StringGrade.ShouldBeEquivalentTo(gradeString);
    }
}

