using MinimalAPI.Models;

namespace MinimalAPI.Data
{
    public interface IContactRepository
    {
        Task<List<Contact>> GetAllAsync();
        Task<Contact?> GetByIdAsync(int id);
        Task<Contact> AddAsync(Contact contact);
        Task<Contact?> UpdateAsync(Contact contact);
        Task<bool> DeleteAsync(int id);
    }
}
