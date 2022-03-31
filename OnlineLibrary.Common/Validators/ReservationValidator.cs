using FluentValidation;
using OnlineLibrary.Common.DBEntities;

namespace OnlineLibrary.Common.Validators
{
    public class ReservationValidator : AbstractValidator<Reservation>
    {
        public ReservationValidator()
        {
            RuleFor(x => x.UserId).GreaterThan(0);
            RuleFor(x => x.BookId).GreaterThan(0);
            RuleFor(x => x.ReservationDate).NotNull();
            RuleFor(x => x.ReturnDate).GreaterThanOrEqualTo(x => x.ReservationDate).When(x => x.ReturnDate != null);
        }
    }
}
