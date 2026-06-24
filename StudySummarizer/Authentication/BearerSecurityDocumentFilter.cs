using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace StudySummarizer.API.Authentication;

public class BearerSecurityDocumentFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        if (swaggerDoc.Components?.SecuritySchemes?.ContainsKey("Bearer") != true) return;

        var schemeRef = new OpenApiSecuritySchemeReference("Bearer", swaggerDoc);

        foreach (var apiDescription in context.ApiDescriptions)
        {
            var hasAuthorize = apiDescription.ActionDescriptor.EndpointMetadata
                .OfType<AuthorizeAttribute>().Any();

            if (!hasAuthorize) continue;

            var path = "/" + apiDescription.RelativePath?.TrimEnd('/');
            if (!swaggerDoc.Paths.TryGetValue(path, out var pathItem)) continue;

            var method = new System.Net.Http.HttpMethod(apiDescription.HttpMethod!);
            if (!pathItem.Operations.TryGetValue(method, out var operation)) continue;

            operation.Security = [new OpenApiSecurityRequirement
            {
                { schemeRef, new List<string>() }
            }];
        }
    }
}
