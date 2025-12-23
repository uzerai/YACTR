using Shouldly;
using YACTR.Data.Model.Climbing.Grade;

namespace YACTR.Tests.UnitTests.GradeConverter;

public class FrenchGradeConverterTests : GradeConverterTests<FrenchGradeConverter>
{
    [Theory]
    [InlineData(1, "1a")]
    [InlineData(9, "1a+")]
    [InlineData(11, "1b")]
    [InlineData(16, "1b+")]
    [InlineData(21, "1c")]
    [InlineData(26, "1c+")]
    [InlineData(31, "2a")]
    [InlineData(36, "2a+")]
    [InlineData(41, "2b")]
    [InlineData(46, "2b+")]
    [InlineData(51, "2c")]
    [InlineData(56, "2c+")]
    [InlineData(61, "3a")]
    [InlineData(66, "3a+")]
    [InlineData(71, "3b")]
    [InlineData(76, "3b+")]
    public void GradeConverter_GetGradeString_ConvertsCorrectly(int numericalGrade, string gradeString)
    {
        var outputGrade = sut.ConvertToGrade(numericalGrade);

        outputGrade.StringGrade.ShouldBeEquivalentTo(gradeString);
    }
}