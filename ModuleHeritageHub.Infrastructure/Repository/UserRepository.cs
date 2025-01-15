using Microsoft.EntityFrameworkCore;
using ModuleHeritageHub.Domain.DTO;
using ModuleHeritageHub.Domain.Exceptions;
using ModuleHeritageHub.Domain.Model;
using ModuleHeritageHub.Infrastructure.DB;
using ModuleHeritageHub.Infrastructure.JWT;

namespace ModuleHeritageHub.Infrastructure.Repository
{
    public class UserRepository(DBContext context, Jwt jwtService)
    {
        private readonly DBContext _context = context;
        private readonly Jwt _jwtService = jwtService;

        public async Task<AuthDTO> Login(string login, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Login == login) ?? throw new UnauthorizedAccessException("Invalid login or password.");
            
            var isPasswordValid = BCrypt.Net.BCrypt.Verify(password, user.Password);
            if (!isPasswordValid)
            {
                throw new UnauthorizedAccessException("Invalid login or password.");
            }

            return _jwtService.Generate(user);
        }

        public async Task<AuthDTO> Register(string login, string password, UserRole role, string firstName, string lastName)
        {
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            var user = new User
            {
                Login = login,
                FirstName = firstName,
                LastName = lastName,
                Password = hashedPassword,
                Role = role
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return _jwtService.Generate(user);
        }

        public async Task<UserDTO> GetById(Guid userId)
        {
            var user = await _context.Users
                .Include(u => u.Image)
                .FirstOrDefaultAsync(u => u.Id == userId) 
                ?? throw new NotFoundException("User not found");


            return new UserDTO
            {
                Id = user.Id,
                Login = user.Login,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role.ToString(),
                ImageUrl = user.Image?.Path,
                CreatedAt = user.CreatedAt,
            };
        }

        public async Task<UserDTO[]> GetList()
        {
            var users = await _context.Users
                .Include(u => u.Image)
                .ToArrayAsync();
            
            return users.Select(u => new UserDTO
            {
                Id = u.Id,
                Login = u.Login,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Role = u.Role.ToString(),
                ImageUrl = u.Image?.Path,
                CreatedAt = u.CreatedAt,
            }).ToArray();
        }

        public async Task<UserDTO> Update(Guid userId, UserUpdateDTO data)
        {
            var user = await _context.Users
                .Include(u => u.Image)
                .FirstOrDefaultAsync(u => u.Id == userId) 
                ?? throw new NotFoundException("User not found");
            
            user.FirstName = data.FirstName;
            user.LastName = data.LastName;
            user.Role = data.Role.GetValueOrDefault(UserRole.MEMBER);
            user.ImageId = data.ImageId;
            
            await _context.SaveChangesAsync();

            return new UserDTO
            {
                Id = user.Id,
                Login = user.Login,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role.ToString(),
                ImageUrl = user.Image?.Path,
                CreatedAt = user.CreatedAt,
            };
        }

        public async Task Delete(Guid userId)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId) 
                ?? throw new NotFoundException("User not found");

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }
}