namespace DotnetTemplate.Responses;

public sealed class LoginResponse
{
    public UserResponse User { get; set; } = new();
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}
