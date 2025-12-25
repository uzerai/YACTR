using Shouldly;
using YACTR.Data.Model.Climbing.Grade.Converter;

namespace YACTR.Tests.UnitTests.Grade.Converter;

public class SwedishGradeConverterTests : GradeConverterTests<SwedishGradeConverter>
{
    [Theory]
    [InlineData(2, "1-")]
    [InlineData(11, "1")]
    [InlineData(20, "1+")]
    [InlineData(29, "2-")]
    [InlineData(38, "2")]
    [InlineData(47, "2+")]
    [InlineData(56, "3-")]
    [InlineData(66, "3")]
    [InlineData(75, "3+")]
    [InlineData(84, "4-")]
    [InlineData(93, "4")]
    [InlineData(109, "4+")]
    [InlineData(126, "5-")]
    [InlineData(141, "5")]
    [InlineData(157, "5+")]
    [InlineData(171, "6-")]
    [InlineData(184, "6")]
    [InlineData(200, "6+")]
    [InlineData(216, "7-")]
    [InlineData(233, "7")]
    [InlineData(249, "7+")]
    [InlineData(266, "8-")]
    [InlineData(283, "8")]
    [InlineData(300, "8+")]
    [InlineData(317, "9-")]
    [InlineData(335, "9")]
    [InlineData(352, "9+")]
    [InlineData(369, "10-")]
    [InlineData(386, "10")]
    [InlineData(405, "10+")]
    [InlineData(422, "11-")]
    [InlineData(439, "11")]
    [InlineData(456, "11+")]
    [InlineData(464, "12-")]
    [InlineData(470, "12")]
    [InlineData(477, "12+")]
    public void GradeConverter_GetGradeString_ConvertsCorrectly(int numericalGrade, string gradeString)
    {
        var outputGrade = sut.Convert(numericalGrade);

        outputGrade.GradeString.ShouldBeEquivalentTo(gradeString);
    }
}

