using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Dissimilis.DbContext.Models.Song;
using Dissimilis.WebAPI.Controllers.BoSong;
using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsIn;
using Dissimilis.WebAPI.Extensions.Models;
using Dissimilis.WebAPI.xUnit.Setup;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Dissimilis.WebAPI.xUnit.Tests
{
    [Collection("Serial")]
    public class SongTests : BaseTestClass
    {
        public const string DefaultTestSongTitle = "TestSong";

        public SongTests(TestServerFixture testServerFixture) : base(testServerFixture)
        {
        }

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

        [Fact]
        public async Task TestNewSongSave()
        {
            var mediator = _testServerFixture.GetServiceProvider().GetService<IMediator>();

            var createSongDto = newCreateSongDto(4,4);

            var updatedSongCommandDto = await mediator.Send(new CreateSongCommand(createSongDto));
            var songDto = await mediator.Send(new QuerySongById(updatedSongCommandDto.SongId));

            createSongDto.Title.ShouldBe(songDto.Title, "Title not the same after creating");
            createSongDto.Numerator.ShouldBe(songDto.Numerator, "Numerator not the same after creating");
            createSongDto.Denominator.ShouldBe(songDto.Denominator, "Denominator not the same after creating");

            Assert.Single(songDto.Voices);

            songDto.Voices.FirstOrDefault()?.Bars.FirstOrDefault()?.ChordsAndNotes.FirstOrDefault()?.Length.ShouldBe(8, "Length of standard pause note is not correct");
        }

        internal static CreateSongDto newCreateSongDto(int numerator = 4, int denominator = 4, string title = DefaultTestSongTitle)
        {
            return new CreateSongDto()
            {
                Title = title,
                Numerator = numerator,
                Denominator = denominator,
            };
        }

        internal static Song NewSong(int numerator, int denominator)
        {
            return new Song()
            {
                Numerator = numerator,
                Denominator = denominator
            };
        }


    }


}

