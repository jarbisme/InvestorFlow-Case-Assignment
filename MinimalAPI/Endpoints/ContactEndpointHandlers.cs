using FluentValidation;
using MinimalAPI.Models;
using MinimalAPI.Models.DTOs;
using MinimalAPI.Services;
using MinimalAPI.Validators;

namespace MinimalAPI.Endpoints
{
    /// <summary>
    /// Handler class for Contact-related endpoint operations.
    /// </summary>
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

        /// <summary>
        /// Handler to get all contacts.
        /// </summary>
        /// <returns></returns>
        public async Task<IResult> GetAllContacts()
        {
            var result = await _contactService.GetAllContactsAsync();

            if (result.IsFailed)
            {
                var error = result.Errors.FirstOrDefault();
                bool isServerError = error?.Metadata.ContainsKey("IsServerError") == true;
                
                int statusCode = isServerError ? 
                    StatusCodes.Status500InternalServerError : 
                    StatusCodes.Status400BadRequest;
                
                var errorResponse = ApiResponse<List<Contact>>.Error(
                    error?.Message ?? "An error occurred while retrieving contacts.");
                return Results.Json(errorResponse, statusCode: statusCode);
            }

            var apiResponse = ApiResponse<List<Contact>>.Success(result.Value!, "Contacts retrieved successfully.");
            return Results.Ok(apiResponse);
        }

        /// <summary>
        /// Handler to get a contact by ID.
        /// </summary>
        /// <param name="id">The unique identifier of the contact</param>
        public async Task<IResult> GetContactById(int id)
        {
            var result = await _contactService.GetContactByIdAsync(id);

            if (result.IsFailed)
            {
                var error = result.Errors.FirstOrDefault();
                bool isServerError = error?.Metadata.ContainsKey("IsServerError") == true;
                
                int statusCode = isServerError ? 
                    StatusCodes.Status500InternalServerError : 
                    StatusCodes.Status400BadRequest;
                
                var errorResponse = ApiResponse<Contact>.Error(
                    error?.Message ?? "An error occurred while retrieving the contact.");
                return Results.Json(errorResponse, statusCode: statusCode);
            }

            var apiResponse = ApiResponse<Contact>.Success(result.Value!, "Contact retrieved successfully.");
            return Results.Ok(apiResponse);
        }

        /// <summary>
        /// Handler to create a new contact.
        /// </summary>
        /// <param name="request">The request containing contact information</param>
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
                var error = result.Errors.FirstOrDefault();
                bool isServerError = error?.Metadata.ContainsKey("IsServerError") == true;
                
                int statusCode = isServerError ? 
                    StatusCodes.Status500InternalServerError : 
                    StatusCodes.Status400BadRequest;
                
                var errorResponse = ApiResponse<Contact>.Error(
                    error?.Message ?? "An error occurred while creating the contact.");
                return Results.Json(errorResponse, statusCode: statusCode);
            }

            var successResponse = ApiResponse<Contact>.Success(
                result.Value!,
                "Contact created successfully.");
            return Results.Created($"/api/contacts/{result.Value!.Id}", successResponse);
        }

        /// <summary>
        /// Handler to update an existing contact.
        /// </summary>
        /// <param name="id">The ID of the contact to update</param>
        /// <param name="request">The request containing updated contact information</param>
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
                var error = result.Errors.FirstOrDefault();
                bool isServerError = error?.Metadata.ContainsKey("IsServerError") == true;
                
                int statusCode = isServerError ? 
                    StatusCodes.Status500InternalServerError : 
                    StatusCodes.Status400BadRequest;
                
                var errorResponse = ApiResponse<Contact>.Error(
                    error?.Message ?? "An error occurred while updating the contact.");
                return Results.Json(errorResponse, statusCode: statusCode);
            }

            var apiResponse = ApiResponse<Contact>.Success(result.Value!, "Contact updated successfully.");
            return Results.Ok(apiResponse);
        }

        /// <summary>
        /// Handler to delete a contact by ID.
        /// </summary>
        /// <param name="id">The unique identifier of the contact to delete</param>
        public async Task<IResult> DeleteContact(int id)
        {
            var result = await _contactService.DeleteContactAsync(id);
            if (result.IsFailed)
            {
                var error = result.Errors.FirstOrDefault();
                bool isServerError = error?.Metadata.ContainsKey("IsServerError") == true;
                
                int statusCode = isServerError ? 
                    StatusCodes.Status500InternalServerError : 
                    StatusCodes.Status400BadRequest;
                
                var errorResponse = ApiResponse<Contact>.Error(
                    error?.Message ?? "An error occurred while deleting the contact.");
                return Results.Json(errorResponse, statusCode: statusCode);
            }

            return Results.NoContent();
        }
    }
}