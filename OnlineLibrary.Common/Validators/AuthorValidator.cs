using FluentValidation;
using OnlineLibrary.Common.DBEntities;

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
