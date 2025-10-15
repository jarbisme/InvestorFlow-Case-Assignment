using MinimalAPI.Models;
using MinimalAPI.Services;

namespace MinimalAPI.Endpoints
{
    public static class ContactEndpoints
    {
        public static void MapContactEndpoints(this IEndpointRouteBuilder endpoints)
        {
            // Get the handlers from the DI container
            var handlers = endpoints.ServiceProvider.GetRequiredService<ContactEndpointHandlers>();

            var contactsGroups = endpoints.MapGroup("/api/contacts")
                .WithTags("Contacts");

            contactsGroups.MapGet("/", handlers.GetAllContacts)
                .WithName("GetContacts")
                .WithSummary("Get all contacts")
                .WithDescription("Retrieves a list of all contacts from the database")
                .Produces<ApiResponse<List<Contact>>>(StatusCodes.Status200OK)
                .WithOpenApi();

            contactsGroups.MapGet("/{id}", handlers.GetContactById)
                .WithName("GetContactById")
                .WithSummary("Get contact by Id")
                .WithDescription("Retrieves a single contact by its unique identifier")
                .Produces<ApiResponse<Contact>>(StatusCodes.Status200OK)
                .WithOpenApi(operation =>
                {
                    operation.Parameters[0].Description = "The unique identifier of the contact";
                    return operation;
                });

            contactsGroups.MapPost("/", handlers.CreateContact)
                .WithName("CreateContact")
                .WithSummary("Create a new contact")
                .WithDescription("Creates a new contact with the provided information. Returns the created contact with its assigned Id.")
                .Produces<ApiResponse<Contact>>(StatusCodes.Status201Created)
                .WithOpenApi();

            contactsGroups.MapPut("/{id}", handlers.UpdateContact)
                .WithName("UpdateContact")
                .WithSummary("Update an existing contact")
                .WithDescription("Updates the details of an existing contact by its Id. All fields will be replaced with the provided values.")
                .Produces<ApiResponse<Contact>>(StatusCodes.Status200OK)
                .WithOpenApi(operation =>
                {
                    operation.Parameters[0].Description = "The unique identifier of the contact to update";
                    return operation;
                });

            contactsGroups.MapDelete("/{id}", handlers.DeleteContact)
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
