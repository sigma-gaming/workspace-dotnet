using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace Common.OpenApi.Transformers;

public class EnumSchemaTransformer : IOpenApiSchemaTransformer
{
    public Task TransformAsync(OpenApiSchema schema, OpenApiSchemaTransformerContext context,
        CancellationToken cancellationToken)
    {
        if (schema.Enum.Count > 0) schema.Type = "string";

        return Task.CompletedTask;
    }
}