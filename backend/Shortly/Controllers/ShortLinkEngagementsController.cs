using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Shortly.Domain.Entities;
using Shortly.Domain.Services;
using Shortly.Features.ShortLinkStats.Dto;

namespace Shortly.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ShortLinkEngagementsController : ControllerBase
{
    private readonly ILogger<ShortLinkEngagementsController> _logger;
    private readonly IShortLinkEngagementsService _engagementService;

    private long CurrentUserId => long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    public ShortLinkEngagementsController(
        ILogger<ShortLinkEngagementsController> logger,
        IShortLinkEngagementsService engagementsService)
    {
        _logger = logger;
        _engagementService = engagementsService;
    }

    [HttpGet("summary")]
    public async Task<ActionResult<ShortLinkEngagementSummaryResponse>> Summary(
        [FromQuery] bool? includeInactive = null,
        [FromQuery] DateTimeOffset? from = null,
        [FromQuery] DateTimeOffset? to = null)
    {
        var result = await _engagementService.GetSummaryAsync(CurrentUserId, includeInactive, from, to);

        return Ok(result);
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<ShortLinkEngagementResponse>> Get(long id)
    {
        var result = await _engagementService.GetAsync(id);

        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpGet("engagements")]
    public async Task<ActionResult<IEnumerable<ShortLinkEngagementResponse>>> List(
        [FromQuery] bool? includeInactive = null,
        [FromQuery] DateTimeOffset? from = null,
        [FromQuery] DateTimeOffset? to = null,
        [FromQuery] int? skip = null,
        [FromQuery] int? take = null,
        [FromQuery] long? shortLinkId = null)
    {
        var result = await _engagementService.ListAsync(
            CurrentUserId,
            includeInactive,
            from,
            to,
            skip,
            take,
            shortLinkId);

        return Ok(result.Select(ToShortLinkEngagementResponse));
    }

    private static ShortLinkEngagementResponse ToShortLinkEngagementResponse(ShortLinkEngagement value)
    {
        return new ShortLinkEngagementResponse
        {
            Country = value.Country,
            Id = value.Id,
            Referer = value.Referer,
            ShortLinkId = value.ShortLinkId,
            UserAgent = value.UserAgent,
        };
    }
}