using System.ComponentModel.DataAnnotations;

namespace DotnetTemplate.Requests;

public sealed class UserForgotPasswordRequest
{
    [Required(ErrorMessage = "E-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "E-mail inválido.")]
    public string Email { get; set; } = string.Empty;
}
