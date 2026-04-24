using Microsoft.OpenApi;
//using Microsoft.OpenApi.Models;

namespace Web.Api.Extensions;

internal static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddSwaggerGenWithAuth(this IServiceCollection services)
    {
        services.AddSwaggerGen(static o =>
        {
            o.CustomSchemaIds(id => id.FullName!.Replace('+', '-'));

            // Temporarily commented out
            //var securityScheme = new OpenApiSecurityScheme
            //{
            //    Name = "JWT Authentication",
            //    Description = "Enter your JWT token in this field",
            //    In = ParameterLocation.Header,
            //    Type = SecuritySchemeType.Http,
            //    Scheme = JwtBearerDefaults.AuthenticationScheme,
            //    BearerFormat = "JWT"
            //};

            //o.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, securityScheme);

            o.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
            {
                Name = "JWT Authentication",
                Description = "JWT Authorization header using the Bearer scheme.",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
            });

            //var securityRequirement = new OpenApiSecurityRequirement
            //{
            //    {
            //        new OpenApiSecurityScheme
            //        {
            //            Reference = new OpenApiReference
            //            {
            //                Type = ReferenceType.SecurityScheme,
            //                Id = JwtBearerDefaults.AuthenticationScheme
            //            }
            //        },
            //        []
            //    }
            //};

            //o.AddSecurityRequirement(securityRequirement);

            o.AddSecurityRequirement(document => new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("bearer", document)] = []
            });
        });

        return services;
    }
}
