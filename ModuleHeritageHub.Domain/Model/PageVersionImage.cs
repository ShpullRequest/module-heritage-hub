namespace ModuleHeritageHub.Domain.Model
{
    public class PageVersionImage
    {
        public Guid VersionId = Guid.Empty;
        public Guid ImageId = Guid.Empty;

        public PageVersion PageVersion { get; set; } = null!;
        public Image Image { get; set; } = null!;
    }
}