namespace ModuleHeritageHub.Domain.Model
{
    public class PageVersion
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid PageId { get; set; } = Guid.Empty;
        public Guid OwnerId { get; set; } = Guid.Empty;
        public string Body { get; set; } = string.Empty;
        public DateTime EditedAt { get; set; } = DateTime.Now;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public Page Page { get; set; } = null!;
        public User Owner { get; set; } = null!;
        public List<PageVersionImage> Images { get; set; } = [];
    }
}