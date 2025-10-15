using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Shortly.Domain.Entities;
using Shortly.Domain.Services;
using Shortly.Features.ShortLinks.Dto;

namespace Shortly.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ShortLinksController : ControllerBase
{
    private readonly ILogger<ShortLinksController> _logger;
    private readonly IShortLinksService _shortLinksService;

    private long CurrentUserId => long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    public ShortLinksController(
        ILogger<ShortLinksController> logger,
        IShortLinksService shortLinksService)
    {
        _logger = logger;
        _shortLinksService = shortLinksService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ShortLinkResponse>>> List(
        [FromQuery] int? skip = null,
        [FromQuery] int? take = null,
        CancellationToken ct = default)
    {
        var shortLink = await _shortLinksService.ListAsync(CurrentUserId, skip, take, ct);
        return Ok(shortLink.Select(ToShortLinkResponse));
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<ShortLinkResponse>> Get(
        long id,
        CancellationToken ct = default)
    {
        var shortLink = await _shortLinksService.GetAsync(id, ct);

        if (shortLink == null)
        {
            return NotFound();
        }

        return Ok(ToShortLinkResponse(shortLink));
    }

    [HttpPost]
    public async Task<ActionResult<ShortLinkResponse>> Create(
        [FromBody] CreateShortLinkRequest request,
        CancellationToken ct = default)
    {
        var shortLink = await _shortLinksService.CreateAsync(
            CurrentUserId,
            request.TargetUrl,
            request.IsActive,
            request.ExpiresAt,
            ct);

        return CreatedAtAction(nameof(Get), new { id = shortLink.Id }, ToShortLinkResponse(shortLink));
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<ShortLinkResponse>> Update(
        long id,
        [FromBody] UpdateShortLinkRequest request,
        CancellationToken ct = default)
    {
        var ok = await _shortLinksService.UpdateAsync(id, request.IsActive, request.ExpiresAt, ct);

        if (ok == null)
        {
            return NotFound();
        }

        return Ok(ToShortLinkResponse(ok));
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(
        long id,
        CancellationToken ct = default)
    {
        var ok = await _shortLinksService.DeleteAsync(id, ct);
        return ok ? NoContent() : NotFound();
    }

    private static ShortLinkResponse ToShortLinkResponse(ShortLink shortLink)
    {
        return new ShortLinkResponse
        {
            CreatedAt = shortLink.CreatedAt,
            Id = shortLink.Id,
            IsActive = shortLink.IsActive,
            ShortCode = shortLink.ShortCode,
            TargetUrl = shortLink.TargetUrl
        };
    }
}