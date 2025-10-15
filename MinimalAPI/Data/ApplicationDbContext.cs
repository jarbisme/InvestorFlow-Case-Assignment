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
                // Seed initial funds first and save to get IDs
                var globalEquityFund = new Fund { Name = "Global Equity Fund" };
                var bondFund = new Fund { Name = "Bond Fund" };
                var emergingMarketsFund = new Fund { Name = "Emerging Markets Fund" };
                
                await context.Funds.AddRangeAsync(globalEquityFund, bondFund, emergingMarketsFund);
                await context.SaveChangesAsync();

                // Seed contacts with fund associations
                await context.Contacts.AddRangeAsync(
                    new Contact { Name = "John Doe", Email = "john@example.com", Phone = "555-0100", Fund = globalEquityFund },
                    new Contact { Name = "Jane Smith", Email = "jane@example.com", Phone = "555-0101", Fund = globalEquityFund },
                    new Contact { Name = "Bob Johnson", Email = "bob@example.com", Fund = bondFund },
                    new Contact { Name = "Sarah Williams", Email = "sarah@example.com", Phone = "555-0102", Fund = bondFund },
                    new Contact { Name = "Michael Brown", Email = "michael@example.com", Phone = "555-0103", Fund = emergingMarketsFund },
                    new Contact { Name = "Emily Davis", Email = "emily@example.com", Phone = "555-0104", Fund = emergingMarketsFund }
                );
                await context.SaveChangesAsync();
            }
        }
    }
}
