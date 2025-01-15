using System.ComponentModel.DataAnnotations;

namespace ModuleHeritageHub.Domain.DTO
{
    public class PageDTO
    {
        public Guid Id { get; set; } = Guid.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string InfoList { get; set; } = string.Empty; 
        public string Body { get; set; } = string.Empty;
        public DateTime LastInteractionAt { get; set; } = DateTime.Now;
        public Dictionary<Guid, string> Images { get; set; } = [];
    }

    public class PageVersionDTO
    {
        public Guid VersionId { get; set; } = Guid.Empty;
        public Guid PageId { get; set; } = Guid.Empty;
        public UserDTO Owner { get; set; } = null!;
        public string Body { get; set; } = string.Empty;
        public DateTime EditedAt { get; set; } = DateTime.Now;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public Dictionary<Guid, string> Images { get; set; } = [];
        public bool CurrentUse { get; set; } = false;

    }

    public class PageCreateDTO
    {
        [Required(ErrorMessage = "Title is required.")]
        [StringLength(100, ErrorMessage = "Title length must be between 3 and 100 characters.", MinimumLength = 3)]
        public string Title { get; set; } = string.Empty;
        [Required(ErrorMessage = "Description is required.")]
        [StringLength(500, ErrorMessage = "Description length must not exceed 500 characters.")]
        public string Description { get; set; } = string.Empty;
        [StringLength(1000, ErrorMessage = "InfoList length must not exceed 1000 characters.")]
        public string InfoList { get; set; } = string.Empty;
        [Required(ErrorMessage = "Body is required.")]
        [MinLength(10, ErrorMessage = "Body must be at least 10 characters long.")]
        public string Body { get; set; } = string.Empty;
        public List<Guid> Images { get; set; } = [];
    }

    public class PageEditDTO
    {
        [Required(ErrorMessage = "Title is required.")]
        [StringLength(100, ErrorMessage = "Title length must be between 3 and 100 characters.", MinimumLength = 3)]
        public string Title { get; set; } = string.Empty;
        [Required(ErrorMessage = "Description is required.")]
        [StringLength(500, ErrorMessage = "Description length must not exceed 500 characters.")]
        public string Description { get; set; } = string.Empty;
        [StringLength(1000, ErrorMessage = "InfoList length must not exceed 1000 characters.")]
        public string InfoList { get; set; } = string.Empty;
        public Guid? CurrentVersionId { get; set; } = null;
    }

    public class PageVersionCreateEditDTO
    {
        [Required(ErrorMessage = "Body is required.")]
        [MinLength(10, ErrorMessage = "Body must be at least 10 characters long.")]
        public string Body { get; set; } = string.Empty;
        public List<Guid> Images { get; set; } = [];
    }
}