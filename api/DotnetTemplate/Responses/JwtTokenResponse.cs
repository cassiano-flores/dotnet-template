namespace DotnetTemplate.Services;

public sealed class JwtTokenResponse
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}
