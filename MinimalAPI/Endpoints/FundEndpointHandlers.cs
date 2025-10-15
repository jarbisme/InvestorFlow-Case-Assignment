using FluentValidation;
using MinimalAPI.Models;
using MinimalAPI.Models.DTOs;
using MinimalAPI.Services;

namespace MinimalAPI.Endpoints
{
    public class FundEndpointHandlers
    {
        private readonly IFundService _fundService;
        private readonly IValidator<AddContactToFundRequest> _addContactValidator;

        public FundEndpointHandlers(IFundService fundService, IValidator<AddContactToFundRequest> addContactValidator)
        {
            _fundService = fundService;
            _addContactValidator = addContactValidator;
        }

        public async Task<IResult> GetAllFunds()
        {
            var result = await _fundService.GetAllFundsAsync();
            if (result.IsFailed)
            {
                var errorResponse = ApiResponse<List<Fund>>.Error(
                    result.Errors.FirstOrDefault()?.Message ?? "An error occurred while retrieving funds.");
                return Results.Json(errorResponse, statusCode: 500);
            }

            // Create anonymous objects for each fund
            var fundsDto = result.Value!.Select(fund => new {
                fund.Id,
                fund.Name
            });

            var apiResponse = ApiResponse<object>.Success(fundsDto, "Funds retrieved successfully.");
            return Results.Ok(apiResponse);
        }

        public async Task<IResult> GetFundById(int id)
        {
            var result = await _fundService.GetFundByIdAsync(id);
            if (result.IsFailed)
            {
                var errorResponse = ApiResponse<Fund>.Error(
                    result.Errors.FirstOrDefault()?.Message ?? "An error occurred while retrieving the fund.");
                return Results.Json(errorResponse, statusCode: 500);
            }

            // Create anonymous object without circular references
            var fundDto = new
            {
                result.Value!.Id,
                result.Value.Name,
                Contacts = result.Value.Contacts.Select(c => new {
                    c.Id,
                    c.Name,
                    c.Email,
                    c.Phone,
                    c.FundId
                })
            };

            var apiResponse = ApiResponse<object>.Success(fundDto, "Fund retrieved successfully.");
            return Results.Ok(apiResponse);
        }

        public async Task<IResult> AddContactToFund(int fundId, AddContactToFundRequest request)
        {
            // Validate request
            var validationResult = await _addContactValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                var errorResponse = ApiResponse.Error("Validation failed", errors);
                return Results.Json(errorResponse, statusCode: 400);
            }

            var result = await _fundService.AddContactToFundAsync(fundId, request.ContactId);
            if (result.IsFailed)
            {
                var errorResponse = ApiResponse.Error(
                    result.Errors.FirstOrDefault()?.Message ?? "An error occurred while adding contact to fund.");
                return Results.Json(errorResponse, statusCode: 500);
            }

            var successResponse = ApiResponse.SuccessNoData("Contact added to fund successfully.");
            return Results.Ok(successResponse);
        }

        public async Task<IResult> RemoveContactFromFund(int fundId, int contactId)
        {
            var result = await _fundService.RemoveContactFromFundAsync(fundId, contactId);
            if (result.IsFailed)
            {
                var errorResponse = ApiResponse.Error(
                    result.Errors.FirstOrDefault()?.Message ?? "An error occurred while removing contact from fund.");
                return Results.Json(errorResponse, statusCode: 500);
            }
            var successResponse = ApiResponse.SuccessNoData("Contact removed from fund successfully.");
            return Results.Ok(successResponse);
        }
    }
}
