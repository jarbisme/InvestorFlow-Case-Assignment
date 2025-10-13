using MinimalAPI.Models;

namespace MinimalAPI.Services
{
    public interface IContactService
    {
        Task<List<Contact>> GetAllContactsAsync();
        Task<Contact?> GetContactByIdAsync(int id);
        Task<Contact> CreateContactAsync(Contact contact);
        Task<Contact?> UpdateContactAsync(int id, Contact contact);
        Task<bool> DeleteContactAsync(int id);

    }
}
