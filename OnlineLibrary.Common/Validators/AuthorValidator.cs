﻿using FluentValidation;
using OnlineLibrary.Common.Entities;

namespace OnlineLibrary.Common.Validators
{
    public class AuthorValidator : AbstractValidator<Author>
    {
        public AuthorValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
        }
    }
}
