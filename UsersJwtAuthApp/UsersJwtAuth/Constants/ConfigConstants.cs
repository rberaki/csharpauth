namespace UsersJwtAuth.Constants;

public static class ConfigConstants
{
    public const string DbConnectionString = "DefaultConnection";

    public const string Issuer = "Auth:Issuer";
    public const string Audience = "Auth:Audience";
    public const string SecretKey = "Auth:SecretKey";
    public const string Algorithm = "Auth:Algorithm";
    public const string TokenExpirationMinutes = "Auth:TokenExpireMinutes";
}
