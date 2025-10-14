using FluentResults;
using MinimalAPI.Models;

namespace MinimalAPI.Services
{
    public interface IContactService
    {
        Task<Result<List<Contact>>> GetAllContactsAsync();
        Task<Result<Contact?>> GetContactByIdAsync(int id);
        Task<Result<Contact>> CreateContactAsync(Contact contact);
        Task<Result<Contact?>> UpdateContactAsync(int id, Contact contact);
        Task<Result<bool>> DeleteContactAsync(int id);

    }
}
