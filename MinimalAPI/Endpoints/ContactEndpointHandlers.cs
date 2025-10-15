using MinimalAPI.Models;
using MinimalAPI.Services;

namespace MinimalAPI.Endpoints
{
    public class ContactEndpointHandlers
    {
        public async Task<IResult> GetAllContacts(IContactService contactService)
        {
            var result = await contactService.GetAllContactsAsync();

            if (result.IsFailed)
            {
                var errorResponse = ApiResponse<List<Contact>>.Error(
                    result.Errors.FirstOrDefault()?.Message ?? "An error occurred while retrieving contacts.");
                return Results.Json(errorResponse, statusCode: 500);
            }

            var apiResponse = ApiResponse<List<Contact>>.Success(result.Value!, "Contacts retrieved successfully.");
            return Results.Ok(apiResponse);
        }

        public async Task<IResult> GetContactById(int id, IContactService contactService)
        {
            var result = await contactService.GetContactByIdAsync(id);

            if (result.IsFailed)
            {
                var errorResponse = ApiResponse<Contact>.Error(
                    result.Errors.FirstOrDefault()?.Message ?? "An error occurred while retrieving the contact.");
                return Results.Json(errorResponse, statusCode: 500);
            }

            var apiResponse = ApiResponse<Contact>.Success(result.Value!, "Contact retrieved successfully.");
            return Results.Ok(apiResponse);
        }

        public async Task<IResult> CreateContact(Contact contact, IContactService contactService)
        {
            // TODO: Add request validation

            var result = await contactService.CreateContactAsync(contact);

            if (result.IsFailed)
            {
                // Extract error messages from FluentResults
                var errorMessages = result.Errors.Select(e => e.Message).ToList();

                // Check if these are validation errors
                if (errorMessages.Count > 1 || result.Errors.Any(e => e.Metadata.ContainsKey("ValidationError")))
                {
                    var failResponse = ApiResponse<Contact>.Fail(
                        "Validation errors occurred while creating the contact.",
                        result.Errors.Select(e => e.Message).ToList());
                    return Results.BadRequest(failResponse);
                }

                // Single error or unexpected error - treat as server error
                var errorResponse = ApiResponse<Contact>.Error(
                    errorMessages.FirstOrDefault() ?? "An error ocurred");
                return Results.Json(errorResponse, statusCode: 500);
            }

            var successResponse = ApiResponse<Contact>.Success(
                result.Value!,
                "Contact created successfully.");
            return Results.Created($"/api/contacts/{result.Value!.Id}", successResponse);
        }

        public async Task<IResult> UpdateContact(int id, Contact contact, IContactService contactService)
        {
            // TODO: Add request validation

            var result = await contactService.UpdateContactAsync(id, contact);
            if (result.IsFailed)
            {
                var errorResponse = ApiResponse<Contact>.Error(
                    result.Errors.FirstOrDefault()?.Message ?? "An error occurred while updating the contact.");
                return Results.Json(errorResponse, statusCode: 500);
            }

            var apiResponse = ApiResponse<Contact>.Success(result.Value!, "Contact updated successfully.");
            return Results.Ok(apiResponse);
        }

        public async Task<IResult> DeleteContact(int id, IContactService contactService)
        {
            var result = await contactService.DeleteContactAsync(id);
            if (result.IsFailed)
            {
                var errorResponse = ApiResponse<Contact>.Error(
                    result.Errors.FirstOrDefault()?.Message ?? "An error occurred while deleting the contact.");
                return Results.Json(errorResponse, statusCode: 500);
            }

            return Results.NoContent();
        }
    }
}