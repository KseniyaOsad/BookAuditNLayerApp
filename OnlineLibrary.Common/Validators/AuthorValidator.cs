using FluentValidation;
using OnlineLibrary.Common.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineLibrary.Common.Validators
{
    public class AuthorValidator : AbstractValidator<Author>
    {
        public AuthorValidator()
        {
            RuleFor(x => x.Id).NotNull();
            RuleFor(x => x.Name).NotEmpty();
        }
    }
}
