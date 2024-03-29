using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using SmallClientBusiness.Common.Interfaces;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SmallClientBusiness.BL.Swagger;

public class AuthOperationFilter: IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var authAttributes = context.MethodInfo
            .GetCustomAttributes(true)
            .OfType<AuthorizeAttribute>()
            .Distinct();
         
        if (authAttributes.Any())
        {
            operation.Responses.TryAdd("401", new OpenApiResponse { Description = "Unauthorized" });
            operation.Responses.TryAdd("403", new OpenApiResponse { Description = "Forbidden" });
         
            var jwtBearerScheme = new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme, 
                    Id = "Bearer"
                },
            };
         
            operation.Security = new List<OpenApiSecurityRequirement>
            {
                new OpenApiSecurityRequirement
                {
                    [ jwtBearerScheme ] = new List<string>()
                }
            };
        }
    }
}