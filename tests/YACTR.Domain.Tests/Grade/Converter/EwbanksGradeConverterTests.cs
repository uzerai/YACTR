using Shouldly;
using YACTR.Domain.Model.Climbing.Grade.GradeConverters;

namespace YACTR.Domain.Tests.Grade.Converter;

public class EwbanksGradeConverterTests : GradeConverterTests<EwbanksGradeConverter>
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
    [InlineData(108, "14")]
    [InlineData(123, "15")]
    [InlineData(138, "16")]
    [InlineData(154, "17")]
    [InlineData(168, "18")]
    [InlineData(184, "19")]
    [InlineData(199, "20")]
    [InlineData(214, "21")]
    [InlineData(229, "22")]
    [InlineData(245, "23")]
    [InlineData(259, "24")]
    [InlineData(275, "25")]
    [InlineData(286, "26")]
    [InlineData(297, "27")]
    [InlineData(309, "28")]
    [InlineData(320, "29")]
    [InlineData(332, "30")]
    [InlineData(343, "31")]
    [InlineData(355, "32")]
    [InlineData(366, "33")]
    [InlineData(378, "34")]
    [InlineData(392, "35")]
    [InlineData(405, "36")]
    [InlineData(417, "37")]
    [InlineData(430, "38")]
    [InlineData(444, "39")]
    public void GradeConverter_GetGradeString_ConvertsCorrectly(int numericalGrade, string gradeString)
    {
        var outputGrade = sut.Convert(numericalGrade);

        outputGrade.GradeString.ShouldBeEquivalentTo(gradeString);
    }
}

