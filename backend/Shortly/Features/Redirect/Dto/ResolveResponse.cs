namespace Shortly.Features.Redirect.Dto;

public record ResolveResponse
{
    public string TargetUrl { get; init; } = default!;
}
