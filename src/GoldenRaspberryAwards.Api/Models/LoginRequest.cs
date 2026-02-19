using System.ComponentModel.DataAnnotations;

namespace GoldenRaspberryAwards.Api.Models;

public class LoginRequest
{
    [Required(ErrorMessage = "UserName is required.")]
    [StringLength(100, MinimumLength = 1)]
    public string UserName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required.")]
    [StringLength(200, MinimumLength = 1)]
    public string Password { get; set; } = string.Empty;
}
