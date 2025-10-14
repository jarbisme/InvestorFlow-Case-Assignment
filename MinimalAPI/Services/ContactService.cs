using FluentResults;
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

        public async Task<Result<List<Contact>>> GetAllContactsAsync()
        {
            try
            {
                var contacts = await _repository.GetAllAsync();
                return Result.Ok(contacts);

            }
            catch (Exception ex)
            {
                return Result.Fail(new Error("An error occurred while retrieving contacts.").CausedBy(ex.Message));
            }
        }

        public async Task<Result<Contact?>> GetContactByIdAsync(int id)
        {
            try
            {
                var contact = await _repository.GetByIdAsync(id);
                if (contact == null)
                {
                    return Result.Fail(new Error("Contact not found."));
                }
                return Result.Ok(contact);

            }
            catch (Exception ex)
            {
                return Result.Fail(new Error("An error occurred while retrieving the contact.").CausedBy(ex.Message));
            }
        }

        public async Task<Result<Contact>> CreateContactAsync(Contact contact)
        {
            try
            {
                var createdContact = await _repository.AddAsync(contact);
                return Result.Ok(createdContact);
            }
            catch (Exception ex)
            {
                return Result.Fail(new Error("An error occurred while creating the contact.").CausedBy(ex.Message));
            }
        }

        public async Task<Result<Contact?>> UpdateContactAsync(int id, Contact contact)
        {
            try
            {
                var updatedContact = await _repository.UpdateAsync(contact);

                if(updatedContact == null)
                {
                    return Result.Fail("Contact non found");
                }

                return Result.Ok(updatedContact);
            }
            catch (Exception ex)
            {
                return Result.Fail(new Error("An error occurred while updating the contact.").CausedBy(ex.Message));
            }
        }

        public async Task<Result<bool>> DeleteContactAsync(int id)
        {
            try
            {
                var result = await _repository.DeleteAsync(id);
                return Result.Ok(result);
            }
            catch (Exception ex)
            {
                return Result.Fail(new Error("An error occurred while deleting the contact.").CausedBy(ex.Message));
            }
        }
    }
}
    