using System.ComponentModel.DataAnnotations;

namespace Shortly.Configuration.Options;

public class JwtOptions
{
    [StringLength(maximumLength: 1024, MinimumLength = 1)]
    public string Issuer { get; set; } = "shortly";

    [StringLength(maximumLength: 1024, MinimumLength = 1)]
    public string Audience { get; set; } = "shortly-web";

    [StringLength(maximumLength: 1024, MinimumLength = 1)]
    public string SigningKeyPassphrase { get; set; } = default!;

    [Range(1, 60)]
    public int ClockSkewMinutes { get; set; } = 1;

    [Range(1, 7 * 24 * 60)]
    public int AccessTokenMinutes { get; set; } = 60;

    [Range(1, 4 * 7)]
    public int RefreshTokenDays { get; set; } = 7;
}
