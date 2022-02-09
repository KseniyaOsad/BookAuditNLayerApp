using FluentValidation;
using OnlineLibrary.Common.DBEntities;

namespace OnlineLibrary.Common.Validators
{
    public class UserValidator : AbstractValidator<User>
    {
        public UserValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.DateOfBirth).NotEmpty();
        }
    }
}
