using FluentValidation;
using MinimalAPI.Models.DTOs;

namespace MinimalAPI.Validators
{
    public class UpdateContactValidator : AbstractValidator<UpdateContactRequest>
    {
        public UpdateContactValidator()
        {
            RuleFor(c => c.Name)
                .NotEmpty().WithMessage("Name is required")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters");

            RuleFor(c => c.Email)
                .EmailAddress().When(c => !string.IsNullOrEmpty(c.Email))
                .WithMessage("A valid email address is required");

            RuleFor(c => c.Phone)
                .Matches(@"^\+?[0-9\s\-\(\)]+$").When(c => !string.IsNullOrEmpty(c.Phone))
                .WithMessage("A valid phone number is required");
        }
    }
}