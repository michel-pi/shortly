using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Shortly.Domain.Services;
using Shortly.Features.Redirect.Dto;

namespace Shortly.Controllers;

[ApiController]
[Route("r")]
[AllowAnonymous]
public class RedirectController : ControllerBase
{
    private readonly ILogger<AccessKeysController> _logger;
    private readonly IShortLinksService _shortLinksService;
    private readonly IShortLinkEngagementsService _engagementService;
    private readonly IGeolocationService _geolocationService;

    public RedirectController(
        ILogger<AccessKeysController> logger,
        IShortLinksService shortLinksService,
        IShortLinkEngagementsService engagementService,
        IGeolocationService geolocationService)
    {
        _logger = logger;
        _shortLinksService = shortLinksService;
        _engagementService = engagementService;
        _geolocationService = geolocationService;
    }

    [HttpGet("{code}")]
    public async Task<IActionResult> ResolveAndRedirect(string code)
    {
        var shortLink = await _shortLinksService.GetByShortCodeAsync(code);

        if (shortLink == null)
        {
            return NotFound();
        }

        if (!shortLink.IsActive)
        {
            return StatusCode(StatusCodes.Status410Gone);
        }

        _ = _engagementService.CreateAsync(
            shortLink.Id,
            HttpContext.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0",
            HttpContext.Request.Headers.UserAgent.ToString(),
            HttpContext.Request.Headers.Referer.ToString(),
            _geolocationService.LookupCountry(HttpContext.Connection.RemoteIpAddress));

        return Redirect(shortLink.TargetUrl);
    }

    [HttpHead("{code}")] // for preview in messenger
    public async Task<IActionResult> Head(string code)
    {
        var shortLink = await _shortLinksService.GetByShortCodeAsync(code);

        if (shortLink == null)
        {
            return NotFound();
        }

        if (!shortLink.IsActive)
        {
            return StatusCode(StatusCodes.Status410Gone);
        }

        Response.Headers.Location = shortLink.TargetUrl;

        return StatusCode(StatusCodes.Status302Found);
    }

    [HttpGet("{code}/target")]
    public async Task<ActionResult<ResolveResponse>> ResolveTarget(string code)
    {
        var shortLink = await _shortLinksService.GetByShortCodeAsync(code);

        if (shortLink == null)
        {
            return NotFound();
        }

        if (!shortLink.IsActive)
        {
            return StatusCode(StatusCodes.Status410Gone);
        }

        _ = _engagementService.CreateAsync(
            shortLink.Id,
            HttpContext.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0",
            HttpContext.Request.Headers.UserAgent.ToString(),
            HttpContext.Request.Headers.Referer.ToString(),
            _geolocationService.LookupCountry(HttpContext.Connection.RemoteIpAddress));

        return Ok(new ResolveResponse
        {
            TargetUrl = shortLink.TargetUrl
        });
    }
}
