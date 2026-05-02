using System.Reflection;

using Shouldly;

using YACTR.Api.Endpoints.Root;
using YACTR.Domain.Model.Climbing;

namespace YACTR.Api.Tests;

/// <summary>
/// Ensures the API layer does not expose YACTR.Domain models directly as request or response types.
/// Endpoints must use API DTOs that map to/from domain models instead.
/// </summary>
public sealed class NoDomainDtosTest
{
    private static readonly Assembly ApiAssembly = typeof(GetIndex).Assembly;
    private static readonly Assembly DomainAssembly = typeof(Area).Assembly;

    [Fact]
    public void No_endpoint_uses_domain_models_as_request_or_response()
    {
        var domainModelTypes = GetDomainModelTypes();
        var violations = new List<(Type EndpointType, Type RequestOrResponseType, string Kind)>();

        foreach (var endpointType in GetEndpointTypes())
        {
            var (requestType, responseType) = GetRequestAndResponseTypes(endpointType);
            if (requestType is null || responseType is null)
                continue;

            foreach (var used in GetTypesUsedInContract(requestType))
            {
                if (used.IsGenericParameter || used.ContainsGenericParameters)
                    continue;
                if (domainModelTypes.Contains(used))
                    violations.Add((endpointType, used, "Request"));
            }
            foreach (var used in GetTypesUsedInContract(responseType))
            {
                if (used.IsGenericParameter || used.ContainsGenericParameters)
                    continue;
                if (domainModelTypes.Contains(used))
                    violations.Add((endpointType, used, "Response"));
            }
        }

        violations.ShouldBeEmpty(
            "Endpoints must not use YACTR.Domain models as request or response types. " +
            "Use API DTOs and mappers instead.\n" +
            "Violations:\n" +
            string.Join("\n", violations.Select(v => $"  - {v.EndpointType.Name}: {v.Kind} type {v.RequestOrResponseType.FullName}")));
    }

    private static HashSet<Type> GetDomainModelTypes()
    {
        return DomainAssembly
            .GetTypes()
            .Where(t => t.IsClass && t.Namespace?.StartsWith("YACTR.Domain") == true)
            .ToHashSet();
    }

    private static IEnumerable<Type> GetEndpointTypes()
    {
        return ApiAssembly
            .GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.BaseType is { IsGenericType: true })
            .Where(t =>
            {
                var def = t.BaseType!.GetGenericTypeDefinition();
                var name = def.Name;
                return name.StartsWith("Endpoint", StringComparison.Ordinal)
                    || name.StartsWith("AuthenticatedEndpoint", StringComparison.Ordinal);
            });
    }

    private static (Type? Request, Type? Response) GetRequestAndResponseTypes(Type endpointType)
    {
        var baseType = endpointType.BaseType;
        if (baseType?.IsGenericType != true)
            return (null, null);
        var args = baseType.GetGenericArguments();
        if (args.Length < 2)
            return (null, null);
        return (args[0], args[1]);
    }

    private static IEnumerable<Type> GetTypesUsedInContract(Type type)
    {
        if (type.IsGenericType && !type.ContainsGenericParameters)
        {
            yield return type;
            foreach (var arg in type.GetGenericArguments())
            {
                foreach (var nested in GetTypesUsedInContract(arg))
                    yield return nested;
            }
            yield break;
        }
        if (type.IsArray)
        {
            yield return type;
            foreach (var nested in GetTypesUsedInContract(type.GetElementType()!))
                yield return nested;
            yield break;
        }
        yield return type;
    }
}
