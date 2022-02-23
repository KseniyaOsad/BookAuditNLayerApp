using FluentValidation;
using OnlineLibrary.Common.DBEntities;

namespace OnlineLibrary.Common.Validators
{
    public class BookValidator : AbstractValidator<Book>
    {
        public BookValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Description).NotEmpty();
            RuleFor(x => x.Authors).NotEmpty();
            RuleFor(x => x.Tags).NotEmpty();
            RuleFor(x => x.Genre).NotEmpty().IsInEnum();
        }
    }
}
