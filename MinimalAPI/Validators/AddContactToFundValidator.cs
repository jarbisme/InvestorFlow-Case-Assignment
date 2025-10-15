using FluentValidation;
using MinimalAPI.Models.DTOs;

namespace MinimalAPI.Validators
{
    /// <summary>
    /// Validator for AddContactToFundRequest to ensure ContactId is valid.
    /// </summary>
    public class AddContactToFundValidator : AbstractValidator<AddContactToFundRequest>
    {
        public AddContactToFundValidator()
        {
            RuleFor(r => r.ContactId)
                .NotEmpty().WithMessage("ContactId is required")
                .GreaterThan(0).WithMessage("ContactId must be greater than 0");
        }
    }
}
