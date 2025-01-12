using System.ComponentModel.DataAnnotations;
using ModuleHeritageHub.Domain.Model;

namespace ModuleHeritageHub.Domain.DTO
{
    public class AuthDTO
    {
        public string Token { get; set; } = string.Empty;
    }

    public class RegisterDTO
    {
        [Required(ErrorMessage = "First name is required")]
        [StringLength(50, ErrorMessage = "First name must be less than 50 characters")]
        public string FirstName { get; set; } = string.Empty;
        [Required(ErrorMessage = "Last name is required")]
        [StringLength(50, ErrorMessage = "Last name must be less than 50 characters")]
        public string LastName { get; set; } = string.Empty;
        [Required(ErrorMessage = "Login is required")]
        [StringLength(20, MinimumLength = 5, ErrorMessage = "Login must be between 5 and 20 characters")]
        public string Login { get; set; } = string.Empty;
        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long")]
        public string Password { get; set; } = string.Empty;
        [Required(ErrorMessage = "Role is required")]
        public UserRole Role { get; set; }
    }

    public class LoginDTO {
        [Required(ErrorMessage = "Login is required")]
        [StringLength(20, MinimumLength = 5, ErrorMessage = "Login must be between 5 and 20 characters")]
        public string Login { get; set; } = string.Empty;
        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long")]
        public string Password { get; set; } = string.Empty;
    }
}