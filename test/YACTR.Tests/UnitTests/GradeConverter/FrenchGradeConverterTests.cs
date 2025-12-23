using Shouldly;
using YACTR.Data.Model.Climbing.Grade;

namespace YACTR.Tests.UnitTests.GradeConverter;

public class FrenchGradeConverterTests : GradeConverterTests<FrenchGradeConverter>
{
    [Theory]
    [InlineData(1, "1a")]
    public void GradeConverter_GetGradeString_ConvertsCorrectly(int numericalGrade, string gradeString)
    {
        var outputGrade = sut.ConvertToGrade(numericalGrade);

        outputGrade.StringGrade.ShouldBeEquivalentTo(gradeString);
    }
}