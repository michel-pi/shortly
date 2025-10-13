namespace Shortly.Configuration.Options;

public class SecurityOptions
{
    public SecretDerivationOptions SecretDerivation { get; set; } = new();

    public JwtOptions Jwt { get; set; } = new();
}
