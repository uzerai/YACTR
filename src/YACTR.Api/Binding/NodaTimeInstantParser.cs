using NodaTime.Text;
using Microsoft.Extensions.Primitives;

using ParseResult = FastEndpoints.ParseResult;

namespace YACTR.Api.Binding;

public static class NodaTimeInstantParser
{
    public static ParseResult ParseResult(StringValues input)
    {
        var value = input.ToString();
        if (string.IsNullOrWhiteSpace(value))
        {
            return new ParseResult(false, null);
        }

        try
        {
            var result = InstantPattern.ExtendedIso.Parse(value);
            return new ParseResult(result.Success, result.Value);
        }
        catch
        {
            return new ParseResult(false, null);
        }
    }
}