using System;
using FluentValidation.TestHelper;
using MyApp.Types.Models;
using Ploeh.AutoFixture;
using Xunit;

namespace MyApp.Validation.Tests
{
    public class ToppingValidatorTests : AutoFixtureTest
    {
        private readonly ToppingValidator _sut;

        public ToppingValidatorTests() => _sut = new ToppingValidator();

        [Fact]
        public void Valid()
        {
            var topping = Fixture.Build<Topping>()
                .With(t => t.Price, 1.99d)
                .Create();
            
            Assert.True(_sut.Validate(topping).IsValid);
        }

        [Fact]
        public void GuidNotEmpty()
        {
            var topping = Fixture.Build<Topping>()
                .With(t => t.Id, Guid.Empty)
                .Create();

            _sut.ShouldHaveValidationErrorFor(t => t.Id, topping);
        }

        [Fact]
        public void NameNotNull()
        {
            var topping = Fixture.Build<Topping>()
                .With(t => t.Name, null)
                .Create();

            _sut.ShouldHaveValidationErrorFor(t => t.Name, topping)
                .WithErrorMessage("a topping needs a name");
        }

        [Fact]
        public void NameNotEmpty()
        {
            var topping = Fixture.Build<Topping>()
                .With(t => t.Name, string.Empty)
                .Create();
            
            _sut.ShouldHaveValidationErrorFor(t => t.Name, topping)
                .WithErrorMessage("a topping needs a name");
        }

        [Fact]
        public void PriceMustNotBeNegative()
        {
            var topping = Fixture.Build<Topping>()
                .With(t => t.Price, -1.99d)
                .Create();

            _sut.ShouldHaveValidationErrorFor(t => t.Price, topping)
                .WithErrorMessage("topping price must not be negative");
        }
    }
}