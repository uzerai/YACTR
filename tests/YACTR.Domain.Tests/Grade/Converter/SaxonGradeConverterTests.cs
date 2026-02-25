using Shouldly;
using YACTR.Domain.Model.Climbing.Grade.GradeConverters;

namespace YACTR.Domain.Tests.Grade.Converter;

public class SaxonGradeConverterTests : GradeConverterTests<SaxonGradeConverter>
{
    [Theory]
    [InlineData(2, "I")]
    [InlineData(32, "II")]
    [InlineData(63, "III")]
    [InlineData(93, "IV")]
    [InlineData(108, "V")]
    [InlineData(123, "VI")]
    [InlineData(138, "VIIa")]
    [InlineData(154, "VIIb")]
    [InlineData(168, "VIIc")]
    [InlineData(184, "VIIIa")]
    [InlineData(199, "VIIIb")]
    [InlineData(214, "VIIIc")]
    [InlineData(229, "IXa")]
    [InlineData(245, "IXb")]
    [InlineData(259, "IXc")]
    [InlineData(275, "Xa")]
    [InlineData(290, "Xb")]
    [InlineData(305, "Xc")]
    [InlineData(320, "XIa")]
    [InlineData(336, "XIb")]
    [InlineData(350, "XIc")]
    [InlineData(366, "XIIa")]
    [InlineData(381, "XIIb")]
    [InlineData(396, "XIIc")]
    [InlineData(411, "XIIIa")]
    [InlineData(426, "XIIIb")]
    [InlineData(441, "XIIIc")]
    public void GradeConverter_GetGradeString_ConvertsCorrectly(int numericalGrade, string gradeString)
    {
        var outputGrade = sut.Convert(numericalGrade);

        outputGrade.GradeString.ShouldBeEquivalentTo(gradeString);
    }
}

