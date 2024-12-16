using System.ComponentModel.DataAnnotations;

namespace Dating.API.DTO;

public class RegisterDto
{
    [Required] public string UserName { get; set; } = string.Empty;
    [Required] public string KnownAs { get; set; } = string.Empty;
    [Required] public string DateOfBirth { get; set; } = string.Empty;
    [Required] public string Gender { get; set; } = string.Empty;
    [Required] public string City { get; set; } = string.Empty;
    [Required] public string Country { get; set; } = string.Empty;
    [Required] public string Password { get; set; } = string.Empty;
}