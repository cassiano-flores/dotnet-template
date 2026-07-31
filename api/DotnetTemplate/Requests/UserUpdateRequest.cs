using System.ComponentModel.DataAnnotations;

namespace DotnetTemplate.Requests;

public sealed class UserUpdateRequest
{
    [Required(ErrorMessage = "Nome é obrigatório.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Nome deve possuir entre 2 e 100 caracteres.")]
    public string Name { get; set; } = string.Empty;
}
