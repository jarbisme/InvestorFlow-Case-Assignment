using FluentResults;
using MinimalAPI.Data;
using MinimalAPI.Models;

namespace MinimalAPI.Services
{
    /// <summary>
    /// Business logic layer for Contact operations. Validates rules and orchestrates data access.
    /// </summary>
    public class ContactService : IContactService
    {
        private readonly IContactRepository _contactRepository;

        public ContactService(IContactRepository repository)
        {
            _contactRepository = repository;
        }

        /// <summary>
        /// Retrieves all contacts.
        /// </summary>
        /// <returns>A list of contacts</returns>
        public async Task<Result<List<Contact>>> GetAllContactsAsync()
        {
            try
            {
                var contacts = await _contactRepository.GetAllAsync();
                return Result.Ok(contacts);

            }
            catch (Exception ex)
            {
                return Result.Fail(new Error("An error occurred while retrieving contacts.")
                    .CausedBy(ex.Message)
                    .WithMetadata("IsServerError", true));
            }
        }

        /// <summary>
        /// Retrieves a contact by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the contact</param>
        /// <returns>The contact if found, otherwise an error</returns>
        public async Task<Result<Contact?>> GetContactByIdAsync(int id)
        {
            try
            {
                var contact = await _contactRepository.GetByIdAsync(id);
                if (contact == null)
                {
                    return Result.Fail("Contact not found.");
                }
                return Result.Ok<Contact?>(contact);

            }
            catch (Exception ex)
            {
                return Result.Fail(new Error("An error occurred while retrieving the contact.")
                    .CausedBy(ex.Message)
                    .WithMetadata("IsServerError", true));
            }
        }

        /// <summary>
        /// Creates a new contact.
        /// </summary>
        /// <param name="contact">The contact to create</param>
        /// <returns>The created contact with generated ID</returns>   
        public async Task<Result<Contact>> CreateContactAsync(Contact contact)
        {
            try
            {
                var createdContact = await _contactRepository.AddAsync(contact);
                return Result.Ok(createdContact);
            }
            catch (Exception ex)
            {
                return Result.Fail(new Error("An error occurred while creating the contact.")
                    .CausedBy(ex.Message)
                    .WithMetadata("IsServerError", true));
            }
        }

        /// <summary>
        /// Updates an existing contact.
        /// </summary>
        /// <param name="id">The ID of the contact to update</param>
        /// <param name="contact">The contact with updated information</param>
        /// <returns>The updated contact if found, otherwise an error</returns>
        public async Task<Result<Contact?>> UpdateContactAsync(int id, Contact contact)
        {
            try
            {
                var updatedContact = await _contactRepository.UpdateAsync(contact);

                if (updatedContact == null)
                {
                    return Result.Fail("Contact non found");
                }

                return Result.Ok<Contact?>(updatedContact);
            }
            catch (Exception ex)
            {
                return Result.Fail(new Error("An error occurred while updating the contact.")
                    .CausedBy(ex.Message)
                    .WithMetadata("IsServerError", true));
            }
        }

        /// <summary>
        /// Deletes a contact by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the contact to delete</param>
        /// <returns>True if the contact was deleted, otherwise false</returns>
        public async Task<Result<bool>> DeleteContactAsync(int id)
        {
            try
            {
                // Check if contact exists
                var contact = await _contactRepository.GetByIdAsync(id);
                if (contact == null)
                {
                    return Result.Fail("Contact not found.");
                }

                // Check if contact is assigned to a fund
                if (contact.FundId != null)
                {
                    return Result.Fail("Cannot delete contact assigned to a fund.");
                }

                var success = await _contactRepository.DeleteAsync(id);

                if (!success)
                {
                    return Result.Fail("Contact could not be deleted.");
                }

                return Result.Ok(true);
            }
            catch (Exception ex)
            {
                return Result.Fail(new Error("An error occurred while deleting the contact.")
                    .CausedBy(ex.Message)
                    .WithMetadata("IsServerError", true));
            }
        }
    }
}
