using MinimalAPI.Models;

namespace MinimalAPI.Services
{
    public class ContactService : IContactService
    {

        public Task<List<Contact>> GetAllContactsAsync()
        {
            var contacts = new[]
            {
                new Contact { Id = 1, Name = "Alice", Email = "alice@email.com", Phone = "8091234567" },
                new Contact { Id = 2, Name = "Mario", Email = "mario@email.com", Phone = "8290983872" },
                new Contact { Id = 3, Name = "Peach", Email = "peach@email.com", Phone = "8290713387" }
            };

            return Task.FromResult(contacts.ToList());
        }

        public Task<Contact?> GetContactByIdAsync(int id)
        {
            var contact = new Contact { Id = 3, Name = "Peach", Email = "peach@email.com", Phone = "8290713387" };
            return Task.FromResult(contact);
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
