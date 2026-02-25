using System.Xml;
using FileSignatures.Formats;

namespace YACTR.Infrastructure.FileFormatExtensions;

/// <summary>
/// Representation of a fileformat for the checking of an SVG file.
/// </summary>
///
/// <remarks>
/// <para>
/// The primary use case is for compatibility with the ImageFormat interface, which is
/// used by the FileFormatInspector to determine the type of an uploaded file.
/// </para>
///
/// <see cref="FileSignatures.Formats.ImageFormat" /><br/>
/// <see cref="FileSignatures.FileFormatInspector" /><br/>
///
/// <seealso href="https://github.com/neilharvey/FileSignatures">
/// FileSignatures github project.
/// </seealso>
/// </remarks>
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