using FluentValidation;
using OnlineLibrary.Common.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineLibrary.Common.Validators
{
    public class BookValidator : AbstractValidator<Book>
    {
        public BookValidator()
        {
            RuleFor(x => x.Id).NotNull();
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Description).NotEmpty();
            RuleFor(x => x.Authors).NotEmpty();
            RuleFor(x => x.Genre).NotEmpty();
            //RuleFor(x => x.Tags).NotEmpty();
        }
    }
}
