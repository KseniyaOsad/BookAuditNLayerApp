using FluentValidation;
using OnlineLibrary.API.Model;

namespace OnlineLibrary.API.Validator
{
    public class CreateBookValidator : AbstractValidator<CreateBook>
    {
        public CreateBookValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Description).NotEmpty();
            RuleFor(x => x.Authors).NotEmpty();
            RuleFor(x => x.Genre).NotEmpty().IsInEnum();
        }
    }
}
