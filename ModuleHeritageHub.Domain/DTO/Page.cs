namespace ModuleHeritageHub.Domain.DTO
{
    class PageDTO
    {
        public Guid Id { get; set; } = Guid.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string InfoList { get; set; } = string.Empty; 
        public string Body { get; set; } = string.Empty;
        public DateTime LastInteractionAt { get; set; } = DateTime.Now;
        public Dictionary<Guid, string> Images { get; set; } = [];
    }
}