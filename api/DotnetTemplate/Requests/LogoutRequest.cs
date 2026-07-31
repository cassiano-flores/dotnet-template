using System.ComponentModel.DataAnnotations;

namespace DotnetTemplate.Requests;

public sealed class LogoutRequest
{
    [Required(ErrorMessage = "Refresh Token é obrigatório.")]
    public string RefreshToken { get; set; } = string.Empty;
}
