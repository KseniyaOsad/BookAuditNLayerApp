using FluentValidation;
using OnlineLibrary.Common.DBEntities;

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
