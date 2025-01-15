namespace ModuleHeritageHub.Domain.Model
{
    public class Page
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string InfoList { get; set; } = string.Empty;
        public Guid? CurrentVersionId { get; set; }
        
        public PageVersion CurrentVersion { get; set; } = null!;
        public List<PageVersion> PageVersions { get; set; } = [];
    }
}