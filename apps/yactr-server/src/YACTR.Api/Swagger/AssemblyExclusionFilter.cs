// using System.Reflection;
// using NJsonSchema.Generation;
// using NSwag.Generation.Processors;
// using NSwag.Generation.Processors.Contexts;

// namespace YACTR.Api.Swagger;
// TODO: Enable this after fixing the references to the domain models in the API.
// /// <summary>
// /// Configures Swagger/OpenAPI document generation so that only request and response models
// /// defined in the YACTR.Api project are exposed in the schema. Domain and other non-API
// /// types are excluded from the document.
// /// </summary>
// public static class AssemblyExclusionFilter
// {
//     /// <summary>
//     /// Returns the set of type full names to exclude from schema generation,
//     /// i.e. all types from the excluded assemblies (e.g. YACTR.Domain).
//     /// </summary>
//     public static IReadOnlyCollection<string> GetExcludedTypeNames()
//     {
//         var swaggerExcludedTypes = new HashSet<string>(StringComparer.Ordinal);

//         foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
//         {
//             if (!IsExcludedYactrAssembly(assembly))
//                 continue;

//             foreach (var type in assembly.ExportedTypes)
//             {
//                 if (type.FullName is null)
//                     continue;
//                 if (type.IsClass || type.IsValueType)
//                     swaggerExcludedTypes.Add(type.FullName);
//             }
//         }

//         return swaggerExcludedTypes;
//     }

//     /// <summary>
//     /// Returns schema definition keys to remove from the document (short type names when
//     /// ShortSchemaNames is true). Only includes keys that are not used by any type in
//     /// the YACTR.Api assembly, so we never remove an API type's schema.
//     /// </summary>
//     public static IReadOnlyCollection<string> GetExcludedSchemaKeys()
//     {
//         var apiShortNames = new HashSet<string>(StringComparer.Ordinal);
//         var excludedShortNames = new HashSet<string>(StringComparer.Ordinal);

//         foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
//         {
//             if (assembly.GetName().FullName?.StartsWith("YACTR.Api") == true)
//             {
//                 CollectShortNames(assembly, apiShortNames);
//                 continue;
//             }
//             if (IsExcludedYactrAssembly(assembly))
//                 CollectShortNames(assembly, excludedShortNames);
//         }

//         excludedShortNames.ExceptWith(apiShortNames);
//         return excludedShortNames;
//     }

//     /// <summary>
//     /// Applies the exclusion list to the schema generator settings so that types from
//     /// non-API assemblies are not included in the OpenAPI document.
//     /// </summary>
//     public static void ApplyAssemblyExclusionFilter(this JsonSchemaGeneratorSettings schemaSettings)
//     {
//         var excluded = GetExcludedTypeNames();
//         if (excluded.Count == 0)
//             return;

//         var existing = schemaSettings.ExcludedTypeNames ?? [];
//         schemaSettings.ExcludedTypeNames = [.. existing.Concat(excluded).Distinct()];
//     }

//     /// <summary>
//     /// Adds a document processor that removes schema definitions for non-API types from
//     /// the generated document. Use when ExcludedTypeNames alone does not prevent those
//     /// types from appearing (e.g. when they are referenced by the generator).
//     /// </summary>
//     public static void AddAssemblyExclusionDocumentProcessor(this NSwag.Generation.OpenApiDocumentGeneratorSettings documentSettings)
//     {
//         documentSettings.DocumentProcessors.Add(new RemoveNonApiSchemasDocumentProcessor());
//     }

//     private static bool IsExcludedYactrAssembly(Assembly assembly)
//     {
//         var fullName = assembly.GetName().FullName;
//         if (fullName?.StartsWith("YACTR", StringComparison.Ordinal) != true)
//             return false;
//         if (fullName.StartsWith("YACTR.Api", StringComparison.Ordinal))
//             return false;
//         return true;
//     }

//     private static void CollectShortNames(Assembly assembly, HashSet<string> target)
//     {
//         try
//         {
//             foreach (var type in assembly.ExportedTypes)
//             {
//                 if (type.IsClass || type.IsValueType)
//                     target.Add(type.Name);
//             }
//         }
//         catch (ReflectionTypeLoadException) { /* skip */ }
//     }

//     private sealed class RemoveNonApiSchemasDocumentProcessor : IDocumentProcessor
//     {
//         public void Process(DocumentProcessorContext context)
//         {
//             var keysToRemove = AssemblyExclusionFilter.GetExcludedSchemaKeys();
//             if (keysToRemove.Count == 0)
//                 return;

//             RemoveDefinitions(context.Document.Definitions, keysToRemove);
//             if (context.Document.Components?.Schemas is { } schemas)
//                 RemoveDefinitions(schemas, keysToRemove);
//         }

//         private static void RemoveDefinitions(IDictionary<string, NJsonSchema.JsonSchema>? definitions, IReadOnlyCollection<string> keysToRemove)
//         {
//             if (definitions is null)
//                 return;
//             foreach (var key in keysToRemove)
//                 definitions.Remove(key);
//         }
//     }
// }
