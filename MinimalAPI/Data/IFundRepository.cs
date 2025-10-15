using MinimalAPI.Models;

namespace MinimalAPI.Data
{
    /// <summary>
    /// Interface defining CRUD operations for Fund entities and managing relationships with Contact entities.
    /// </summary>
    public interface IFundRepository
    {
        Task<List<Fund>> GetAllAsync();
        Task<Fund?> GetByIdAsync(int id);
        Task<bool> AddContactToFundAsync(int fundId, int contactId);
        Task<bool> RemoveContactFromFundAsync(int fundId, int contactId);
        Task<List<Contact>> GetContactsByFundIdAsync(int fundId);
    }
}
