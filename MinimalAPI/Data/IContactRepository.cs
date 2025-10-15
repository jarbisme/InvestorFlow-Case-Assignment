using MinimalAPI.Models;

namespace MinimalAPI.Data
{
    /// <summary>
    /// Interface defining CRUD operations for Contact entities.
    /// </summary>
    public interface IContactRepository
    {
        Task<List<Contact>> GetAllAsync();
        Task<Contact?> GetByIdAsync(int id);
        Task<Contact> AddAsync(Contact contact);
        Task<Contact?> UpdateAsync(Contact contact);
        Task<bool> DeleteAsync(int id);
    }
}
