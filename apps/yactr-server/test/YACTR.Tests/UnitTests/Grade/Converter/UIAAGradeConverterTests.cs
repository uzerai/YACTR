using Shouldly;
using YACTR.Data.Model.Climbing.Grade.Converter;

namespace YACTR.Tests.UnitTests.Grade.Converter;

public class UIAAGradeConverterTests : GradeConverterTests<UIAAGradeConverter>
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
    [InlineData(106, "5-")]
    [InlineData(119, "5")]
    [InlineData(132, "5+")]
    [InlineData(145, "6-")]
    [InlineData(157, "6")]
    [InlineData(171, "6+")]
    [InlineData(184, "7-")]
    [InlineData(200, "7")]
    [InlineData(216, "7+")]
    [InlineData(233, "8-")]
    [InlineData(249, "8")]
    [InlineData(266, "8+")]
    [InlineData(282, "9-")]
    [InlineData(296, "9")]
    [InlineData(310, "9+")]
    [InlineData(324, "10-")]
    [InlineData(337, "10")]
    [InlineData(352, "10+")]
    [InlineData(366, "11-")]
    [InlineData(381, "11")]
    [InlineData(396, "11+")]
    [InlineData(411, "12-")]
    [InlineData(426, "12")]
    [InlineData(441, "12+")]
    public void GradeConverter_GetGradeString_ConvertsCorrectly(int numericalGrade, string gradeString)
    {
        var outputGrade = sut.Convert(numericalGrade);

        outputGrade.GradeString.ShouldBeEquivalentTo(gradeString);
    }
}

