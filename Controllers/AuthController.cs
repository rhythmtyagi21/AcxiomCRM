using AcxiomCRM.Backend.Data;
using AcxiomCRM.Backend.DTOs;
using AcxiomCRM.Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcxiomCRM.Backend.Controllers;
[ApiController, Route("api/auth")]
public class AuthController(UserManager<ApplicationUser> users, SignInManager<ApplicationUser> signIn, CrmDbContext db) : ControllerBase
{
    [AllowAnonymous, HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var role = request.Role.Trim();
        if (role is not ("Admin" or "Manager" or "Sales Executive")) return BadRequest(new { message = "Choose Admin, Manager or Sales Executive." });
        if (role == "Admin" && await users.Users.AnyAsync()) return Forbid();
        var user = new ApplicationUser { UserName = request.Email, Email = request.Email, FullName = request.FullName, IsActive = true };
        var result = await users.CreateAsync(user, request.Password);
        if (!result.Succeeded) return BadRequest(new { errors = result.Errors.Select(e => e.Description) });
        await users.AddToRoleAsync(user, role);
        db.AuditLogs.Add(new Models.AuditLog { User = request.Email, Action = "Registered", Entity = "Authentication", Record = user.Id });
        await db.SaveChangesAsync();
        return Created("/api/auth/me", new UserResponse(user.Id, user.FullName, user.Email!, role));
    }

    [AllowAnonymous, HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var user = await users.FindByEmailAsync(request.Email);
        if (user is null || !user.IsActive) return Unauthorized(new { message = "Invalid credentials or inactive account." });
        var result = await signIn.PasswordSignInAsync(user, request.Password, isPersistent: false, lockoutOnFailure: true);
        db.AuditLogs.Add(new Models.AuditLog { User = request.Email, Action = result.Succeeded ? "Login success" : "Login failed", Entity = "Authentication", Record = user.Id, Result = result.Succeeded ? "Success" : "Failure", IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() });
        await db.SaveChangesAsync();
        if (!result.Succeeded) return Unauthorized(new { message = result.IsLockedOut ? "Account temporarily locked. Try again later." : "Invalid credentials." });
        var role = (await users.GetRolesAsync(user)).FirstOrDefault() ?? "Sales Executive";
        return Ok(new UserResponse(user.Id, user.FullName, user.Email!, role));
    }

    [Authorize, HttpPost("logout")]
    public async Task<IActionResult> Logout() { await signIn.SignOutAsync(); return Ok(new { message = "Logged out successfully." }); }

    [Authorize, HttpGet("me")]
    public async Task<IActionResult> Me() { var user = await users.GetUserAsync(User); if (user is null) return Unauthorized(); var role = (await users.GetRolesAsync(user)).FirstOrDefault() ?? "Sales Executive"; return Ok(new UserResponse(user.Id, user.FullName, user.Email!, role)); }
}
