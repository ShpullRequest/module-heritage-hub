using Microsoft.EntityFrameworkCore;

namespace RESTfull.Infrastructure
{
    public class Context(DbContextOptions<Context> options) : DbContext(options)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }
    }   
}