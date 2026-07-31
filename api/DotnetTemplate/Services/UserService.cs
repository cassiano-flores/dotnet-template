using Common.Models;
using DotnetTemplate.Exceptions;
using DotnetTemplate.Repositories;
using DotnetTemplate.Requests;
using DotnetTemplate.Responses;

namespace DotnetTemplate.Services;

public class UserService
{
    private readonly UserRepository _repository;
    private readonly PasswordService _passwordService;

    public UserService(UserRepository repository, PasswordService passwordService)
    {
        _repository = repository;
        _passwordService = passwordService;
    }

    public async Task<UserResponse?> GetUserById(Guid id)
    {
        var user = await _repository.GetUserById(id);

        if (user == null)
            throw new KeyNotFoundException($"Usuário não encontrado. ID: {id}");

        return UserResponseConvert(user);
    }

    public async Task<UserResponse> CreateUser(UserCreateRequest request, CancellationToken cancellationToken)
    {
        var existente = await _repository.GetUserByEmail(request.Email);

        if (existente != null)
            throw new ConflictException("Já existe um usuário com este e-mail.");

        var user = new User
        {
            Name = request.Name,
            Email = request.Email,
            PasswordHash = _passwordService.Hash(request.Password),
        };

        user = await _repository.CreateUser(user);

        return UserResponseConvert(user);
    }

    public async Task<UserResponse> UpdateUser(Guid id, UserUpdateRequest request)
    {
        var user = await _repository.GetUserById(id);

        if (user == null)
            throw new KeyNotFoundException($"Usuário não encontrado. ID: {id}");

        user.Name = request.Name;

        await _repository.UpdateUser(user);

        return UserResponseConvert(user);
    }

    public async Task<UserResponse> ChangePassword(Guid id, string currentPassword, string newPassword)
    {
        var user = await _repository.GetUserById(id);

        if (user == null)
            throw new KeyNotFoundException($"Usuário não encontrado. ID: {id}");

        if (!_passwordService.Verify(currentPassword, user.PasswordHash))
            throw new InvalidOperationException("A senha atual está incorreta.");

        user.PasswordHash = _passwordService.Hash(newPassword);

        await _repository.UpdateUser(user);

        return UserResponseConvert(user);
    }

    public async Task RemoveUser(Guid id)
    {
        var user = await _repository.GetUserById(id);

        if (user == null)
            throw new KeyNotFoundException($"Usuário não encontrado. ID: {id}");

        await _repository.RemoveUser(user);
    }

    private static UserResponse UserResponseConvert(User user)
    {
        return new UserResponse
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            CreatedAt = user.CreatedAt
        };
    }
}
