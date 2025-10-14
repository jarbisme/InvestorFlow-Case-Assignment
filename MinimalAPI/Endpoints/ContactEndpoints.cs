using MinimalAPI.Models;
using MinimalAPI.Services;
using System.Runtime.CompilerServices;

namespace MinimalAPI.Endpoints
{
    public static class ContactEndpoints
    {
        public static void MapContactEndpoints(this IEndpointRouteBuilder endpoints)
        {
            var contactsGroups = endpoints.MapGroup("/api/contacts")
                .WithTags("Contacts");

            contactsGroups.MapGet("/", GetAllContacts)
                .WithName("GetContacts")
                .WithSummary("Get all contacts")
                .WithDescription("Retreives a list of all contacts")
                .WithOpenApi();

            contactsGroups.MapGet("/{id}", GetContactById)
                .WithName("GetContactById")
                .WithSummary("Get contact by Id")
                .WithDescription("Retreives a single contact by its Id")
                .WithOpenApi();

            contactsGroups.MapPost("/", CreateContact)
                .WithName("CreateContact")
                .WithSummary("Create a new contact")
                .WithDescription("Creates a new contact with the provided information")
                .WithOpenApi();

            contactsGroups.MapPut("/{id}", UpdateContact)
                .WithName("UpdateContact")
                .WithSummary("Update an existing contact")
                .WithDescription("Updates the details of an existing contact by its Id")
                .WithOpenApi();

            contactsGroups.MapDelete("/{id}", DeleteContact)
                .WithName("DeleteContact")
                .WithSummary("Delete a contact")
                .WithDescription("Deletes a contact by its Id")
                .WithOpenApi();
        }

        private static async Task<IResult> GetAllContacts(IContactService contactService)
        {
            List<Contact> contacts = await contactService.GetAllContactsAsync();
            return Results.Ok(contacts);
        }

        private static async Task<IResult> GetContactById(int id, IContactService contactService)
        {
            var contact = await contactService.GetContactByIdAsync(id);
            return Results.Ok(contact);
        }

        private static async Task<IResult> CreateContact(Contact contact, IContactService contactService)
        {
            var createdContact = await contactService.CreateContactAsync(contact);
            return Results.Created($"/api/contacts/{createdContact.Id}", createdContact);
        }

        private static async Task<IResult> UpdateContact(int id, Contact contact, IContactService contactService)
        {
            if (id != contact.Id)
            {
                return Results.BadRequest("Contact ID mismatch");
            }
            var updatedContact = await contactService.UpdateContactAsync(id, contact);
            if (updatedContact == null)
            {
                return Results.NotFound();
            }
            return Results.Ok(updatedContact);
        }

        private static async Task<IResult> DeleteContact(int id, IContactService contactService)
        {
            var deleted = await contactService.DeleteContactAsync(id);
            if (!deleted)
            {
                return Results.NotFound();
            }
            return Results.NoContent();
        }
    }
}
