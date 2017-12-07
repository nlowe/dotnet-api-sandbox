using Ploeh.AutoFixture;

namespace MyApp.Validation.Tests
{
    public class AutoFixtureTest
    {
        protected readonly Fixture Fixture;

        public AutoFixtureTest() => Fixture = new Fixture();
    }
}