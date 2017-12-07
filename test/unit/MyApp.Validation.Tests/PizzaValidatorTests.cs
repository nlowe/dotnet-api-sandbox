using System;
using FluentValidation.TestHelper;
using MyApp.Types.Models;
using Ploeh.AutoFixture;
using Xunit;

namespace MyApp.Validation.Tests
{
    public class PizzaValidatorTests : AutoFixtureTest
    {
        private readonly PizzaValidator _sut;

        public PizzaValidatorTests() => _sut = new PizzaValidator(new ToppingValidator());

        [Fact]
        public void Valid()
        {
            var pizza = Fixture.Build<Pizza>()
                .With(p => p.BasePrice, 999)
                .Create();
            
            Assert.True(_sut.Validate(pizza).IsValid);
        }
        
        [Fact]
        public void GuidNotEmpty()
        {
            var pizza = Fixture.Build<Pizza>()
                .With(p => p.Id, Guid.Empty)
                .With(p => p.BasePrice, 999)
                .Create();

            _sut.ShouldHaveValidationErrorFor(p => p.Id, pizza);
        }

        [Fact]
        public void NameNotNull()
        {
            var pizza = Fixture.Build<Pizza>()
                .With(p => p.Name, null)
                .Create();
            
            _sut.ShouldHaveValidationErrorFor(p => p.Name, pizza)
                .WithErrorMessage("a pizza needs a name");
        }

        [Fact]
        public void NameNotEmpty()
        {
            var pizza = Fixture.Build<Pizza>()
                .With(p => p.Name, string.Empty)
                .Create();

            _sut.ShouldHaveValidationErrorFor(p => p.Name, pizza)
                .WithErrorMessage("a pizza needs a name");
        }

        [Fact]
        public void BasePriceCannotBeNegative()
        {
            var pizza = Fixture.Build<Pizza>()
                .With(p => p.BasePrice, -1.0d)
                .Create();

            _sut.ShouldHaveValidationErrorFor(p => p.BasePrice, pizza)
                .WithErrorMessage("the base price must not be negative");
        }

        [Fact]
        public void UsesToppingValidatorForToppings() =>
            _sut.ShouldHaveChildValidator(p => p.Toppings, typeof(ToppingValidator));
    }
}
