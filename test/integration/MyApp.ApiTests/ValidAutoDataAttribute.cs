using System;
using MyApp.Types.Models;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Xunit2;

namespace MyApp.ApiTests
{
    public class ValidAutoDataAttribute : AutoDataAttribute
    {
        private class MyDataCusomizations : ICustomization
        {
            public void Customize(IFixture fixture)
            {
                fixture.Customize<Topping>(tb => tb
                    .WithAutoProperties()
                    .With(t => t.Price, Math.Round(Math.Sqrt(Math.Pow(fixture.Create<double>(), 2)), 2))
                );
                
                fixture.Customize<Pizza>(pb => pb
                    .WithAutoProperties()
                    .With(p => p.BasePrice, Math.Round(Math.Sqrt(Math.Pow(fixture.Create<double>(), 2)), 2))
                );
            }
        }
        
        public ValidAutoDataAttribute() : base(new Fixture().Customize(new MyDataCusomizations()))
        {
        }
    }
}