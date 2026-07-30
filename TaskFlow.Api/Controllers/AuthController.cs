using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Api.DTOs;
using TaskFlow.Infrastructure.Identity;

namespace TaskFlow.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.DisplayName))
        {
            ModelState.AddModelError(nameof(dto.DisplayName), "El nombre es obligatorio.");
            return ValidationProblem(ModelState);
        }
        var user = new ApplicationUser
        {
            DisplayName = dto.DisplayName.Trim(),
            UserName = dto.Email.Trim().ToLowerInvariant(),
            Email = dto.Email.Trim().ToLowerInvariant()
        };
        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError(error.Code, Translate(error.Description));
            return ValidationProblem(ModelState);
        }
        await _signInManager.SignInAsync(user, isPersistent: false);
        return Ok(new UserInfoDto(user.DisplayName, user.Email!));
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email.Trim());
        if (user is null)
            return Unauthorized(new { message = "Correo o contraseña incorrectos." });
        var result = await _signInManager.PasswordSignInAsync(
            user, dto.Password, isPersistent: false, lockoutOnFailure: true);
        return result.Succeeded
            ? Ok(new UserInfoDto(user.DisplayName, user.Email!))
            : Unauthorized(new { message = "Correo o contraseña incorrectos." });
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return NoContent();
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        var user = await _userManager.GetUserAsync(User);
        return user is null
            ? Unauthorized()
            : Ok(new UserInfoDto(user.DisplayName, user.Email!));
    }

    private static string Translate(string description)
    {
        if (description.Contains("already taken", StringComparison.OrdinalIgnoreCase))
            return "Este correo ya está registrado.";
        if (description.Contains("Passwords must", StringComparison.OrdinalIgnoreCase))
            return "La contraseña debe incluir mayúscula, minúscula y número.";
        return description;
    }
}
