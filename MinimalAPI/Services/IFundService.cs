using FluentResults;
using MinimalAPI.Models;

namespace MinimalAPI.Services
{
    public interface IFundService
    {
        Task<Result<List<Fund>>> GetAllFundsAsync();
        Task<Result<Fund?>> GetFundByIdAsync(int id);
        //Task<Result<List<Contact>>> GetContactsByFundIdAsync(int fundId);
        Task<Result<bool>> AddContactToFundAsync(int fundId, int contactId);
        Task<Result<bool>> RemoveContactFromFundAsync(int fundId, int contactId);
    }
}