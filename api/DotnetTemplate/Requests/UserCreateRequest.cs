using System.ComponentModel.DataAnnotations;

namespace DotnetTemplate.Requests;

public sealed class UserCreateRequest
{
    [Required(ErrorMessage = "Nome é obrigatório.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Nome deve possuir entre 2 e 100 caracteres.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "E-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "E-mail inválido.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Senha é obrigatória.")]
    [MinLength(8, ErrorMessage = "A senha deve possuir pelo menos 8 caracteres.")]
    public string Password { get; set; } = string.Empty;
}
