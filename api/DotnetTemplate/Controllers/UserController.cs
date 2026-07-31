using DotnetTemplate.Requests;
using DotnetTemplate.Responses;
using DotnetTemplate.Services;
using Microsoft.AspNetCore.Mvc;

namespace DotnetTemplate.Controllers;

[ApiController]
[Route("users")]
public sealed class UserController : ControllerBase
{
    private readonly UserService _userService;

    public UserController(UserService userService)
    {
        _userService = userService;
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserResponse>> GetUserById(Guid id)
    {
        var user = await _userService.GetUserById(id);
        return Ok(user);
    }

    [HttpPost]
    public async Task<ActionResult<UserResponse>> CreateUser(UserCreateRequest request, CancellationToken cancellationToken)
    {
        var user = await _userService.CreateUser(request, cancellationToken);
        return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<UserResponse>> UpdateUser(Guid id, UserUpdateRequest request)
    {
        var user = await _userService.UpdateUser(id, request);
        return Ok(user);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> RemoveUser(Guid id)
    {
        await _userService.RemoveUser(id);
        return NoContent();
    }

    [HttpPut("{id:guid}/password")]
    public async Task<ActionResult<UserResponse>> ChangePassword(Guid id, string currentPassword, string newPassword)
    {
        var user = await _userService.ChangePassword(id, currentPassword, newPassword);
        return Ok(user);
    }
}
