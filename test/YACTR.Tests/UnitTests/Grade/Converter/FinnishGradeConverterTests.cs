using Shouldly;
using YACTR.Data.Model.Climbing.Grade.Converter;

namespace YACTR.Tests.UnitTests.Grade.Converter;

public class FinnishGradeConverterTests : GradeConverterTests<FinnishGradeConverter>
{
    [Theory]
    [InlineData(2, "1-")]
    [InlineData(10, "1")]
    [InlineData(18, "1+")]
    [InlineData(26, "2-")]
    [InlineData(35, "2")]
    [InlineData(43, "2+")]
    [InlineData(52, "3-")]
    [InlineData(60, "3")]
    [InlineData(68, "3+")]
    [InlineData(76, "4-")]
    [InlineData(85, "4")]
    [InlineData(93, "4+")]
    [InlineData(107, "5-")]
    [InlineData(123, "5")]
    [InlineData(137, "5+")]
    [InlineData(164, "6-")]
    [InlineData(189, "6")]
    [InlineData(207, "6+")]
    [InlineData(226, "7-")]
    [InlineData(245, "7")]
    [InlineData(264, "7+")]
    [InlineData(282, "8-")]
    [InlineData(291, "8")]
    [InlineData(300, "8+")]
    [InlineData(310, "9-")]
    [InlineData(319, "9")]
    [InlineData(328, "9+")]
    [InlineData(337, "10-")]
    [InlineData(347, "10")]
    [InlineData(356, "10+")]
    [InlineData(366, "11-")]
    [InlineData(381, "11")]
    [InlineData(396, "11+")]
    [InlineData(411, "12-")]
    [InlineData(426, "12")]
    [InlineData(441, "12+")]
    public void GradeConverter_GetGradeString_ConvertsCorrectly(int numericalGrade, string gradeString)
    {
        var outputGrade = sut.ConvertToGrade(numericalGrade);

        outputGrade.StringGrade.ShouldBeEquivalentTo(gradeString);
    }
}

