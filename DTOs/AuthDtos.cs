using System.ComponentModel.DataAnnotations;
namespace AcxiomCRM.Backend.DTOs;
public record RegisterRequest([Required, StringLength(100)] string FullName, [Required, EmailAddress] string Email, [Required, MinLength(8)] string Password, [Required] string Role);
public record LoginRequest([Required, EmailAddress] string Email, [Required] string Password);
public record UserResponse(string Id, string FullName, string Email, string Role);
