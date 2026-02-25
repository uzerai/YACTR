using Shouldly;
using YACTR.Domain.Model.Climbing.Grade.GradeConverters;

namespace YACTR.Domain.Tests.Grade.Converter;

public class FrenchGradeConverterTests : GradeConverterTests<FrenchGradeConverter>
{
    [Theory]
    [InlineData(2, "1a")]
    [InlineData(7, "1a+")]
    [InlineData(12, "1b")]
    [InlineData(17, "1b+")]
    [InlineData(22, "1c")]
    [InlineData(27, "1c+")]
    [InlineData(32, "2a")]
    [InlineData(37, "2a+")]
    [InlineData(42, "2b")]
    [InlineData(47, "2b+")]
    [InlineData(53, "2c")]
    [InlineData(57, "2c+")]
    [InlineData(63, "3a")]
    [InlineData(67, "3a+")]
    [InlineData(73, "3b")]
    [InlineData(77, "3b+")]
    [InlineData(83, "3c")]
    [InlineData(87, "3c+")]
    [InlineData(93, "4a")]
    [InlineData(100, "4a+")]
    [InlineData(106, "4b")]
    [InlineData(114, "4b+")]
    [InlineData(121, "4c")]
    [InlineData(127, "4c+")]
    [InlineData(135, "5a")]
    [InlineData(142, "5a+")]
    [InlineData(149, "5b")]
    [InlineData(156, "5b+")]
    [InlineData(163, "5c")]
    [InlineData(170, "5c+")]
    [InlineData(176, "6a")]
    [InlineData(184, "6a+")]
    [InlineData(196, "6b")]
    [InlineData(210, "6b+")]
    [InlineData(223, "6c")]
    [InlineData(236, "6c+")]
    [InlineData(248, "7a")]
    [InlineData(262, "7a+")]
    [InlineData(275, "7b")]
    [InlineData(286, "7b+")]
    [InlineData(297, "7c")]
    [InlineData(309, "7c+")]
    [InlineData(320, "8a")]
    [InlineData(332, "8a+")]
    [InlineData(343, "8b")]
    [InlineData(355, "8b+")]
    [InlineData(366, "8c")]
    [InlineData(378, "8c+")]
    [InlineData(392, "9a")]
    [InlineData(405, "9a+")]
    [InlineData(417, "9b")]
    [InlineData(430, "9b+")]
    [InlineData(444, "9c")]
    public void GradeConverter_GetGradeString_ConvertsCorrectly(int numericalGrade, string gradeString)
    {
        var outputGrade = Sut.Convert(numericalGrade);

        outputGrade.GradeString.ShouldBeEquivalentTo(gradeString);
    }
}