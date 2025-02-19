using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Common.OpenApi.DocumentFilters;

public class FlattenAllOfRefDocumentFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument document, DocumentFilterContext context)
    {
        if (document?.Components?.Schemas == null) return;

        foreach (var kvp in document.Components.Schemas) FlattenAllOf(kvp.Value);
    }

    private void FlattenAllOf(OpenApiSchema schema)
    {
        if (schema.Properties != null)
            foreach (var propertySchema in schema.Properties.Values)
                FlattenAllOf(propertySchema);

        if (schema.Items != null) FlattenAllOf(schema.Items);

        if (schema.AdditionalProperties != null) FlattenAllOf(schema.AdditionalProperties);

        if (schema.AllOf != null && schema.AllOf.Count == 1)
        {
            var onlyAllOfItem = schema.AllOf[0];
            if (onlyAllOfItem.Reference != null && IsEmptySchema(schema))
            {
                schema.Reference = onlyAllOfItem.Reference;
                schema.AllOf = null;
            }
        }
    }

    private bool IsEmptySchema(OpenApiSchema schema)
    {
        if (schema.Reference != null) return false;

        if (schema.Properties?.Count > 0) return false;
        if (schema.Type != null) return false;
        if (schema.Format != null) return false;
        if (schema.Nullable) return false;
        if (schema.Enum?.Count > 0) return false;
        if (schema.OneOf?.Count > 0) return false;
        if (schema.AnyOf?.Count > 0) return false;
        if (schema.Discriminator != null) return false;
        if (schema.Required?.Count > 0) return false;
        if (schema.AdditionalProperties != null) return false;
        if (schema.Items != null) return false;

        return true;
    }
}