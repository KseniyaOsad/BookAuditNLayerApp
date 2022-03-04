using FluentValidation;
using OnlineLibrary.DAL.DTO;

namespace OnlineLibrary.DAL.DTOValidators
{
    public class BookDTOValidator : AbstractValidator<BookDTO>
    {
        public BookDTOValidator()
        {
            RuleFor(x => x.Name).NotEmpty().When(x=>x.Name != null);
            RuleFor(x => x.Description).NotEmpty().When(x => x.Description != null);
            RuleFor(x => x.Genre).NotEmpty().IsInEnum().When(x => x.Genre != null);
            RuleFor(x => x.Authors).NotEmpty();
            RuleFor(x => x.Tags).NotEmpty();
        }
    }
}
