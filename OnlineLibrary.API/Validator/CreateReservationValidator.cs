using FluentValidation;
using OnlineLibrary.API.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineLibrary.API.Validator
{
    public class CreateReservationValidator : AbstractValidator<ReservationModel>
    {
        public CreateReservationValidator()
        {
            RuleFor(x => x.UserId).GreaterThan(0);
            RuleFor(x => x.BookId).GreaterThan(0);
        }
    }
}
