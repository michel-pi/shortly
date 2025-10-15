using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Shortly.Domain.Identity;
using Shortly.Domain.Services;
using Shortly.Features.Auth.Dto;

namespace Shortly.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private const string c_CookiePath = "/api";
    private const string c_RefreshTokenCookieName = "rt";

    private readonly ILogger<AuthController> _logger;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly UserManager<AppUser> _userManager;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IRefreshTokenService _refreshTokenService;

    public AuthController(
        ILogger<AuthController> logger,
        SignInManager<AppUser> signInManager,
        UserManager<AppUser> userManager,
        IJwtTokenService jwtTokenService,
        IRefreshTokenService refreshTokenService)
    {
        _logger = logger;
        _signInManager = signInManager;
        _userManager = userManager;
        _jwtTokenService = jwtTokenService;
        _refreshTokenService = refreshTokenService;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(
        [FromBody] LoginRequest request,
        CancellationToken ct = default)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null)
        {
            return Unauthorized();
        }

        var passwordCheck = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: false);

        if (!passwordCheck.Succeeded)
        {
            return Unauthorized();
        }

        var roles = await _userManager.GetRolesAsync(user);

        var jwt = _jwtTokenService.CreateAccessToken(user, roles);
        var (refreshToken, expires) = await _refreshTokenService.IssueAsync(user, ct);

        SetRefreshTokenCookie(refreshToken, expires);

        return Ok(new LoginResponse
        {
            AccessToken = jwt,
            RefreshToken = refreshToken
        });
    }

    [AllowAnonymous]
    [HttpPost("refresh")]
    public async Task<ActionResult<LoginResponse>> Refresh(CancellationToken ct = default)
    {
        if (!TryGetRefreshTokenCookie(out var tokenValue))
        {
            return Unauthorized();
        }

        var refreshToken = await _refreshTokenService.GetRefreshTokenAsync(tokenValue, ct);
        var user = refreshToken.AppUserNavigation!;
        var roles = await _userManager.GetRolesAsync(user);

        var jwt = _jwtTokenService.CreateAccessToken(user, roles);

        var (newRefreshToken, newExpirationDate) = await _refreshTokenService.RotateAsync(tokenValue, ct);

        SetRefreshTokenCookie(newRefreshToken, newExpirationDate);

        return Ok(new LoginResponse
        {
            AccessToken = jwt,
            RefreshToken = newRefreshToken
        });
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(CancellationToken ct = default)
    {
        if (TryGetRefreshTokenCookie(out var refreshToken))
        {
            await _refreshTokenService.RevokeAsync(refreshToken, ct);
        }

        RemoveRefreshTokenCookie();

        return NoContent();
    }

    [Authorize]
    [HttpPost("logout-all")]
    public async Task<IActionResult> LogoutAll(CancellationToken ct = default)
    {
        if (long.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
        {
            await _refreshTokenService.RevokeAllAsync(userId, ct);
        }

        RemoveRefreshTokenCookie();

        return NoContent();
    }

    [Authorize]
    [HttpGet("user")]
    public async Task<ActionResult<UserResponse>> GetUser()
    {
        if (!long.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
        {
            return Unauthorized();
        }

        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user == null)
        {
            return Unauthorized();
        }

        var roles = await _userManager.GetRolesAsync(user);

        return Ok(new UserResponse
        {
            Email = user.Email ?? string.Empty,
            Id = user.Id,
            Name = user.UserName ?? user.Email ?? user.Id.ToString(),
            Roles = [.. roles]
        });
    }

    private bool TryGetRefreshTokenCookie(out Guid refreshToken)
    {
        refreshToken = default;
        return Request.Cookies.TryGetValue(c_RefreshTokenCookieName, out var cookieValue)
            && Guid.TryParse(cookieValue, out refreshToken);
    }

    private void SetRefreshTokenCookie(Guid refreshToken, DateTimeOffset? expires)
    {
        Response.Cookies.Append(c_RefreshTokenCookieName, refreshToken.ToString(), new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Lax,
            Path = c_CookiePath,
            Expires = expires
        });
    }

    private void RemoveRefreshTokenCookie()
    {
        Response.Cookies.Append(c_RefreshTokenCookieName, "", new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Lax,
            Path = c_CookiePath,
            Expires = DateTimeOffset.UnixEpoch
        });
    }
}