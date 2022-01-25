using FluentValidation;
using OnlineLibrary.Common.Entities;

namespace OnlineLibrary.Common.Validators
{
    public class TagValidator : AbstractValidator<Tag>
    {
        public TagValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
        }
    }
}
