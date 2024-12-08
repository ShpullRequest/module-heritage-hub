namespace ModuleHeritageHub.Domain.Model
{
    public class Image
    {
       public Guid Id { get; set; } = Guid.NewGuid();
       public string Path { get; set; } = string.Empty;
    }
}