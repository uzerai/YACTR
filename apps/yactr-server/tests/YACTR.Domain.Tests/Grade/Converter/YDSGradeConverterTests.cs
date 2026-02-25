using Shouldly;
using YACTR.Domain.Model.Climbing.Grade.GradeConverters;

namespace YACTR.Domain.Tests.Grade.Converter;

public class YDSGradeConverterTests : GradeConverterTests<YDSGradeConverter>
{
    [Theory]
    [InlineData(22, "5.0")]
    [InlineData(34, "5.1")]
    [InlineData(46, "5.2")]
    [InlineData(57, "5.3")]
    [InlineData(69, "5.4")]
    [InlineData(81, "5.5")]
    [InlineData(93, "5.6")]
    [InlineData(111, "5.7")]
    [InlineData(129, "5.8")]
    [InlineData(147, "5.9")]
    [InlineData(166, "5.10a")]
    [InlineData(184, "5.10b")]
    [InlineData(196, "5.10c")]
    [InlineData(206, "5.10d")]
    [InlineData(218, "5.11a")]
    [InlineData(229, "5.11b")]
    [InlineData(241, "5.11c")]
    [InlineData(252, "5.11d")]
    [InlineData(264, "5.12a")]
    [InlineData(275, "5.12b")]
    [InlineData(286, "5.12c")]
    [InlineData(297, "5.12d")]
    [InlineData(309, "5.13a")]
    [InlineData(320, "5.13b")]
    [InlineData(332, "5.13c")]
    [InlineData(343, "5.13d")]
    [InlineData(355, "5.14a")]
    [InlineData(366, "5.14b")]
    [InlineData(378, "5.14c")]
    [InlineData(392, "5.14d")]
    [InlineData(405, "5.15a")]
    [InlineData(417, "5.15b")]
    [InlineData(430, "5.15c")]
    [InlineData(444, "5.15d")]
    public void GradeConverter_GetGradeString_ConvertsCorrectly(int numericalGrade, string gradeString)
    {
        var outputGrade = Sut.Convert(numericalGrade);

        outputGrade.GradeString.ShouldBeEquivalentTo(gradeString);
    }
}

