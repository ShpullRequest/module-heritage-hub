namespace ModuleHeritageHub.Domain.Model
{
    public enum UserRole {
        MEMBER,
        EDITOR,
        ADMIN
    }

    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Login { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public UserRole Role { get; set; } = UserRole.MEMBER;
        public Guid ImageId { get; set; } = Guid.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public Image Image { get; set; } = null!;
        public List<PageVersion> PageVersions { get; set; } = null!;
    }
}