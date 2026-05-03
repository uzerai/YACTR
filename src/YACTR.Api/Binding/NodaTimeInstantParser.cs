using NodaTime.Text;
using Microsoft.Extensions.Primitives;

using ParseResult = FastEndpoints.ParseResult;

namespace YACTR.Api.Binding;

/// <summary>
/// Instant parser to allow fastendpoints to correctly translate query parameters from
/// string ($datetime format, as exposed by the OpenApi schema) to Instant at model binding
/// time.
/// </summary>
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