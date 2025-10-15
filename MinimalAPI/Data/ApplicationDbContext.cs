using Microsoft.EntityFrameworkCore;
using MinimalAPI.Models;

namespace MinimalAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Fund> Funds { get; set; }


        public static async Task SeedDataAsync(ApplicationDbContext context)
        {
            if (!await context.Contacts.AnyAsync())
            {
                await context.Contacts.AddRangeAsync(
                    new Contact { Name = "John Doe", Email = "john@example.com", Phone = "555-0100" },
                    new Contact { Name = "Jane Smith", Email = "jane@example.com", Phone = "555-0101" },
                    new Contact { Name = "Bob Johnson", Email = "bob@example.com" }
                );
                await context.SaveChangesAsync();
            }
        }
    }
}
