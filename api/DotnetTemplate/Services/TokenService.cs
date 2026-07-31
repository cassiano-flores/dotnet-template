using System.Security.Cryptography;

namespace DotnetTemplate.Services;

public sealed class TokenService
{
    public string GenerateRefreshToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);

        return Convert.ToBase64String(bytes);
    }

    public string GeneratePasswordResetToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(32);

        return Convert.ToBase64String(bytes);
    }
}
