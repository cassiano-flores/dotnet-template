using System.ComponentModel.DataAnnotations;

namespace DotnetTemplate.Requests;

public sealed class UserRefreshTokenRequest
{
    [Required(ErrorMessage = "Refresh Token é obrigatório.")]
    public string RefreshToken { get; set; } = string.Empty;
}
