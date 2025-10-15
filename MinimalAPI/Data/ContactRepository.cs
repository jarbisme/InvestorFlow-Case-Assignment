using Microsoft.EntityFrameworkCore;
using MinimalAPI.Models;

namespace MinimalAPI.Data
{
    /// <summary>
    /// Repository implementation for data access layer for Contact entities. Handles database operations and entity persistence.
    /// </summary>
    public class ContactRepository : IContactRepository
    {
        private readonly ApplicationDbContext _context;

        public ContactRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all contacts from the database.
        /// </summary>
        /// <returns>List of all contacts</returns>
        public async Task<List<Contact>> GetAllAsync()
        {
            return await _context.Contacts.ToListAsync();
        }

        /// <summary>
        /// Retrieves a specific contact by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the contact</param>
        /// <returns>The contact if found, otherwise null</returns>
        public async Task<Contact?> GetByIdAsync(int id)
        {
            return await _context.Contacts.FindAsync(id);
        }

        /// <summary>
        /// Adds a new contact to the database.
        /// </summary>
        /// <param name="contact">The contact to add</param>
        /// <returns>The added contact with generated ID</returns>
        /// <exception cref="ArgumentException">Thrown when contact name is null or empty</exception>
        public async Task<Contact> AddAsync(Contact contact)
        {
            _context.Contacts.Add(contact);
            await _context.SaveChangesAsync();
            return contact;
        }

        /// <summary>
        /// Updates an existing contact in the database.
        /// </summary>
        /// <param name="contact">The contact with updated information</param>
        /// <returns>The updated contact if found, otherwise null</returns>
        /// <exception cref="ArgumentException">Thrown when contact name is null or empty</exception>
        public async Task<Contact?> UpdateAsync(Contact contact)
        {
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

        /// <summary>
        /// Deletes a contact from the database.
        /// Contacts assigned to funds cannot be deleted.
        /// </summary>
        /// <param name="id">The ID of the contact to delete</param>
        /// <returns>True if the contact was deleted, false if not found</returns>
        /// <exception cref="InvalidOperationException">Thrown when trying to delete a contact assigned to a fund</exception>
        public async Task<bool> DeleteAsync(int id)
        {
            // Check if contact exists
            var contact = await _context.Contacts.FindAsync(id);
            if (contact == null)
                return false;

            // Cannot delete contacts assigned to funds
            if (contact.FundId != null)
                throw new InvalidOperationException("Cannot delete a contact that is assigned to a fund.");

            _context.Contacts.Remove(contact);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
