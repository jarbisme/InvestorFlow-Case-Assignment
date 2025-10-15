using FluentResults;
using MinimalAPI.Data;
using MinimalAPI.Models;

namespace MinimalAPI.Services
{
    public class FundService : IFundService
    {
        private readonly IFundRepository _fundRepository;
        private readonly IContactRepository _contactRepository;

        public FundService(IFundRepository fundRepository, IContactRepository contactRepository)
        {
            _fundRepository = fundRepository;
            _contactRepository = contactRepository;
        }

        public async Task<Result<List<Fund>>> GetAllFundsAsync()
        {
            try
            {
                var funds = await _fundRepository.GetAllAsync();
                return Result.Ok(funds);
            }
            catch (Exception ex)
            {
                return Result.Fail(new Error("An error occurred while retrieving funds.").CausedBy(ex.Message));
            }
        }

        public async Task<Result<Fund?>> GetFundByIdAsync(int id)
        {
            try
            {
                var fund = await _fundRepository.GetByIdAsync(id);
                if (fund == null)
                {
                    return Result.Fail(new Error("Fund not found."));
                }
                return Result.Ok(fund);
            }
            catch (Exception ex)
            {
                return Result.Fail(new Error("An error occurred while retrieving the fund.").CausedBy(ex.Message));
            }
        }

        public async Task<Result<bool>> AddContactToFundAsync(int fundId, int contactId)
        {
            try
            {
                // First, check if the contact exists
                var fund = await _fundRepository.GetByIdAsync(fundId);
                if (fund == null)
                {
                    return Result.Fail(new Error("Fund not found."));
                }

                // Check if the contact exists
                var contact = await _contactRepository.GetByIdAsync(contactId);
                if (contact == null)
                {
                    return Result.Fail(new Error("Contact not found."));
                }

                // Check if conctact is already assigned to this fund
                if (contact.FundId == fundId)
                {
                    return Result.Fail(new Error("Contact is already assigned to this fund."));
                }

                // Check if contact is assigned with another fund
                if (contact.FundId != null && contact.FundId != fundId)
                {
                    return Result.Fail(new Error("Contact is already assigned to another fund."));
                }

                var success = await _fundRepository.AddContactToFundAsync(fundId, contactId);

                if (!success)
                {
                    return Result.Fail(new Error("Failed to add contact to fund."));
                }

                return Result.Ok(true);
            }
            catch (Exception ex)
            {
                return Result.Fail(new Error("An error occurred while adding the contact to the fund.").CausedBy(ex.Message));
            }
        }

        public async Task<Result<bool>> RemoveContactFromFundAsync(int fundId, int contactId)
        {
            try
            {
                // Check if fund exists
                var fund = await _fundRepository.GetByIdAsync(fundId);
                if (fund == null)
                {
                    return Result.Fail(new Error("Fund not found."));
                }

                var success = await _fundRepository.RemoveContactFromFundAsync(fundId, contactId);

                if (!success)
                {
                    return Result.Fail(new Error("Failed to remove contact from fund. The contact may not be assigned to this fund."));
                }

                return Result.Ok(true);
            }
            catch (Exception ex)
            {
                return Result.Fail(new Error("An error occurred while removing the contact from the fund.").CausedBy(ex.Message));
            }
        }
    }
}
