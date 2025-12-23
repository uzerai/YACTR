using Shouldly;
using YACTR.Data.Model.Climbing.Grade.Converter;

namespace YACTR.Tests.UnitTests.Grade.Converter;

public class BrazilTechnicalGradeConverterTests : GradeConverterTests<BrazilTechnicalGradeConverter>
{
    [Theory]
    [InlineData(2, "I")]
    [InlineData(20, "Isup")]
    [InlineData(38, "II")]
    [InlineData(56, "IIsup")]
    [InlineData(75, "III")]
    [InlineData(93, "IIIsup")]
    [InlineData(116, "IV")]
    [InlineData(138, "IVsup")]
    [InlineData(161, "V")]
    [InlineData(184, "Vsup")]
    [InlineData(196, "VI")]
    [InlineData(210, "VIsup")]
    [InlineData(223, "VIIa")]
    [InlineData(236, "VIIb")]
    [InlineData(249, "VIIc")]
    [InlineData(262, "VIIIa")]
    [InlineData(275, "VIIIb")]
    [InlineData(286, "VIIIc")]
    [InlineData(297, "IXa")]
    [InlineData(309, "IXb")]
    [InlineData(320, "IXc")]
    [InlineData(332, "Xa")]
    [InlineData(343, "Xb")]
    [InlineData(355, "Xc")]
    [InlineData(366, "XIa")]
    [InlineData(378, "XIb")]
    [InlineData(392, "XIc")]
    [InlineData(405, "XIIa")]
    [InlineData(417, "XIIb")]
    [InlineData(431, "XIIc")]
    public void GradeConverter_GetGradeString_ConvertsCorrectly(int numericalGrade, string gradeString)
    {
        var outputGrade = sut.ConvertToGrade(numericalGrade);

        outputGrade.StringGrade.ShouldBeEquivalentTo(gradeString);
    }
}

