using FluentResults;
using MinimalAPI.Models;

namespace MinimalAPI.Services
{
    /// <summary>
    /// Interface defining business logic operations for Contact entities.
    /// </summary>
    public interface IContactService
    {
        Task<Result<List<Contact>>> GetAllContactsAsync();
        Task<Result<Contact?>> GetContactByIdAsync(int id);
        Task<Result<Contact>> CreateContactAsync(Contact contact);
        Task<Result<Contact?>> UpdateContactAsync(int id, Contact contact);
        Task<Result<bool>> DeleteContactAsync(int id);

    }
}
