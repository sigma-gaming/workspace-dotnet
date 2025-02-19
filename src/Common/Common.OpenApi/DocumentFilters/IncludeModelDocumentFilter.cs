using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Common.OpenApi.DocumentFilters;

public class IncludeModelDocumentFilter<T> : IDocumentFilter
{
    public void Apply(OpenApiDocument document, DocumentFilterContext context)
    {
        context.SchemaGenerator.GenerateSchema(typeof(T), context.SchemaRepository);
    }
}