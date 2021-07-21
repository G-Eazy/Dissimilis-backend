using Dissimilis.WebAPI.xUnit.Setup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Dissimilis.WebAPI.xUnit.Tests
{
    [Collection("Serial")]
    [CollectionDefinition("Serial", DisableParallelization = true)]
    public class BarTests : BaseTestClass
    {

        public BarTests(TestServerFixture testServerFixture) : base(testServerFixture)
        {
        }

        [Fact]
        public async Task TestCreateNewSongBarWhenCurrentUserIsSongArranger()
        {
            TestServerFixture.ChangeCurrentUserId(DeepPurpleFanUser.Id);
        }

        [Fact]
        public async Task TestCreateNewSongBarWhenCurrentUserIsNotSongArranger()
        {

        }

    }
}
