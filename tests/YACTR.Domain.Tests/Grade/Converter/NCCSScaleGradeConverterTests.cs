using Shouldly;
using YACTR.Domain.Model.Climbing.Grade.GradeConverters;

namespace YACTR.Domain.Tests.Grade.Converter;

public class NCCSScaleGradeConverterTests : GradeConverterTests<NCCSScaleGradeConverter>
{
    [Theory]
    [InlineData(2, "F4")]
    [InlineData(47, "F5")]
    [InlineData(93, "F6")]
    [InlineData(116, "F7")]
    [InlineData(138, "F8")]
    [InlineData(161, "F9")]
    [InlineData(184, "F10")]
    [InlineData(202, "F11")]
    [InlineData(220, "F12")]
    [InlineData(238, "F13")]
    [InlineData(256, "F14")]
    [InlineData(275, "F15")]
    [InlineData(297, "F16")]
    public void GradeConverter_GetGradeString_ConvertsCorrectly(int numericalGrade, string gradeString)
    {
        var outputGrade = sut.Convert(numericalGrade);

        outputGrade.GradeString.ShouldBeEquivalentTo(gradeString);
    }
}

