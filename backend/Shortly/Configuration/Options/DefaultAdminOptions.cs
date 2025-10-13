using System.ComponentModel.DataAnnotations;

namespace Shortly.Configuration.Options;

public class DefaultAdminOptions
{
    [EmailAddress, Required]
    public string Email { get; set; } = default!;

    [StringLength(maximumLength: 128, MinimumLength = 1)]
    public string Password { get; set; } = default!;
}
