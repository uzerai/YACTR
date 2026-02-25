using Shouldly;
using YACTR.Domain.Model.Climbing.Grade.GradeConverters;

namespace YACTR.Domain.Tests.Grade.Converter;

public class PolishGradeConverterTests : GradeConverterTests<PolishGradeConverter>
{
    [Theory]
    [InlineData(2, "I-")]
    [InlineData(10, "I")]
    [InlineData(18, "I+")]
    [InlineData(26, "II-")]
    [InlineData(35, "II")]
    [InlineData(43, "II+")]
    [InlineData(52, "III-")]
    [InlineData(60, "III")]
    [InlineData(68, "III+")]
    [InlineData(76, "IV-")]
    [InlineData(85, "IV")]
    [InlineData(93, "IV+")]
    [InlineData(108, "V-")]
    [InlineData(123, "V")]
    [InlineData(138, "V+")]
    [InlineData(154, "VI-")]
    [InlineData(168, "VI")]
    [InlineData(184, "VI+")]
    [InlineData(196, "VI.1")]
    [InlineData(210, "VI.1+")]
    [InlineData(223, "VI.2")]
    [InlineData(236, "VI.2+")]
    [InlineData(248, "VI.3")]
    [InlineData(262, "VI.3+")]
    [InlineData(275, "VI.4")]
    [InlineData(290, "VI.4+")]
    [InlineData(305, "VI.5")]
    [InlineData(320, "VI.5+")]
    [InlineData(336, "VI.6")]
    [InlineData(350, "VI.6+")]
    [InlineData(366, "VI.7")]
    [InlineData(381, "VI.7+")]
    [InlineData(396, "VI.8")]
    [InlineData(411, "VI.8+")]
    [InlineData(426, "VI.9")]
    [InlineData(441, "VI.9+")]
    public void GradeConverter_GetGradeString_ConvertsCorrectly(int numericalGrade, string gradeString)
    {
        var outputGrade = Sut.Convert(numericalGrade);

        outputGrade.GradeString.ShouldBeEquivalentTo(gradeString);
    }
}

