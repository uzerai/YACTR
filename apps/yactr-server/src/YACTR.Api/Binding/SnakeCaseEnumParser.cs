using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.Serialization;
using Microsoft.Extensions.Primitives;
using YACTR.Api.Pagination;

using ParseResult = FastEndpoints.ParseResult;

namespace YACTR.Api.Binding;

/// <summary>
/// Enum parser for FastEndpoints model binding that resolves values from
/// <see cref="EnumMemberAttribute"/> wire names (e.g. "created_at" -> CreatedAt), matching
/// the values exposed in the OpenAPI schema. Falls back to case-insensitive member-name
/// parsing so plain member names remain accepted.
/// </summary>
public static class SnakeCaseEnumParser
{
    private static readonly ConcurrentDictionary<Type, IReadOnlyDictionary<string, object>> WireNameCache = new();

    public static ParseResult Parse(Type enumType, StringValues input)
    {
        var value = input.ToString();
        if (string.IsNullOrWhiteSpace(value))
        {
            return new ParseResult(false, null);
        }

        var wireNames = WireNameCache.GetOrAdd(enumType, BuildWireNameMap);
        if (wireNames.TryGetValue(value, out var result))
        {
            return new ParseResult(true, result);
        }

        return Enum.TryParse(enumType, value, ignoreCase: true, out var fallback)
            ? new ParseResult(true, fallback)
            : new ParseResult(false, null);
    }

    private static IReadOnlyDictionary<string, object> BuildWireNameMap(Type enumType)
    {
        var map = new Dictionary<string, object>(StringComparer.Ordinal);
        foreach (var field in enumType.GetFields(BindingFlags.Public | BindingFlags.Static))
        {
            var wireName = field.GetCustomAttribute<EnumMemberAttribute>()?.Value ?? field.Name;
            map[wireName] = field.GetValue(null)!;
        }

        return map;
    }
}

public static class SortEnumBindingExtensions
{
    /// <summary>
    /// Registers the <see cref="SnakeCaseEnumParser"/> for <see cref="SortDirection"/> and every
    /// TSortBy enum used by a <see cref="SortedPaginationRequest{TSortBy}"/>-derived request DTO
    /// in the API assembly, so new sortable endpoints need no per-enum registration.
    /// </summary>
    public static void AddSortEnumValueParsers(this FastEndpoints.BindingOptions bindingOptions)
    {
        foreach (var enumType in DiscoverSortEnumTypes())
        {
            bindingOptions.ValueParserFor(enumType, input => SnakeCaseEnumParser.Parse(enumType, input));
        }
    }

    private static IEnumerable<Type> DiscoverSortEnumTypes()
    {
        yield return typeof(SortDirection);

        var sortByTypes = typeof(SortedPaginationRequest<>).Assembly
            .GetTypes()
            .Select(GetSortByTypeArgument)
            .OfType<Type>()
            .Distinct();

        foreach (var sortByType in sortByTypes)
        {
            yield return sortByType;
        }
    }

    private static Type? GetSortByTypeArgument(Type type)
    {
        for (var current = type.BaseType; current is not null; current = current.BaseType)
        {
            if (current.IsGenericType && current.GetGenericTypeDefinition() == typeof(SortedPaginationRequest<>))
            {
                return current.GetGenericArguments()[0];
            }
        }

        return null;
    }
}
