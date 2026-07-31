using System.ComponentModel.DataAnnotations;

namespace DotnetTemplate.Requests;

public sealed class UserResetPasswordRequest
{
    [Required(ErrorMessage = "Token é obrigatório.")]
    public string Token { get; set; } = string.Empty;

    [Required(ErrorMessage = "Nova senha é obrigatória.")]
    [MinLength(8, ErrorMessage = "A senha deve possuir pelo menos 8 caracteres.")]
    public string NewPassword { get; set; } = string.Empty;
}
