using FluentValidation;
using OnlineLibrary.API.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineLibrary.API.Validator
{
    public class CreateUserValidator : AbstractValidator<CreateUser>
    {
        public CreateUserValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Day).NotEmpty().InclusiveBetween(1,31);
            RuleFor(x => x.Month).NotEmpty().InclusiveBetween(1, 12);
            RuleFor(x => x.Year).NotEmpty().GreaterThan(1900);
        }
    }
}
