using Common.Models;
using DotnetTemplate.Options;
using DotnetTemplate.Repositories;
using DotnetTemplate.Requests;
using DotnetTemplate.Responses;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Options;

namespace DotnetTemplate.Services;

public sealed class AuthService
{
    private readonly UserRepository _userRepository;
    private readonly JwtService _jwtService;
    private readonly PasswordService _passwordService;
    private readonly TokenService _tokenService;
    private readonly RefreshTokenRepository _refreshTokenRepository;
    private readonly CurrentUserService _currentUserService;
    private readonly JwtOptions _jwtOptions;
    private readonly IEmailService _emailService;

    public AuthService(
        UserRepository repository,
        JwtService jwtService,
        PasswordService passwordService,
        TokenService tokenService,
        RefreshTokenRepository refreshTokenRepository,
        CurrentUserService currentUserService,
        IEmailService emailService,
        IOptions<JwtOptions> jwtOptions)
    {
        _userRepository = repository;
        _jwtService = jwtService;
        _passwordService = passwordService;
        _tokenService = tokenService;
        _refreshTokenRepository = refreshTokenRepository;
        _currentUserService = currentUserService;
        _emailService = emailService;
        _jwtOptions = jwtOptions.Value;
    }

    public async Task<LoginResponse> Login(UserLoginRequest request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByEmail(request.Email);

        if (user == null)
            throw new UnauthorizedAccessException("E-mail ou senha inválidos.");

        if (!_passwordService.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("E-mail ou senha inválidos.");

        user.LastLoginAt = DateTime.UtcNow;
        await _userRepository.UpdateUser(user);

        var accessToken = _jwtService.GenerateAccessToken(user);

        var refreshToken = new RefreshToken
        {
            UserId = user.Id,
            Token = _tokenService.GenerateRefreshToken(),
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenExpirationDays)
        };
        await _refreshTokenRepository.CreateToken(refreshToken);

        return new LoginResponse
        {
            AccessToken = accessToken.Token,
            RefreshToken = refreshToken.Token,
            ExpiresAt = refreshToken.ExpiresAt,

            User = new UserResponse
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email
            },
        };
    }

    public async Task Logout(string refreshToken, CancellationToken cancellationToken)
    {
        var token = await _refreshTokenRepository.GetTokenByToken(refreshToken);

        if (token == null)
            return;

        token.RevokedAt = DateTime.UtcNow;

        await _refreshTokenRepository.UpdateToken(token);
    }

    public async Task ForgotPassword(UserForgotPasswordRequest request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByEmail(request.Email);

        if (user == null)
            throw new KeyNotFoundException("E-mail não cadastrado.");

        var resetToken = _tokenService.GeneratePasswordResetToken();

        user.PasswordResetToken = resetToken;
        user.PasswordResetTokenExpiresAt = DateTime.UtcNow.AddHours(1);

        await _userRepository.UpdateUser(user);

        var body = $"""
        <h2>Redefinição de senha</h2>

        <p>Você solicitou a redefinição da sua senha.</p>

        <p>
            Seu token de redefinição é:
        </p>

        <p>
            <strong>{resetToken}</strong>
        </p>

        <p>
            Este token é válido por 1 hora.
        </p>

        <p>
            Se você não solicitou a redefinição,
            ignore este e-mail.
        </p>
        """;

        await _emailService.Send(user.Email, "Redefinição de senha", body, true, cancellationToken);
    }

    public async Task ResetPassword(UserResetPasswordRequest request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByPasswordResetToken(request.Token);

        if (user == null)
            throw new InvalidOperationException("Token inválido.");

        if (!user.PasswordResetTokenExpiresAt.HasValue ||
            user.PasswordResetTokenExpiresAt.Value < DateTime.UtcNow)
        {
            throw new InvalidOperationException("Token expirado.");
        }

        user.PasswordHash = _passwordService.Hash(request.NewPassword);
        user.PasswordResetToken = null;
        user.PasswordResetTokenExpiresAt = null;

        await _userRepository.UpdateUser(user);
        await _refreshTokenRepository.RevokeAllTokensByUserId(user.Id);
    }

    public async Task<LoginResponse> RefreshToken(string refreshToken, CancellationToken cancellationToken)
    {
        var token = await _refreshTokenRepository.GetTokenByToken(refreshToken);

        if (token == null)
            throw new UnauthorizedAccessException("Refresh Token inválido.");

        if (!token.IsActive)
            throw new UnauthorizedAccessException("Refresh Token expirado.");

        token.RevokedAt = DateTime.UtcNow;
        await _refreshTokenRepository.UpdateToken(token);

        var novoRefresh = new RefreshToken
        {
            UserId = token.User.Id,
            Token = _tokenService.GenerateRefreshToken(),
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenExpirationDays)
        };

        await _refreshTokenRepository.CreateToken(novoRefresh);

        var accessToken = _jwtService.GenerateAccessToken(token.User);

        return new LoginResponse
        {
            AccessToken = accessToken.Token,
            RefreshToken = novoRefresh.Token,
            ExpiresAt = novoRefresh.ExpiresAt,

            User = new UserResponse
            {
                Id = token.User.Id,
                Name = token.User.Name,
                Email = token.User.Email
            }
        };
    }

    public async Task<UserResponse> Me(CancellationToken cancellationToken)
    {
        var id = _currentUserService.GetUserId();

        var usuario = await _userRepository.GetUserById(id);

        if (usuario == null)
            throw new UnauthorizedAccessException($"Usuário não logado. ID: {id}");

        return new UserResponse
        {
            Id = usuario.Id,
            Name = usuario.Name,
            Email = usuario.Email
        };
    }
}
