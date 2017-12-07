using System;
using FluentValidation;
using MyApp.Types.Models;

namespace MyApp.Validation
{
    public class ToppingValidator : AbstractValidator<Topping>
    {
        public ToppingValidator()
        {
            RuleFor(t => t.Id).Must(id => id != Guid.Empty);
            RuleFor(t => t.Name).NotEmpty().WithMessage("a topping needs a name");
            RuleFor(t => t.Price).GreaterThanOrEqualTo(0.0d).WithMessage("topping price must not be negative");
        }
    }
}