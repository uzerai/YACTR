using Shouldly;
using YACTR.Infrastructure.FileFormatExtensions;

namespace YACTR.Infrastructure.Tests.FileFormatExtensions;

public class SvgTests
{
    [Fact]
    public void IsMatch_ShouldReturnTrue_WhenRootElementIsSvg()
    {
        var sut = new Svg();
        using var stream = new MemoryStream("""<svg xmlns="http://www.w3.org/2000/svg"></svg>"""u8.ToArray());

        var result = sut.IsMatch(stream);

        result.ShouldBeTrue();
    }

    [Fact]
    public void IsMatch_ShouldReturnFalse_WhenXmlRootIsNotSvg()
    {
        var sut = new Svg();
        using var stream = new MemoryStream("""<root></root>"""u8.ToArray());

        var result = sut.IsMatch(stream);

        result.ShouldBeFalse();
    }

    [Fact]
    public void IsMatch_ShouldReturnFalse_WhenContentIsNotValidXml()
    {
        var sut = new Svg();
        using var stream = new MemoryStream([0xFF, 0xD8, 0xFF, 0xE0, 0x10, 0x20, 0x30, 0x40]);

        var result = sut.IsMatch(stream);

        result.ShouldBeFalse();
    }
}
