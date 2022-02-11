using FluentValidation;
using OnlineLibrary.Common.DBEntities;
using System;

namespace OnlineLibrary.Common.Validators
{
    public class UserValidator : AbstractValidator<User>
    {
        public UserValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.DateOfBirth).NotEmpty().InclusiveBetween(new DateTime(1900, 1, 1), DateTime.Now);
        }
    }
}
