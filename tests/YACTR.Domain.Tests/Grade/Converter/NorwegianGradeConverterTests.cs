using Shouldly;
using YACTR.Domain.Model.Climbing.Grade.GradeConverters;

namespace YACTR.Domain.Tests.Grade.Converter;

public class NorwegianGradeConverterTests : GradeConverterTests<NorwegianGradeConverter>
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
    [InlineData(205, "6+")]
    [InlineData(220, "7-")]
    [InlineData(236, "7")]
    [InlineData(251, "7+")]
    [InlineData(266, "8-")]
    [InlineData(282, "8")]
    [InlineData(302, "8+")]
    [InlineData(321, "9-")]
    [InlineData(341, "9")]
    [InlineData(360, "9+")]
    [InlineData(376, "10-")]
    [InlineData(392, "10")]
    [InlineData(408, "10+")]
    [InlineData(425, "11-")]
    [InlineData(440, "11")]
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

