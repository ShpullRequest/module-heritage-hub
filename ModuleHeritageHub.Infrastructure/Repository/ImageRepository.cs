using System.Security.Cryptography;
using System.Text;
using ModuleHeritageHub.Domain.DTO;
using ModuleHeritageHub.Domain.Model;
using ModuleHeritageHub.Infrastructure.DB;

namespace ModuleHeritageHub.Infrastructure.Repository
{
    public class ImageRepository(DBContext context)
    {
        private readonly DBContext _context = context;

        public async Task<ImageDTO> Create(string originalFileName)
        {
            var image = new Image
            {
                Path = GenerateImagePath(originalFileName)
            };

            _context.Images.Add(image);
            await _context.SaveChangesAsync();

            return new ImageDTO
            {
                Id = image.Id,
                Path = image.Path
            };
        }

        private string GenerateImagePath(string originalFileName)
        {
            var fileExtension = Path.GetExtension(originalFileName);

            fileExtension = string.IsNullOrEmpty(fileExtension) ? ".png" : fileExtension;

            var currentDate = DateTime.UtcNow;
            var datePath = Path.Combine(
                currentDate.Year.ToString(),
                currentDate.Month.ToString("D2"),
                currentDate.Day.ToString("D2")
            );

            var uniqueFileName = GenerateUniqueName(originalFileName) + fileExtension;

            return Path.Combine("static", datePath, uniqueFileName);
        }

        private string GenerateUniqueName(string originalFileName)
        {
            // Используем имя файла + текущий временной штамп + Guid.
            using var sha256 = SHA256.Create();
            var data = Encoding.UTF8.GetBytes(originalFileName + DateTime.UtcNow.Ticks + Guid.NewGuid());
            var hashBytes = sha256.ComputeHash(data);
            return Convert.ToHexString(hashBytes).ToLower();
        }
    }
}