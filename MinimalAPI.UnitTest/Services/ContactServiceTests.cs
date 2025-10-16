using FluentAssertions;
using FluentResults;
using MinimalAPI.Data;
using MinimalAPI.Models;
using MinimalAPI.Services;
using Moq;
using Xunit;

namespace MinimalAPI.UnitTest.Services
{
    public class ContactServiceTests
    {
        private readonly Mock<IContactRepository> _repositoryMock;
        private readonly ContactService _contactService;

        public ContactServiceTests()
        {
            _repositoryMock = new Mock<IContactRepository>();
            _contactService = new ContactService(_repositoryMock.Object);
        }

        #region GetAllContactsAsync Tests

        [Fact]
        public async Task GetAllContactsAsync_ReturnsSuccess_WhenRepositoryReturnsContacts()
        {
            // Arrange
            var expectedContacts = new List<Contact>
            {
                new() { Id = 1, Name = "John Doe", Email = "john@example.com", Phone = "123-456-7890" },
                new() { Id = 2, Name = "Jane Smith", Email = "jane@example.com", Phone = "987-654-3210" }
            };

            _repositoryMock.Setup(x => x.GetAllAsync())
                .ReturnsAsync(expectedContacts);

            // Act
            var result = await _contactService.GetAllContactsAsync();

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().HaveCount(2);
            result.Value.Should().BeEquivalentTo(expectedContacts);
        }

        [Fact]
        public async Task GetAllContactsAsync_ReturnsFailure_WhenRepositoryThrowsException()
        {
            // Arrange
            var exceptionMessage = "Database connection error";
            _repositoryMock.Setup(x => x.GetAllAsync())
                .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _contactService.GetAllContactsAsync();

            // Assert
            result.IsFailed.Should().BeTrue();
            result.Errors.Should().ContainSingle();
            result.Errors.First().Message.Should().Be("An error occurred while retrieving contacts.");
            result.Errors.First().Reasons.First().Message.Should().Be(exceptionMessage);
        }

        #endregion

        #region GetContactByIdAsync Tests

        [Fact]
        public async Task GetContactByIdAsync_ReturnsSuccess_WhenContactExists()
        {
            // Arrange
            var contactId = 1;
            var expectedContact = new Contact
            {
                Id = contactId,
                Name = "John Doe",
                Email = "john@example.com",
                Phone = "123-456-7890"
            };

            _repositoryMock.Setup(x => x.GetByIdAsync(contactId))
                .ReturnsAsync(expectedContact);

            // Act
            var result = await _contactService.GetContactByIdAsync(contactId);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEquivalentTo(expectedContact);
        }

        [Fact]
        public async Task GetContactByIdAsync_ReturnsFailure_WhenContactDoesNotExist()
        {
            // Arrange
            var contactId = 999;
            _repositoryMock.Setup(x => x.GetByIdAsync(contactId))
                .ReturnsAsync((Contact?)null);

            // Act
            var result = await _contactService.GetContactByIdAsync(contactId);

            // Assert
            result.IsFailed.Should().BeTrue();
            result.Errors.Should().ContainSingle();
            result.Errors.First().Message.Should().Be("Contact not found.");
        }

        [Fact]
        public async Task GetContactByIdAsync_ReturnsFailure_WhenRepositoryThrowsException()
        {
            // Arrange
            var contactId = 1;
            var exceptionMessage = "Database connection error";
            _repositoryMock.Setup(x => x.GetByIdAsync(contactId))
                .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _contactService.GetContactByIdAsync(contactId);

            // Assert
            result.IsFailed.Should().BeTrue();
            result.Errors.Should().ContainSingle();
            result.Errors.First().Message.Should().Be("An error occurred while retrieving the contact.");
            result.Errors.First().Reasons.First().Message.Should().Be(exceptionMessage);
        }

        #endregion

        #region CreateContactAsync Tests

        [Fact]
        public async Task CreateContactAsync_ReturnsSuccess_WhenContactIsCreated()
        {
            // Arrange
            var contactToCreate = new Contact
            {
                Name = "New Contact",
                Email = "new@example.com",
                Phone = "555-1234"
            };

            var createdContact = new Contact
            {
                Id = 1,
                Name = contactToCreate.Name,
                Email = contactToCreate.Email,
                Phone = contactToCreate.Phone
            };

            _repositoryMock.Setup(x => x.AddAsync(contactToCreate))
                .ReturnsAsync(createdContact);

            // Act
            var result = await _contactService.CreateContactAsync(contactToCreate);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEquivalentTo(createdContact);
        }

        [Fact]
        public async Task CreateContactAsync_ReturnsFailure_WhenRepositoryThrowsException()
        {
            // Arrange
            var contactToCreate = new Contact
            {
                Name = "New Contact",
                Email = "new@example.com",
                Phone = "555-1234"
            };

            var exceptionMessage = "Database connection error";
            _repositoryMock.Setup(x => x.AddAsync(contactToCreate))
                .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _contactService.CreateContactAsync(contactToCreate);

            // Assert
            result.IsFailed.Should().BeTrue();
            result.Errors.Should().ContainSingle();
            result.Errors.First().Message.Should().Be("An error occurred while creating the contact.");
            result.Errors.First().Reasons.First().Message.Should().Be(exceptionMessage);
        }

        #endregion

        #region UpdateContactAsync Tests

        [Fact]
        public async Task UpdateContactAsync_ReturnsSuccess_WhenContactIsUpdated()
        {
            // Arrange
            var contactId = 1;
            var contactToUpdate = new Contact
            {
                Id = contactId,
                Name = "Updated Contact",
                Email = "updated@example.com",
                Phone = "555-5678"
            };

            _repositoryMock.Setup(x => x.UpdateAsync(contactToUpdate))
                .ReturnsAsync(contactToUpdate);

            // Act
            var result = await _contactService.UpdateContactAsync(contactId, contactToUpdate);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEquivalentTo(contactToUpdate);
        }

        [Fact]
        public async Task UpdateContactAsync_ReturnsFailure_WhenContactDoesNotExist()
        {
            // Arrange
            var contactId = 999;
            var contactToUpdate = new Contact
            {
                Id = contactId,
                Name = "Updated Contact",
                Email = "updated@example.com",
                Phone = "555-5678"
            };

            _repositoryMock.Setup(x => x.UpdateAsync(contactToUpdate))
                .ReturnsAsync((Contact?)null);

            // Act
            var result = await _contactService.UpdateContactAsync(contactId, contactToUpdate);

            // Assert
            result.IsFailed.Should().BeTrue();
            result.Errors.Should().ContainSingle();
            result.Errors.First().Message.Should().Be("Contact non found");
        }

        [Fact]
        public async Task UpdateContactAsync_ReturnsFailure_WhenRepositoryThrowsException()
        {
            // Arrange
            var contactId = 1;
            var contactToUpdate = new Contact
            {
                Id = contactId,
                Name = "Updated Contact",
                Email = "updated@example.com",
                Phone = "555-5678"
            };

            var exceptionMessage = "Database connection error";
            _repositoryMock.Setup(x => x.UpdateAsync(contactToUpdate))
                .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _contactService.UpdateContactAsync(contactId, contactToUpdate);

            // Assert
            result.IsFailed.Should().BeTrue();
            result.Errors.Should().ContainSingle();
            result.Errors.First().Message.Should().Be("An error occurred while updating the contact.");
            result.Errors.First().Reasons.First().Message.Should().Be(exceptionMessage);
        }

        #endregion

        #region DeleteContactAsync Tests

        //[Fact]
        //public async Task DeleteContactAsync_ReturnsSuccess_WhenContactIsDeleted()
        //{
        //    // Arrange
        //    var contactId = 1;
        //    _repositoryMock.Setup(x => x.DeleteAsync(contactId))
        //        .ReturnsAsync(true);

        //    // Act
        //    var result = await _contactService.DeleteContactAsync(contactId);

        //    // Assert
        //    result.IsSuccess.Should().BeTrue();
        //    result.Value.Should().BeTrue();
        //}

        //[Fact]
        //public async Task DeleteContactAsync_ReturnsSuccess_WithFalseValue_WhenContactDoesNotExist()
        //{
        //    // Arrange
        //    var contactId = 999;
        //    _repositoryMock.Setup(x => x.DeleteAsync(contactId))
        //        .ReturnsAsync(false);

        //    // Act
        //    var result = await _contactService.DeleteContactAsync(contactId);

        //    // Assert
        //    result.IsSuccess.Should().BeTrue();
        //    result.Value.Should().BeFalse();
        //}

        //[Fact]
        //public async Task DeleteContactAsync_ReturnsFailure_WhenRepositoryThrowsException()
        //{
        //    // Arrange
        //    var contactId = 1;
        //    var exceptionMessage = "Database connection error";
        //    _repositoryMock.Setup(x => x.DeleteAsync(contactId))
        //        .ThrowsAsync(new Exception(exceptionMessage));

        //    // Act
        //    var result = await _contactService.DeleteContactAsync(contactId);

        //    // Assert
        //    result.IsFailed.Should().BeTrue();
        //    result.Errors.Should().ContainSingle();
        //    result.Errors.First().Message.Should().Be("An error occurred while deleting the contact.");
        //    result.Errors.First().Reasons.First().Message.Should().Be(exceptionMessage);
        //}

        #endregion
    }
}