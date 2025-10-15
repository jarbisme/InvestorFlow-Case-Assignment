using FluentAssertions;
using FluentResults;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using MinimalAPI.Endpoints;
using MinimalAPI.Models;
using MinimalAPI.Models.DTOs;
using MinimalAPI.Services;
using Moq;
using System.Reflection;
using Xunit;

namespace MinimalAPI.UnitTest.Handlers
{
    public class FundEndpointHandlersTests
    {
        private readonly Mock<IFundService> _fundServiceMock;
        private readonly Mock<IValidator<AddContactToFundRequest>> _addContactValidatorMock;
        private readonly FundEndpointHandlers _handlers;

        public FundEndpointHandlersTests()
        {
            _fundServiceMock = new Mock<IFundService>();
            _addContactValidatorMock = new Mock<IValidator<AddContactToFundRequest>>();
            _handlers = new FundEndpointHandlers(_fundServiceMock.Object, _addContactValidatorMock.Object);
        }

        #region GetAllFunds Tests
        [Fact]
        public async Task GetAllFunds_ReturnsSuccess_WhenServiceReturnsFunds()
        {
            // Arrange
            var funds = new List<Fund>
            {
                new() { Id = 1, Name = "Fund A" },
                new() { Id = 2, Name = "Fund B" }
            };
            
            _fundServiceMock.Setup(x => x.GetAllFundsAsync())
                .ReturnsAsync(Result.Ok(funds));

            // Act
            var result = await _handlers.GetAllFunds();

            // Assert
            var okResult = Assert.IsType<Ok<ApiResponse<object>>>(result);
            var response = okResult.Value;
            
            response.Should().NotBeNull();
            response.Status.Should().Be(ApiResponseStatus.Success);
            response.Message.Should().Be("Funds retrieved successfully.");
            // Since we return anonymous objects, we need to verify through dynamic
            dynamic data = response.Data;
            ((int)data.Count).Should().Be(2);
        }

        [Fact]
        public async Task GetAllFunds_ReturnsError_WhenServiceFails()
        {
            // Arrange
            var errorMessage = "Database connection error";
            _fundServiceMock.Setup(x => x.GetAllFundsAsync())
                .ReturnsAsync(Result.Fail(errorMessage));

            // Act
            var result = await _handlers.GetAllFunds();

            // Assert
            var jsonResult = GetJsonResultFromIResult<ApiResponse<List<Fund>>>(result);
            
            jsonResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
            jsonResult.Value.Status.Should().Be(ApiResponseStatus.Error);
            jsonResult.Value.Message.Should().Be(errorMessage);
        }
        #endregion

        #region GetFundById Tests
        [Fact]
        public async Task GetFundById_ReturnsSuccess_WhenServiceReturnsFund()
        {
            // Arrange
            var fundId = 1;
            var fund = new Fund 
            { 
                Id = fundId, 
                Name = "Fund A",
                Contacts = new List<Contact> 
                {
                    new() { Id = 1, Name = "John Doe", Email = "john@example.com", FundId = fundId }
                }
            };
            
            _fundServiceMock.Setup(x => x.GetFundByIdAsync(fundId))
                .ReturnsAsync(Result.Ok<Fund?>(fund));

            // Act
            var result = await _handlers.GetFundById(fundId);

            // Assert
            var okResult = Assert.IsType<Ok<ApiResponse<object>>>(result);
            var response = okResult.Value;
            
            response.Should().NotBeNull();
            response.Status.Should().Be(ApiResponseStatus.Success);
            response.Message.Should().Be("Fund retrieved successfully.");
            
            // Since we return anonymous object, we need to verify through dynamic
            dynamic data = response.Data;
            ((int)data.Id).Should().Be(fundId);
            ((string)data.Name).Should().Be("Fund A");
            ((int)data.Contacts.Count).Should().Be(1);
        }

        [Fact]
        public async Task GetFundById_ReturnsError_WhenServiceFails()
        {
            // Arrange
            var fundId = 999;
            var errorMessage = "Fund not found";
            _fundServiceMock.Setup(x => x.GetFundByIdAsync(fundId))
                .ReturnsAsync(Result.Fail(errorMessage));

            // Act
            var result = await _handlers.GetFundById(fundId);

            // Assert
            var jsonResult = GetJsonResultFromIResult<ApiResponse<Fund>>(result);
            
            jsonResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
            jsonResult.Value.Status.Should().Be(ApiResponseStatus.Error);
            jsonResult.Value.Message.Should().Be(errorMessage);
        }
        #endregion

        #region AddContactToFund Tests
        [Fact]
        public async Task AddContactToFund_ReturnsSuccess_WhenContactIsAdded()
        {
            // Arrange
            var fundId = 1;
            var request = new AddContactToFundRequest { ContactId = 2 };
            
            // Setup validator to pass validation
            _addContactValidatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());
                
            _fundServiceMock.Setup(s => s.AddContactToFundAsync(fundId, request.ContactId))
                .ReturnsAsync(Result.Ok(true));

            // Act
            var result = await _handlers.AddContactToFund(fundId, request);

            // Assert
            var okResult = Assert.IsType<Ok<ApiResponse>>(result);
            var response = okResult.Value;
            
            response.Status.Should().Be(ApiResponseStatus.Success);
            response.Message.Should().Be("Contact added to fund successfully.");
        }

        [Fact]
        public async Task AddContactToFund_ReturnsBadRequest_WhenValidationFails()
        {
            // Arrange
            var fundId = 1;
            var invalidRequest = new AddContactToFundRequest { ContactId = 0 };
            
            var validationResult = new ValidationResult();
            validationResult.Errors.Add(new ValidationFailure("ContactId", "ContactId must be greater than 0"));
            
            _addContactValidatorMock.Setup(v => v.ValidateAsync(invalidRequest, It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);

            // Act
            var result = await _handlers.AddContactToFund(fundId, invalidRequest);

            // Assert
            var jsonResult = GetJsonResultFromIResult<ApiResponse>(result);
            
            jsonResult.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            jsonResult.Value.Status.Should().Be(ApiResponseStatus.Error);
            jsonResult.Value.Message.Should().Be("Validation failed");
            jsonResult.Value.Errors.Should().Contain("ContactId must be greater than 0");
            
            // Verify service was never called
            _fundServiceMock.Verify(s => s.AddContactToFundAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task AddContactToFund_ReturnsError_WhenServiceFails()
        {
            // Arrange
            var fundId = 1;
            var request = new AddContactToFundRequest { ContactId = 2 };
            var errorMessage = "Contact or fund not found";
            
            // Setup validator to pass validation
            _addContactValidatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());
                
            _fundServiceMock.Setup(s => s.AddContactToFundAsync(fundId, request.ContactId))
                .ReturnsAsync(Result.Fail(errorMessage));

            // Act
            var result = await _handlers.AddContactToFund(fundId, request);

            // Assert
            var jsonResult = GetJsonResultFromIResult<ApiResponse>(result);
            
            jsonResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
            jsonResult.Value.Status.Should().Be(ApiResponseStatus.Error);
            jsonResult.Value.Message.Should().Be(errorMessage);
        }
        #endregion

        #region RemoveContactFromFund Tests
        [Fact]
        public async Task RemoveContactFromFund_ReturnsSuccess_WhenContactIsRemoved()
        {
            // Arrange
            var fundId = 1;
            var contactId = 2;
            
            _fundServiceMock.Setup(s => s.RemoveContactFromFundAsync(fundId, contactId))
                .ReturnsAsync(Result.Ok(true));

            // Act
            var result = await _handlers.RemoveContactFromFund(fundId, contactId);

            // Assert
            var okResult = Assert.IsType<Ok<ApiResponse>>(result);
            var response = okResult.Value;
            
            response.Status.Should().Be(ApiResponseStatus.Success);
            response.Message.Should().Be("Contact removed from fund successfully.");
        }

        [Fact]
        public async Task RemoveContactFromFund_ReturnsError_WhenServiceFails()
        {
            // Arrange
            var fundId = 1;
            var contactId = 2;
            var errorMessage = "Contact or fund not found";
            
            _fundServiceMock.Setup(s => s.RemoveContactFromFundAsync(fundId, contactId))
                .ReturnsAsync(Result.Fail(errorMessage));

            // Act
            var result = await _handlers.RemoveContactFromFund(fundId, contactId);

            // Assert
            var jsonResult = GetJsonResultFromIResult<ApiResponse>(result);
            
            jsonResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
            jsonResult.Value.Status.Should().Be(ApiResponseStatus.Error);
            jsonResult.Value.Message.Should().Be(errorMessage);
        }
        #endregion

        #region Helper Methods
        private JsonHttpResult<T> GetJsonResultFromIResult<T>(IResult result)
        {
            // For results created with Results.Json()
            var resultType = result.GetType();
            
            // Get properties using reflection
            var valueProperty = resultType.GetProperty("Value") ?? 
                throw new InvalidOperationException("Could not find Value property on result");
            var statusCodeProperty = resultType.GetProperty("StatusCode") ?? 
                throw new InvalidOperationException("Could not find StatusCode property on result");
            
            var value = (T)valueProperty.GetValue(result)!;
            var statusCode = (int)statusCodeProperty.GetValue(result)!;

            return new JsonHttpResult<T>(value) { StatusCode = statusCode };
        }

        // Helper class for JSON results
        private class JsonHttpResult<T>
        {
            public T Value { get; set; }
            public int StatusCode { get; set; }

            public JsonHttpResult(T value)
            {
                Value = value;
            }
        }
        #endregion
    }
}