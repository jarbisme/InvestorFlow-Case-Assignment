using Microsoft.EntityFrameworkCore;
using MinimalAPI.Models;

namespace MinimalAPI.Data
{
    public class FundRepository : IFundRepository
    {
        private readonly ApplicationDbContext _context;

        public FundRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Fund>> GetAllAsync()
        {
            return await _context.Funds.ToListAsync();
        }

        public async Task<Fund?> GetByIdAsync(int id)
        {
            return await _context.Funds.FindAsync(id);
        }

        public async Task<Fund?> GetByIdWithContactsAsync(int id)
        {
            return await _context.Funds
                .Include(f => f.Contacts)
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task<bool> AddContactToFundAsync(int fundId, int contactId)
        {
            var fund = await _context.Funds
                .Include(f => f.Contacts)
                .FirstOrDefaultAsync(f => f.Id == fundId);

            if (fund == null)
                return false;

            var contact = await _context.Contacts.FindAsync(contactId);
            if (contact == null)
                return false;

            // Check if contact is already in this fund
            if (fund.Contacts.Any(c => c.Id == contactId))
                return false; // Contact already assigned to this fund

            // Update both sides of the relationship
            fund.Contacts.Add(contact);
            contact.FundId = fundId;
            contact.Fund = fund;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveContactFromFundAsync(int fundId, int contactId)
        {
            var fund = await _context.Funds
                .Include(f => f.Contacts)
                .FirstOrDefaultAsync(f => f.Id == fundId);

            if (fund == null)
                return false;

            var contact = fund.Contacts.FirstOrDefault(c => c.Id == contactId);
            if (contact == null)
                return false;

            // Update both sides of the relationship
            fund.Contacts.Remove(contact);
            contact.FundId = null;
            contact.Fund = null;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Contact>> GetContactsByFundIdAsync(int fundId)
        {
            var fund = await _context.Funds
                .Include(f => f.Contacts)
                .FirstOrDefaultAsync(f => f.Id == fundId);

            return fund?.Contacts.ToList() ?? new List<Contact>();
        }

        public async Task<bool> IsContactInFundAsync(int contactId)
        {
            var contact = await _context.Contacts.FindAsync(contactId);
            return contact != null && contact.FundId.HasValue;
        }
    }
}
