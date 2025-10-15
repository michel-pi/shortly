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
using Shortly.Features.AccessKeys.Dto;

namespace Shortly.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AccessKeysController : ControllerBase
{
    private readonly ILogger<AccessKeysController> _logger;
    private readonly IAccessKeysService _accessKeysService;

    private long CurrentUserId => long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    public AccessKeysController(
        ILogger<AccessKeysController> logger,
        IAccessKeysService accessKeysService)
    {
        _logger = logger;
        _accessKeysService = accessKeysService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AccessKeyResponse>>> List(
        [FromQuery] int? skip = null,
        [FromQuery] int? take = null,
        CancellationToken ct = default)
    {
        var accessKeys = await _accessKeysService.ListAsync(CurrentUserId, skip, take, ct);
        return Ok(accessKeys.Select(ToAccessKeyResponse));
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<AccessKeyResponse>> Get(
        long id,
        CancellationToken ct = default)
    {
        var accessKey = await _accessKeysService.GetAsync(id, ct);

        if (accessKey == null)
        {
            return NotFound();
        }

        return Ok(ToAccessKeyResponse(accessKey));
    }

    [HttpPost]
    public async Task<ActionResult<AccessKeyResponse>> Create(
        [FromBody] CreateAccessKeyRequest request,
        CancellationToken ct = default)
    {
        var accessKey = await _accessKeysService.CreateAsync(CurrentUserId, request.Name, request.IsActive, request.ExpiresAt, ct);

        return CreatedAtAction(nameof(Get), new { id = accessKey.Id }, ToAccessKeyResponse(accessKey));
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<AccessKeyResponse>> Update(
        long id,
        [FromBody] UpdateAccessKeyRequest request,
        CancellationToken ct = default)
    {
        var ok = await _accessKeysService.UpdateAsync(id, request.Name, request.IsActive, request.ExpiresAt, ct);

        if (ok == null)
        {
            return NotFound();
        }

        return Ok(ToAccessKeyResponse(ok));
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(
        long id,
        CancellationToken ct = default)
    {
        var ok = await _accessKeysService.DeleteAsync(id, ct);
        return ok ? NoContent() : NotFound();
    }

    private static AccessKeyResponse ToAccessKeyResponse(AccessKey accessKey)
    {
        return new AccessKeyResponse
        {
            CreatedAt = accessKey.CreatedAt,
            Id = accessKey.Id,
            IsActive = accessKey.IsActive,
            Name = accessKey.Name,
            Token = accessKey.TokenHash
        };
    }
}