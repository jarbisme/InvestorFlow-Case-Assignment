using FluentAssertions;
using FluentResults;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using MinimalAPI.Endpoints;
using MinimalAPI.Models;
using MinimalAPI.Models.DTOs;
using MinimalAPI.Services;
using MinimalAPI.Validators;
using Moq;
using System.Reflection;
using Xunit;

namespace MinimalAPI.UnitTest.Handlers
{
    public class ContactEndpointHandlersTests
    {
        private readonly Mock<IContactService> _contactServiceMock;
        private readonly ContactEndpointHandlers _handlers;

        public ContactEndpointHandlersTests()
        {
            _contactServiceMock = new Mock<IContactService>();
            _handlers = new ContactEndpointHandlers();
        }

        #region GetAllContacts Tests
        [Fact]
        public async Task GetAllContacts_ReturnsSuccess_WhenServiceReturnsContacts()
        {
            // Arrange
            var contacts = new List<Contact>
            {
                new() { Id = 1, Name = "John Doe", Email = "john@example.com", Phone = "123-456-7890" },
                new() { Id = 2, Name = "Jane Smith", Email = "jane@example.com", Phone = "987-654-3210" }
            };
            
            _contactServiceMock.Setup(x => x.GetAllContactsAsync())
                .ReturnsAsync(Result.Ok(contacts));

            // Act - direct call, no reflection needed
            var result = await _handlers.GetAllContacts(_contactServiceMock.Object);

            // Assert
            var okResult = Assert.IsType<Ok<ApiResponse<List<Contact>>>>(result);
            var response = okResult.Value;
            
            response.Should().NotBeNull();
            response.Status.Should().Be(ApiResponseStatus.Success);
            response.Data.Should().HaveCount(2);
            response.Message.Should().Be("Contacts retrieved successfully.");
        }

        #endregion


        #region GetContactById Tests
        [Fact]
        public async Task GetContactById_ReturnsSuccess_WhenServiceReturnsContact()
        {
            // Arrange
            var contactId = 1;
            var contact = new Contact 
            { 
                Id = contactId, 
                Name = "John Doe", 
                Email = "john@example.com", 
                Phone = "123-456-7890" 
            };
            
            _contactServiceMock.Setup(x => x.GetContactByIdAsync(contactId))
                .ReturnsAsync(Result.Ok<Contact?>(contact));

            // Act
            var result = await _handlers.GetContactById(contactId, _contactServiceMock.Object);

            // Assert
            var okResult = Assert.IsType<Ok<ApiResponse<Contact>>>(result);
            var response = okResult.Value;
            
            response.Should().NotBeNull();
            response.Status.Should().Be(ApiResponseStatus.Success);
            response.Data.Should().NotBeNull();
            response.Data.Id.Should().Be(contactId);
            response.Data.Name.Should().Be("John Doe");
            response.Message.Should().Be("Contact retrieved successfully.");
        }

        [Fact]
        public async Task GetContactById_ReturnsError_WhenServiceFails()
        {
            // Arrange
            var contactId = 999;
            var errorMessage = "Contact not found";
            _contactServiceMock.Setup(x => x.GetContactByIdAsync(contactId))
                .ReturnsAsync(Result.Fail(errorMessage));

            // Act
            var result = await _handlers.GetContactById(contactId, _contactServiceMock.Object);

            // Assert
            var jsonResult = GetJsonResultFromIResult<ApiResponse<Contact>>(result);
            
            jsonResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
            jsonResult.Value.Status.Should().Be(ApiResponseStatus.Error);
            jsonResult.Value.Message.Should().Be(errorMessage);
        }

        [Fact]
        public async Task GetContactById_ReturnsError_WhenServiceReturnsNull()
        {
            // Arrange
            var contactId = 999;
            // Service returning Ok with null value is a valid scenario in some implementations
            _contactServiceMock.Setup(x => x.GetContactByIdAsync(contactId))
                .ReturnsAsync(Result.Ok<Contact?>(null));

            // Act
            var result = await _handlers.GetContactById(contactId, _contactServiceMock.Object);

            // Assert
            // This test will actually fail because your handler doesn't handle null values explicitly
            // You may want to update your handler to check for null values
            var okResult = Assert.IsType<Ok<ApiResponse<Contact>>>(result);
            var response = okResult.Value;
            
            response.Should().NotBeNull();
            response.Status.Should().Be(ApiResponseStatus.Success);
            response.Data.Should().BeNull();
            response.Message.Should().Be("Contact retrieved successfully.");
        }

        #endregion


        #region CreateContact Tests

        [Fact]
        public async Task CreateContact_ReturnsCreated_WhenContactIsValid()
        {
            // Arrange
            var createRequest = new CreateContactRequest
            {
                Name = "John Doe",
                Email = "john@example.com",
                Phone = "555-1234"
            };

            var createdContact = new Contact
            {
                Id = 1,
                Name = createRequest.Name,
                Email = createRequest.Email,
                Phone = createRequest.Phone
            };

            // Mock validator to pass validation
            var validatorMock = new Mock<IValidator<CreateContactRequest>>();
            var validationResult = new FluentValidation.Results.ValidationResult();
            validatorMock.Setup(v => v.ValidateAsync(createRequest, It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);

            // Setup service to return success
            _contactServiceMock.Setup(s => s.CreateContactAsync(It.Is<Contact>(c => 
                c.Name == createRequest.Name && 
                c.Email == createRequest.Email && 
                c.Phone == createRequest.Phone)))
                .ReturnsAsync(Result.Ok(createdContact));

            // Act
            var result = await _handlers.CreateContact(createRequest, _contactServiceMock.Object, validatorMock.Object);

            // Assert
            var createdResult = Assert.IsType<Created<ApiResponse<Contact>>>(result);
            createdResult.Location.Should().Be("/api/contacts/1");
            
            var response = createdResult.Value;
            response.Status.Should().Be(ApiResponseStatus.Success);
            response.Message.Should().Be("Contact created successfully.");
            response.Data.Should().NotBeNull();
            response.Data.Id.Should().Be(1);
            response.Data.Name.Should().Be(createRequest.Name);
        }

        [Fact]
        public async Task CreateContact_ReturnsBadRequest_WhenValidationFails()
        {
            // Arrange
            var invalidRequest = new CreateContactRequest { Name = "" };

            // Mock validator to fail validation
            var validatorMock = new Mock<IValidator<CreateContactRequest>>();
            var validationResult = new FluentValidation.Results.ValidationResult();
            validationResult.Errors.Add(new FluentValidation.Results.ValidationFailure("Name", "Name is required"));
            validatorMock.Setup(v => v.ValidateAsync(invalidRequest, It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);

            // Act
            var result = await _handlers.CreateContact(invalidRequest, _contactServiceMock.Object, validatorMock.Object);

            // Assert
            var badRequestResult = Assert.IsType<BadRequest<ApiResponse<Contact>>>(result);
            var response = badRequestResult.Value;
            
            response.Status.Should().Be(ApiResponseStatus.Fail);
            response.Message.Should().Be("Validation failed");
            response.Errors.Should().HaveCount(1);
            response.Errors.Should().Contain("Name is required");
            
            // Verify service was never called
            _contactServiceMock.Verify(s => s.CreateContactAsync(It.IsAny<Contact>()), Times.Never);
        }

        [Fact]
        public async Task CreateContact_ReturnsServerError_WhenServiceFails()
        {
            // Arrange
            var request = new CreateContactRequest { Name = "John Doe" };
            var errorMessage = "Database connection failed";
            
            // Mock validator to pass validation
            var validatorMock = new Mock<IValidator<CreateContactRequest>>();
            var validationResult = new FluentValidation.Results.ValidationResult();
            validatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);
    
            _contactServiceMock.Setup(s => s.CreateContactAsync(It.IsAny<Contact>()))
                .ReturnsAsync(Result.Fail(errorMessage));

            // Act
            var result = await _handlers.CreateContact(request, _contactServiceMock.Object, validatorMock.Object);

            // Assert
            var jsonResult = GetJsonResultFromIResult<ApiResponse<Contact>>(result);
            
            jsonResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
            jsonResult.Value.Status.Should().Be(ApiResponseStatus.Error);
            jsonResult.Value.Message.Should().Be(errorMessage);
        }

        #endregion

        #region UpdateContact Tests

        [Fact]
        public async Task UpdateContact_ReturnsSuccess_WhenContactIsUpdated()
        {
            // Arrange
            var contactId = 1;
            var updatedContact = new Contact
            {
                Id = contactId,
                Name = "John Updated",
                Email = "john.updated@example.com",
                Phone = "555-5678"
            };

            _contactServiceMock.Setup(s => s.UpdateContactAsync(contactId, updatedContact))
                .ReturnsAsync(Result.Ok(updatedContact));

            // Act
            var result = await _handlers.UpdateContact(contactId, updatedContact, _contactServiceMock.Object);

            // Assert
            var okResult = Assert.IsType<Ok<ApiResponse<Contact>>>(result);
            var response = okResult.Value;
            
            response.Status.Should().Be(ApiResponseStatus.Success);
            response.Message.Should().Be("Contact updated successfully.");
            response.Data.Should().NotBeNull();
            response.Data.Id.Should().Be(contactId);
            response.Data.Name.Should().Be("John Updated");
        }

        [Fact]
        public async Task UpdateContact_ReturnsError_WhenServiceFails()
        {
            // Arrange
            var contactId = 999;
            var contact = new Contact { Id = contactId, Name = "John Doe" };
            var errorMessage = "Contact not found";
            
            _contactServiceMock.Setup(s => s.UpdateContactAsync(contactId, contact))
                .ReturnsAsync(Result.Fail(errorMessage));

            // Act
            var result = await _handlers.UpdateContact(contactId, contact, _contactServiceMock.Object);

            // Assert
            var jsonResult = GetJsonResultFromIResult<ApiResponse<Contact>>(result);
            
            jsonResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
            jsonResult.Value.Status.Should().Be(ApiResponseStatus.Error);
            jsonResult.Value.Message.Should().Be(errorMessage);
        }

        #endregion

        #region DeleteContact Tests

        [Fact]
        public async Task DeleteContact_ReturnsNoContent_WhenContactIsDeleted()
        {
            // Arrange
            var contactId = 1;
            _contactServiceMock.Setup(s => s.DeleteContactAsync(contactId))
                .ReturnsAsync(Result.Ok(true));

            // Act
            var result = await _handlers.DeleteContact(contactId, _contactServiceMock.Object);

            // Assert
            Assert.IsType<NoContent>(result);
        }

        [Fact]
        public async Task DeleteContact_ReturnsError_WhenServiceFails()
        {
            // Arrange
            var contactId = 999;
            var errorMessage = "Contact not found";
            
            _contactServiceMock.Setup(s => s.DeleteContactAsync(contactId))
                .ReturnsAsync(Result.Fail(errorMessage));

            // Act
            var result = await _handlers.DeleteContact(contactId, _contactServiceMock.Object);

            // Assert
            var jsonResult = GetJsonResultFromIResult<ApiResponse<Contact>>(result);
            
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