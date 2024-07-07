using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

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
}
