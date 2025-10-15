using Microsoft.EntityFrameworkCore;
using MinimalAPI.Models;

namespace MinimalAPI.Data
{
    public class ContactRepository : IContactRepository
    {
        private readonly ApplicationDbContext _context;

        public ContactRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Contact>> GetAllAsync()
        {
            return await _context.Contacts.ToListAsync();
        }

        public async Task<Contact?> GetByIdAsync(int id)
        {
            return await _context.Contacts.FindAsync(id);
        }

        public async Task<Contact> AddAsync(Contact contact)
        {
            if (string.IsNullOrWhiteSpace(contact.Name))
                throw new ArgumentException("Contact name is required.");

            _context.Contacts.Add(contact);
            await _context.SaveChangesAsync();
            return contact;
        }

        public async Task<Contact?> UpdateAsync(Contact contact)
        {
            if (string.IsNullOrWhiteSpace(contact.Name))
                throw new ArgumentException("Contact name is required.");

            var existing = await _context.Contacts.FindAsync(contact.Id);
            if (existing == null)
                return null;

            // Preserve the existing fund relationship if not specified in the update
            if (contact.FundId == null && existing.FundId != null)
            {
                contact.FundId = existing.FundId;
            }

            _context.Entry(existing).CurrentValues.SetValues(contact);
            await _context.SaveChangesAsync();
            return contact;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            // Check if contact is assigned to a fund - directly using the context
            var contact = await _context.Contacts.FindAsync(id);
            if (contact == null)
                return false;
                
            if (contact.FundId != null)
                throw new InvalidOperationException("Cannot delete a contact that is assigned to a fund.");

            _context.Contacts.Remove(contact);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
