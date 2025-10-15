using FluentValidation;
using MinimalAPI.Models.DTOs;

namespace MinimalAPI.Validators
{
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
