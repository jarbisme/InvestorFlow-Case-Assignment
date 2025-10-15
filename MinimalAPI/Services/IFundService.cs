using FluentResults;
using MinimalAPI.Models;

namespace MinimalAPI.Services
{
    /// <summary>
    /// Interface defining business logic operations for Fund entities and managing relationships with Contact entities.
    /// </summary>
    public interface IFundService
    {
        Task<Result<List<Fund>>> GetAllFundsAsync();
        Task<Result<Fund?>> GetFundByIdAsync(int id);
        Task<Result<bool>> AddContactToFundAsync(int fundId, int contactId);
        Task<Result<bool>> RemoveContactFromFundAsync(int fundId, int contactId);
    }
}