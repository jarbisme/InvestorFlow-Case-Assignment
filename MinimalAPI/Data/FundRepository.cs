using Microsoft.EntityFrameworkCore;
using MinimalAPI.Models;

namespace MinimalAPI.Data
{
    /// <summary>
    /// Repository implementation for data access layer for Fund entities. Handles database operations and entity persistence.
    /// </summary>
    public class FundRepository : IFundRepository
    {
        private readonly ApplicationDbContext _context;

        public FundRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all funds from the database.
        /// </summary>
        /// <returns>List of all funds</returns>
        public async Task<List<Fund>> GetAllAsync()
        {
            return await _context.Funds.ToListAsync();
        }

        /// <summary>
        /// Retrieves a specific fund by its ID, including its associated contacts.
        /// </summary>
        /// <param name="id">The unique identifier of the fund</param>
        /// <returns>The fund if found, otherwise null</returns>
        public async Task<Fund?> GetByIdAsync(int id)
        {
            return await _context.Funds
                .Include(f => f.Contacts)
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        /// <summary>
        /// Adds an existing contact to a fund.
        /// </summary>
        /// <param name="fundId">The ID of the fund</param>
        /// <param name="contactId">The ID of the contact</param>
        /// <returns>True if the contact was added, false if not found</returns>
        public async Task<bool> AddContactToFundAsync(int fundId, int contactId)
        {
            var contact = await _context.Contacts.FindAsync(contactId);
            if (contact == null)
                return false;

            contact.FundId = fundId;
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Removes a contact from a fund.
        /// </summary>
        /// <param name="fundId">The ID of the fund</param>
        /// <param name="contactId">The ID of the contact</param>
        /// <returns>True if the contact was removed, false if not found</returns>
        public async Task<bool> RemoveContactFromFundAsync(int fundId, int contactId)
        {
            var contact = await _context.Contacts.FindAsync(contactId);

            // Check if the contact exists and is associated with the specified fund
            if (contact == null || contact.FundId != fundId)
                return false;

            contact.FundId = null;
            contact.Fund = null;
            await _context.SaveChangesAsync();
            return true;
        }

        // TODO: This method is not currently used
        public async Task<List<Contact>> GetContactsByFundIdAsync(int fundId)
        {
            var fund = await _context.Funds
                .Include(f => f.Contacts)
                .FirstOrDefaultAsync(f => f.Id == fundId);

            return fund?.Contacts.ToList() ?? new List<Contact>();
        }
    }
}
