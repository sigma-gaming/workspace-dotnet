using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Common.OpenApi.DocumentFilters;

public class DiscriminatedUnionDocumentFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument document, DocumentFilterContext context)
    {
        if (document.Components.Schemas == null)
            return;

        var schemas = document.Components.Schemas;

        var baseToDerivedMap = new Dictionary<string, HashSet<string>>();

        foreach (var (schemaName, schema) in schemas)
        {
            if (IsReferenceSchema(schema))
                continue;

            if (schema.Properties == null || schema.Required == null) continue;

            var readOnlyProp = schema.Properties.FirstOrDefault(p => p.Value.ReadOnly);

            if (readOnlyProp.Key == null) continue;

            if (schema.Required.Contains(readOnlyProp.Key))
                baseToDerivedMap[schemaName] = [];
        }

        foreach (var (schemaName, schema) in schemas)
        {
            if (IsReferenceSchema(schema))
                continue;

            if (schema.AllOf != null)
                foreach (var allOfItem in schema.AllOf)
                    if (allOfItem.Reference != null)
                    {
                        var baseName = allOfItem.Reference.Id;
                        if (baseToDerivedMap.TryGetValue(baseName, out var derivedSet)) derivedSet.Add(schemaName);
                    }
        }

        foreach (var kvp in baseToDerivedMap)
        {
            var baseName = kvp.Key;

            // Exclude relations with chain longer than 1 (false positives)
            if (baseToDerivedMap.SelectMany(p => p.Value).Contains(baseName))
                continue;

            var derivedNames = kvp.Value;
            if (derivedNames.Count == 0) continue;

            if (!schemas.TryGetValue(baseName, out var baseSchema))
                continue;

            var discriminatorField = GuessDiscriminatorField(baseSchema);
            if (discriminatorField == null)
                continue;

            baseSchema.Type = null;
            baseSchema.AllOf = null;
            baseSchema.Properties = null;
            baseSchema.Required = null;

            baseSchema.OneOf = new List<OpenApiSchema>();
            baseSchema.Discriminator = new OpenApiDiscriminator
            {
                PropertyName = discriminatorField,
                Mapping = new Dictionary<string, string>(),
            };

            foreach (var derivedName in derivedNames)
            {
                if (!schemas.TryGetValue(derivedName, out var derivedSchema))
                    continue;

                derivedSchema.AllOf = null;

                var discValue = derivedName.Replace(baseName, "");
                if (string.IsNullOrWhiteSpace(discValue))
                    discValue = "Unknown";

                derivedSchema.Properties ??= new Dictionary<string, OpenApiSchema>();

                derivedSchema.Properties[discriminatorField] = new OpenApiSchema
                {
                    Type = "string",
                    Enum = new List<IOpenApiAny> { new OpenApiString(discValue) },
                    ReadOnly = true,
                };

                derivedSchema.Required ??= new HashSet<string>();
                derivedSchema.Required.Add(discriminatorField);

                baseSchema.OneOf.Add(
                    new OpenApiSchema
                    {
                        Reference = new OpenApiReference
                        {
                            Id = derivedName,
                            Type = ReferenceType.Schema,
                        },
                    }
                );

                baseSchema.Discriminator.Mapping[discValue] = $"#/components/schemas/{derivedName}";
            }
        }
    }

    private bool IsReferenceSchema(OpenApiSchema schema)
    {
        return schema.Reference != null;
    }

    private string? GuessDiscriminatorField(OpenApiSchema schema)
    {
        if (schema is not { Properties: not null, Required: not null })
            return null;

        foreach (var propName in schema.Required)
        {
            if (!schema.Properties.TryGetValue(propName, out var propSchema))
                continue;

            if (propSchema.ReadOnly)
                return propName;
        }

        return null;
    }
}