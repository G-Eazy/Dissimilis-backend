using Shouldly;
using Xunit;
using Dissimilis.WebAPI.Extensions.Models;
using static Dissimilis.WebAPI.xUnit.Extensions;

namespace Dissimilis.WebAPI.xUnit.Tests
{
    [Collection("Serial")]
    [CollectionDefinition("Serial", DisableParallelization = true)]
    public class SongExtensionTests
    {
        [Fact]
        public void TestMaxBarPositionFunction()
        {
            // Expect 4 parts, but starting at 0
            NewSong(2, 4).GetMaxBarPosition().ShouldBe(4 - 1);

            // Expect 6 parts, but starting at 0
            NewSong(3, 4).GetMaxBarPosition().ShouldBe(6 - 1);

            // Expect 8 parts, but starting at 0 
            NewSong(4, 4).GetMaxBarPosition().ShouldBe(8 - 1);

            // Expect 12 parts, but starting at 0
            NewSong(6, 4).GetMaxBarPosition().ShouldBe(12 - 1);

            // Expect 6 parts, but starting at 0
            NewSong(6, 8).GetMaxBarPosition().ShouldBe(6 - 1);
        }
    }
}
