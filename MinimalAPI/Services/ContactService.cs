using MinimalAPI.Data;
using MinimalAPI.Models;

namespace MinimalAPI.Services
{
    public class ContactService : IContactService
    {
        private readonly IContactRepository _repository;

        public ContactService(IContactRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<Contact>> GetAllContactsAsync()
        {
           
            return await _repository.GetAllAsync();
        }

        public async Task<Contact?> GetContactByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }
        public async Task<ServiceResult<Contact>> CreateContactAsync(Contact contact)
        {
            try
            {
                var createdContact = await _repository.AddAsync(contact);
                return ServiceResult<Contact>.Success(createdContact);
            }
            catch (Exception ex)
            {
                return ServiceResult<Contact>.Failure("An error occurred while creating the contact.", new List<string> { ex.Message });
            }
        }

        public async Task<Contact?> UpdateContactAsync(int id, Contact contact)
        {
            return await _repository.UpdateAsync(contact);
        }

        public async Task<bool> DeleteContactAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }
    }
}
