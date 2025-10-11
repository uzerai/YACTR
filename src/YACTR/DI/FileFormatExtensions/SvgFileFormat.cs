using System.Xml;
using FileSignatures.Formats;

namespace YACTR.DI.FileFormatExtensions;

public class Svg : Image
{
    public Svg() : base([], "image/svg+xml", "svg", 0)
    { }

    public override bool IsMatch(Stream stream)
    {
        try
        {
            using var xmlReader = XmlReader.Create(stream);
            return xmlReader.MoveToContent() == XmlNodeType.Element && "svg".Equals(xmlReader.Name, StringComparison.OrdinalIgnoreCase);
        }
        catch
        {
            return false;
        }
    }
}