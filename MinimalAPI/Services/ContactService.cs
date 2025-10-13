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

        public Task<Contact?> GetContactByIdAsync(int id)
        {
            return _repository.GetByIdAsync(id);
        }
        public Task<Contact> CreateContactAsync(Contact contact)
        {
            throw new NotImplementedException();
        }

        public Task<Contact?> UpdateContactAsync(int id, Contact contact)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteContactAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
