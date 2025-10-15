using MinimalAPI.Models;
using MinimalAPI.Models.DTOs;
using MinimalAPI.Services;

namespace MinimalAPI.Endpoints
{
    public static class ContactEndpoints
    {
        public static void MapContactEndpoints(this IEndpointRouteBuilder endpoints)
        {
            var contactsGroup = endpoints.MapGroup("/api/contacts")
                .WithTags("Contacts");


            contactsGroup.MapGet("/", (IServiceProvider sp) =>
                sp.GetRequiredService<ContactEndpointHandlers>().GetAllContacts())
                .WithName("GetContacts")
                .WithSummary("Get all contacts")
                .WithDescription("Retrieves a list of all contacts from the database")
                .Produces<ApiResponse<List<Contact>>>(StatusCodes.Status200OK)
                .WithOpenApi();

            contactsGroup.MapGet("/{id}", (IServiceProvider sp, int id) =>
                sp.GetRequiredService<ContactEndpointHandlers>().GetContactById(id))
                .WithName("GetContactById")
                .WithSummary("Get contact by Id")
                .WithDescription("Retrieves a single contact by its unique identifier")
                .Produces<ApiResponse<Contact>>(StatusCodes.Status200OK)
                .WithOpenApi(operation =>
                {
                    operation.Parameters[0].Description = "The unique identifier of the contact";
                    return operation;
                });

            contactsGroup.MapPost("/", (IServiceProvider sp, CreateContactRequest request) =>
                sp.GetRequiredService<ContactEndpointHandlers>().CreateContact(request))
                .WithName("CreateContact")
                .WithSummary("Create a new contact")
                .WithDescription("Creates a new contact with the provided information. Returns the created contact with its assigned Id.")
                .Produces<ApiResponse<Contact>>(StatusCodes.Status201Created)
                .WithOpenApi();

            contactsGroup.MapPut("/{id}", (IServiceProvider sp, int id, UpdateContactRequest request) =>
                sp.GetRequiredService<ContactEndpointHandlers>().UpdateContact(id, request))
                .WithName("UpdateContact")
                .WithSummary("Update an existing contact")
                .WithDescription("Updates the details of an existing contact by its Id. All fields will be replaced with the provided values.")
                .Produces<ApiResponse<Contact>>(StatusCodes.Status200OK)
                .WithOpenApi(operation =>
                {
                    operation.Parameters[0].Description = "The unique identifier of the contact to update";
                    return operation;
                });

            contactsGroup.MapDelete("/{id}", (IServiceProvider sp, int id) =>
                sp.GetRequiredService<ContactEndpointHandlers>().DeleteContact(id))
                .WithName("DeleteContact")
                .WithSummary("Delete a contact")
                .WithDescription("Permanently deletes a contact by its Id. This action cannot be undone.")
                .Produces(StatusCodes.Status204NoContent)
                .WithOpenApi(operation =>
                {
                    operation.Parameters[0].Description = "The unique identifier of the contact to delete";
                    return operation;
                });
        }
    }
}
