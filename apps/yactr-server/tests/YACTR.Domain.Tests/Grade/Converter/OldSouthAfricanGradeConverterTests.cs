using Shouldly;
using YACTR.Domain.Model.Climbing.Grade.GradeConverters;

namespace YACTR.Domain.Tests.Grade.Converter;

public class OldSouthAfricanGradeConverterTests : GradeConverterTests<OldSouthAfricanGradeConverter>
{
    [Theory]
    [InlineData(2, "A1")]
    [InlineData(7, "A2")]
    [InlineData(14, "A3")]
    [InlineData(19, "B1")]
    [InlineData(25, "B2")]
    [InlineData(30, "B3")]
    [InlineData(36, "C1")]
    [InlineData(42, "C2")]
    [InlineData(47, "C3")]
    [InlineData(53, "D1")]
    [InlineData(59, "D2")]
    [InlineData(65, "D3")]
    [InlineData(70, "E1")]
    [InlineData(76, "E2")]
    [InlineData(82, "E3")]
    [InlineData(87, "F1")]
    [InlineData(93, "F2")]
    [InlineData(116, "F3")]
    [InlineData(138, "G1")]
    [InlineData(161, "G2")]
    [InlineData(184, "G3")]
    [InlineData(206, "H1")]
    [InlineData(229, "H2")]
    [InlineData(252, "H3")]
    [InlineData(275, "I1")]
    [InlineData(297, "I2")]
    [InlineData(320, "I3")]
    [InlineData(343, "J1")]
    [InlineData(366, "J2")]
    [InlineData(379, "J3")]
    public void GradeConverter_GetGradeString_ConvertsCorrectly(int numericalGrade, string gradeString)
    {
        var outputGrade = sut.Convert(numericalGrade);

        outputGrade.GradeString.ShouldBeEquivalentTo(gradeString);
    }
}

