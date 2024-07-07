using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace UsersJwtAuth.StartupDependency;

public static class DiExtensions
{
    public static void AddAuthServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddAuthorization(options =>
        {
            options.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
        });

        services.AddAuthentication("Bearer").AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = config.GetValue<string>("Auth:Issuer"),
                ValidAudience = config.GetValue<string>("Auth:Audience"),
                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(config.GetValue<string>("Auth:SecretKey")))
            };
        });
    }

    public static void AddSwaggerServices(this IServiceCollection services)
    {
        var securityScheme = new OpenApiSecurityScheme()
        {
            Name = "Authorization",
            Description = "JWT Authorization using bearer token",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT"
        };

        var securityRequirement = new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "bearerAuth"
                    }
                },
                Array.Empty<string>()
            }
        };

        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("bearerAuth", securityScheme);
            options.AddSecurityRequirement(securityRequirement);
        });
    }
}
