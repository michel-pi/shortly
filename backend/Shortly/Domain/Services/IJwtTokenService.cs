using System.Collections.Generic;

using Shortly.Domain.Identity;

namespace Shortly.Domain.Services;

public interface IJwtTokenService
{
    string CreateAccessToken(
        AppUser user,
        IEnumerable<string>? roles = null);
}
