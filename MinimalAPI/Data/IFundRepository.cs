using MinimalAPI.Models;

namespace MinimalAPI.Data
{
    public interface IFundRepository
    {
        Task<List<Fund>> GetAllAsync();
        Task<Fund?> GetByIdAsync(int id);
        Task<bool> AddContactToFundAsync(int fundId, int contactId);
        Task<bool> RemoveContactFromFundAsync(int fundId, int contactId);
        Task<List<Contact>> GetContactsByFundIdAsync(int fundId);
    }
}
