using DotnetTemplate.Requests;
using DotnetTemplate.Responses;
using DotnetTemplate.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotnetTemplate.Controllers;

[ApiController]
[Route("auth")]
public sealed class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(UserLoginRequest request, CancellationToken cancellationToken)
    {
        var response = await _authService.Login(request, cancellationToken);
        return Ok(response);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout(LogoutRequest request, CancellationToken cancellationToken)
    {
        await _authService.Logout(request.RefreshToken, cancellationToken);
        return NoContent();
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(UserForgotPasswordRequest request, CancellationToken cancellationToken)
    {
        await _authService.ForgotPassword(request, cancellationToken);
        return Ok(new { mensagem = "Se existir uma conta para este e-mail, enviaremos instruções para redefinição de senha." });
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(UserResetPasswordRequest request, CancellationToken cancellationToken)
    {
        await _authService.ResetPassword(request, cancellationToken);
        return Ok(new { mensagem = "Senha alterada com sucesso." });
    }

    [HttpPost("refresh-token")]
    public async Task<ActionResult<LoginResponse>> RefreshToken(UserRefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var response = await _authService.RefreshToken(request.RefreshToken, cancellationToken);
        return Ok(response);
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<UserResponse>> Me(CancellationToken cancellationToken)
    {
        var usuario = await _authService.Me(cancellationToken);
        return Ok(usuario);
    }
}
