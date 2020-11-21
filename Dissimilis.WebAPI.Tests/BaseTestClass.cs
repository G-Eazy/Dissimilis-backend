using Dissimilis.WebAPI.xUnit.Setup;
using Xunit;

namespace Dissimilis.WebAPI.xUnit
{
    public class BaseTestClass : IClassFixture<TestServerFixture>
    {
        internal readonly TestServerFixture _testServerFixture;

        public BaseTestClass(TestServerFixture testServerFixture)
        {
            _testServerFixture = testServerFixture;
        }
    }
}
