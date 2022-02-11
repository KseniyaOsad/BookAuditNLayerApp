using FluentValidation;
using OnlineLibrary.API.Model;

namespace OnlineLibrary.API.Validator
{
    public class ReservationModelValidator : AbstractValidator<ReservationModel>
    {
        public ReservationModelValidator()
        {
            RuleFor(x => x.UserId).GreaterThan(0);
            RuleFor(x => x.BookId).GreaterThan(0);
        }
    }
}
