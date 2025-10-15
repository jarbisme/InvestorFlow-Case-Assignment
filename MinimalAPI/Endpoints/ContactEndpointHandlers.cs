using FluentValidation;
using MinimalAPI.Models;
using MinimalAPI.Models.DTOs;
using MinimalAPI.Services;
using MinimalAPI.Validators;

namespace MinimalAPI.Endpoints
{
    public class ContactEndpointHandlers
    {
        private readonly IContactService _contactService;
        private readonly IValidator<CreateContactRequest> _createValidator;
        private readonly IValidator<UpdateContactRequest> _updateValidator;

        public ContactEndpointHandlers(
            IContactService contactService,
            IValidator<CreateContactRequest> createValidator,
            IValidator<UpdateContactRequest> updateValidator)
        {
            _contactService = contactService;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

         
        public async Task<IResult> GetAllContacts()
        {
            var result = await _contactService.GetAllContactsAsync();

            if (result.IsFailed)
            {
                var errorResponse = ApiResponse<List<Contact>>.Error(
                    result.Errors.FirstOrDefault()?.Message ?? "An error occurred while retrieving contacts.");
                return Results.Json(errorResponse, statusCode: 500);
            }

            var apiResponse = ApiResponse<List<Contact>>.Success(result.Value!, "Contacts retrieved successfully.");
            return Results.Ok(apiResponse);
        }

        public async Task<IResult> GetContactById(int id)
        {
            var result = await _contactService.GetContactByIdAsync(id);

            if (result.IsFailed)
            {
                var errorResponse = ApiResponse<Contact>.Error(
                    result.Errors.FirstOrDefault()?.Message ?? "An error occurred while retrieving the contact.");
                return Results.Json(errorResponse, statusCode: 500);
            }

            var apiResponse = ApiResponse<Contact>.Success(result.Value!, "Contact retrieved successfully.");
            return Results.Ok(apiResponse);
        }

        public async Task<IResult> CreateContact(CreateContactRequest request)
        {
            // Validate request
            var validationResult = await _createValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                var failResponse = ApiResponse<Contact>.Fail("Validation failed", errors);
                return Results.BadRequest(failResponse);
            }

            // Map DTO to entity (ignoring Id)
            var contact = new Contact
            {
                Name = request.Name,
                Email = request.Email,
                Phone = request.Phone
            };

            var result = await _contactService.CreateContactAsync(contact);

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

        public async Task<IResult> UpdateContact(int id, UpdateContactRequest request)
        {
            // Validate request
            var validationResult = await _updateValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                var failResponse = ApiResponse<Contact>.Fail("Validation failed", errors);
                return Results.BadRequest(failResponse);
            }

            // Map DTO to entity for update
            var contact = new Contact
            {
                Id = id, // Include the ID from the route parameter
                Name = request.Name,
                Email = request.Email,
                Phone = request.Phone
            };

            var result = await _contactService.UpdateContactAsync(id, contact);
            if (result.IsFailed)
            {
                var errorResponse = ApiResponse<Contact>.Error(
                    result.Errors.FirstOrDefault()?.Message ?? "An error occurred while updating the contact.");
                return Results.Json(errorResponse, statusCode: 500);
            }

            var apiResponse = ApiResponse<Contact>.Success(result.Value!, "Contact updated successfully.");
            return Results.Ok(apiResponse);
        }

        public async Task<IResult> DeleteContact(int id)
        {
            var result = await _contactService.DeleteContactAsync(id);
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