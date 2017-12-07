using System;
using FluentValidation;
using MyApp.Types.Models;

namespace MyApp.Validation
{
    public class PizzaValidator : AbstractValidator<Pizza>
    {
        public PizzaValidator(IValidator<Topping> toppingValidator)
        {
            RuleFor(p => p.Id).Must(id => id != Guid.Empty);
            RuleFor(p => p.Name).NotEmpty().WithMessage("a pizza needs a name");
            RuleFor(p => p.BasePrice).GreaterThanOrEqualTo(0.0d).WithMessage("the base price must not be negative");
            RuleFor(p => p.Toppings).SetCollectionValidator(toppingValidator);
        }
    }
}