using Shouldly;
using YACTR.Data.Model.Climbing.Grade.Converter;

namespace YACTR.Tests.UnitTests.Grade.Converter;

public class SouthAfricanGradeConverterTests : GradeConverterTests<SouthAfricanGradeConverter>
{
    [Theory]
    [InlineData(2, "1")]
    [InlineData(9, "2")]    
    [InlineData(17, "3")]
    [InlineData(25, "4")]
    [InlineData(32, "5")]
    [InlineData(40, "6")]
    [InlineData(47, "7")]
    [InlineData(55, "8")]
    [InlineData(63, "9")]
    [InlineData(70, "10")]
    [InlineData(77, "11")]
    [InlineData(86, "12")]
    [InlineData(93, "13")]
    [InlineData(106, "14")]
    [InlineData(119, "15")]
    [InlineData(132, "16")]
    [InlineData(145, "17")]
    [InlineData(157, "18")]
    [InlineData(171, "19")]
    [InlineData(184, "20")]
    [InlineData(196, "21")]
    [InlineData(210, "22")]
    [InlineData(223, "23")]
    [InlineData(236, "24")]
    [InlineData(248, "25")]
    [InlineData(262, "26")]
    [InlineData(275, "27")]
    [InlineData(286, "28")]
    [InlineData(297, "29")]
    [InlineData(309, "30")]
    [InlineData(320, "31")]
    [InlineData(332, "32")]
    [InlineData(343, "33")]
    [InlineData(355, "34")]
    [InlineData(366, "35")]
    [InlineData(378, "36")]
    [InlineData(392, "37")]
    [InlineData(405, "38")]
    [InlineData(417, "39")]
    [InlineData(430, "40")]
    public void GradeConverter_GetGradeString_ConvertsCorrectly(int numericalGrade, string gradeString)
    {
        var outputGrade = sut.ConvertToGrade(numericalGrade);

        outputGrade.StringGrade.ShouldBeEquivalentTo(gradeString);
    }
}

