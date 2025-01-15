using System.ComponentModel.DataAnnotations;
using ModuleHeritageHub.Domain.Model;

namespace ModuleHeritageHub.Domain.DTO
{
    public class UserDTO
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Login { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string? ImageUrl { get; set; } = null;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }

    public class UserUpdateDTO
    {
        [Required(ErrorMessage = "First name is required")]
        [StringLength(50, ErrorMessage = "First name must be less than 50 characters")]
        public string FirstName { get; set; } = string.Empty;
        [Required(ErrorMessage = "Last name is required")]
        [StringLength(50, ErrorMessage = "Last name must be less than 50 characters")]
        public string LastName { get; set; } = string.Empty;
        [Required(ErrorMessage = "Role is required")]
        public UserRole? Role { get; set; } = null;
        public Guid? ImageId { get; set; } = null;
    }
}