using Microsoft.EntityFrameworkCore;
using MinimalAPI.Models;

namespace MinimalAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Contact> Contacts => Set<Contact>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var contacts = new[]
            {
                new Contact { Id = 1, Name = "Alice", Email = "alice@email.com", Phone = "8091234567" },
                new Contact { Id = 2, Name = "Mario", Email = "mario@email.com", Phone = "8290983872" },
                new Contact { Id = 3, Name = "Peach", Email = "peach@email.com", Phone = "8290713387" }
            };

            modelBuilder.Entity<Contact>().HasData(contacts);
        }
    }
}
