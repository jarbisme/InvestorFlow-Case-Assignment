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
    }
}
