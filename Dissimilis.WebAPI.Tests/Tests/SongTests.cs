using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Dissimilis.DbContext.Models.Song;
using Dissimilis.WebAPI.Controllers.BoSong;
using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsIn;
using Dissimilis.WebAPI.Controllers.BoVoice;
using Dissimilis.WebAPI.Extensions.Models;
using Dissimilis.WebAPI.xUnit.Setup;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;
using static Dissimilis.WebAPI.xUnit.Extensions;

namespace Dissimilis.WebAPI.xUnit.Tests
{
    [Collection("Serial")]
    public class SongTests : BaseTestClass
    {


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
        public async Task TestSearchForSongs()
        {
            var mediator = _testServerFixture.GetServiceProvider().GetService<IMediator>();

            var createSongDto = CreateSongDto(4, 4);
            var updatedSongCommandDto = await mediator.Send(new CreateSongCommand(createSongDto));

            //Search for the user who created the song
            var searchQueryDto = SearchQueryDto();
            var songDtos = await mediator.Send(new QuerySongSearch(searchQueryDto));

            songDtos.Any(s => s.SongId == updatedSongCommandDto.SongId).ShouldBeTrue();
        }

        [Fact]
        public async Task TestNewSongSave()
        {
            var mediator = _testServerFixture.GetServiceProvider().GetService<IMediator>();

            var createSongDto = CreateSongDto(4, 4);

            var updatedSongCommandDto = await mediator.Send(new CreateSongCommand(createSongDto));
            var songDto = await mediator.Send(new QuerySongById(updatedSongCommandDto.SongId));

            createSongDto.Title.ShouldBe(songDto.Title, "Title not the same after creating");
            createSongDto.Numerator.ShouldBe(songDto.Numerator, "Numerator not the same after creating");
            createSongDto.Denominator.ShouldBe(songDto.Denominator, "Denominator not the same after creating");

            Assert.Single(songDto.Voices);

            songDto.Voices.FirstOrDefault()?.Bars.FirstOrDefault()?.ChordsAndNotes.FirstOrDefault()?.Length.ShouldBe(8, "Length of standard pause note is not correct");
        }


        [Fact]
        public async Task TestCopyBars()
        {
            var mediator = _testServerFixture.GetServiceProvider().GetService<IMediator>();

            var updatedSongCommandDto = await mediator.Send(new CreateSongCommand(CreateSongDto(4, 4)));
            var songDto = await mediator.Send(new QuerySongById(updatedSongCommandDto.SongId));

            var firstVoice = songDto.Voices.First();

            // populate a voice with bars and notes
            var firstVoiceFirstBar = firstVoice.Bars.First();
            await mediator.Send(new CreateSongNoteCommand(songDto.SongId, firstVoice.SongVoiceId, firstVoiceFirstBar.BarId, CreateNoteDto(6, 2, new[] { "H" })));

            var firstVoiceSecondBar = await mediator.Send(new CreateSongBarCommand(songDto.SongId, firstVoice.SongVoiceId, CreateBarDto()));
            await mediator.Send(new CreateSongNoteCommand(songDto.SongId, firstVoice.SongVoiceId, firstVoiceSecondBar.SongBarId, CreateNoteDto(0, 2, value: new[] { "A" })));

            var firstVoiceThirdBar = await mediator.Send(new CreateSongBarCommand(songDto.SongId, firstVoice.SongVoiceId, CreateBarDto(house: 1)));
            await mediator.Send(new CreateSongNoteCommand(songDto.SongId, firstVoice.SongVoiceId, firstVoiceThirdBar.SongBarId, CreateNoteDto(2, 6, value: new[] { "C" })));

            var firstVoiceFourthBar = await mediator.Send(new CreateSongBarCommand(songDto.SongId, firstVoice.SongVoiceId, CreateBarDto(house: 2, repAfter: true)));
            await mediator.Send(new CreateSongNoteCommand(songDto.SongId, firstVoice.SongVoiceId, firstVoiceFourthBar.SongBarId, CreateNoteDto(0, 8)));

            songDto = await mediator.Send(new QuerySongById(updatedSongCommandDto.SongId));
            songDto.Voices.Length.ShouldBe(1, "It is supposed to be only one voice");

            // make another voice, and set notes on 1 and 2
            var secondVoiceId = await mediator.Send(new CreateSongVoiceCommand(songDto.SongId, CreateSongVoiceDto("SecondVoice")));
            songDto = await mediator.Send(new QuerySongById(updatedSongCommandDto.SongId));
            var secondVoice = songDto.Voices.First(v => v.SongVoiceId == secondVoiceId.SongVoiceId);
            var secondVoiceFirstBar = secondVoice.Bars.OrderBy(b => b.Position).First();
            await mediator.Send(new CreateSongNoteCommand(songDto.SongId, secondVoice.SongVoiceId, secondVoiceFirstBar.BarId, CreateNoteDto(0, 2, value: new[] { "F" })));
            var secondVoiceSecondBar = secondVoice.Bars.OrderBy(b => b.Position).Skip(1).First();
            await mediator.Send(new CreateSongNoteCommand(songDto.SongId, secondVoice.SongVoiceId, secondVoiceSecondBar.BarId, CreateNoteDto(2, 6, value: new[] { "G" })));

            songDto.Voices.First().Bars.Length.ShouldBe(4, "It should be 4 bars before copying");


            // do Copying
            await mediator.Send(new CopyBarsCommand(songDto.SongId, CreateCopyBarsDto(1, 2, 4)));
            var afterCopySongDto = await mediator.Send(new QuerySongById(updatedSongCommandDto.SongId));

            // single check
            var fistCheck = afterCopySongDto.Voices.First(v => v.SongVoiceId == firstVoice.SongVoiceId).Bars.First(b => b.Position == 4)
                .ChordsAndNotes.FirstOrDefault(n => n.NoteId != null).Notes;
            fistCheck.ShouldBe(new[] { "H" }, "First copied note value not as expected");

            var secondCheck = afterCopySongDto.Voices.First(v => v.SongVoiceId == firstVoice.SongVoiceId).Bars.First(b => b.Position == 5)
                            .ChordsAndNotes.FirstOrDefault(n => n.NoteId != null).Notes;
            secondCheck.ShouldBe(new[] { "A" }, "Second copied note value not as expected");

            // check index
            afterCopySongDto.Voices.First(v => v.SongVoiceId == firstVoiceFirstBar.SongVoiceId).Bars.Length.ShouldBe(6, "It should be 6 bars after copying");
            var index = 1;
            foreach (var bar in afterCopySongDto.Voices.First().Bars)
            {
                bar.Position.ShouldBe(index++, "Index is not as expected after copying");
            }


            // check that value in bar position 1 and 2 are equal to 4 and 5 in all voices
            foreach (var voiceAfterCopy in afterCopySongDto.Voices)
            {
                var firstBarAfterCopy = voiceAfterCopy.Bars.First(b => b.Position == 1);
                var fourthBarAfterCopy = voiceAfterCopy.Bars.First(b => b.Position == 4);
                firstBarAfterCopy.CheckBarEqualTo(fourthBarAfterCopy, includeNoteComparison: true, stepDescription: "Position 1 against 4 after copy");

                var secondBarAfterCopy = voiceAfterCopy.Bars.First(b => b.Position == 2);
                var fifthBarAfterCopy = voiceAfterCopy.Bars.First(b => b.Position == 5);
                secondBarAfterCopy.CheckBarEqualTo(fifthBarAfterCopy, includeNoteComparison: true, stepDescription: "Position 2 against 5 after copy");

            }

        }

        [Fact]
        public async Task TestMoveBars()
        {
            var mediator = _testServerFixture.GetServiceProvider().GetService<IMediator>();

            var updatedSongCommandDto = await mediator.Send(new CreateSongCommand(CreateSongDto(4, 4)));
            var songDto = await mediator.Send(new QuerySongById(updatedSongCommandDto.SongId));

            var firstVoice = songDto.Voices.First();

            // populate a voice with bars and notes
            var firstVoiceFirstBar = firstVoice.Bars.First();
            await mediator.Send(new CreateSongNoteCommand(songDto.SongId, firstVoice.SongVoiceId, firstVoiceFirstBar.BarId, CreateNoteDto(0, 8, new[] { "A","C" })));

            var firstVoiceSecondBar = await mediator.Send(new CreateSongBarCommand(songDto.SongId, firstVoice.SongVoiceId, CreateBarDto()));
            await mediator.Send(new CreateSongNoteCommand(songDto.SongId, firstVoice.SongVoiceId, firstVoiceSecondBar.SongBarId, CreateNoteDto(0, 8, value: new[] { "A","H" })));

            var firstVoiceThirdBar = await mediator.Send(new CreateSongBarCommand(songDto.SongId, firstVoice.SongVoiceId, CreateBarDto(house: 1)));
            await mediator.Send(new CreateSongNoteCommand(songDto.SongId, firstVoice.SongVoiceId, firstVoiceThirdBar.SongBarId, CreateNoteDto(0, 8, value: new[] { "C","D" })));

            var firstVoiceFourthBar = await mediator.Send(new CreateSongBarCommand(songDto.SongId, firstVoice.SongVoiceId, CreateBarDto(house: 2, repAfter: true)));
            await mediator.Send(new CreateSongNoteCommand(songDto.SongId, firstVoice.SongVoiceId, firstVoiceFourthBar.SongBarId, CreateNoteDto(0, 8, value: new[] { "C","H" })));

            songDto = await mediator.Send(new QuerySongById(updatedSongCommandDto.SongId));
            songDto.Voices.Length.ShouldBe(1, "It is supposed to be only one voice");

            // make another voice, and set notes on 1 and 2
            var secondVoiceId = await mediator.Send(new CreateSongVoiceCommand(songDto.SongId, CreateSongVoiceDto("SecondVoice")));
            
            songDto = await mediator.Send(new QuerySongById(updatedSongCommandDto.SongId));
            var secondVoice = songDto.Voices.First(v => v.SongVoiceId == secondVoiceId.SongVoiceId);
            
            var secondVoiceFirstBar = secondVoice.Bars.OrderBy(b => b.Position).First();
            await mediator.Send(new CreateSongNoteCommand(songDto.SongId, secondVoice.SongVoiceId, secondVoiceFirstBar.BarId, CreateNoteDto(0, 2, value: new[] { "F" })));
            
            var secondVoiceSecondBar = secondVoice.Bars.OrderBy(b => b.Position).Skip(1).First();
            await mediator.Send(new CreateSongNoteCommand(songDto.SongId, secondVoice.SongVoiceId, secondVoiceSecondBar.BarId, CreateNoteDto(2, 6, value: new[] { "G" })));
            songDto.Voices.First().Bars.Length.ShouldBe(4, "It should be 4 bars before moving");


            // do Moving
            await mediator.Send(new MoveBarsCommand(songDto.SongId, CreateMoveBarsDto(0, 2, 3)));
            var afterCopySongDto = await mediator.Send(new QuerySongById(updatedSongCommandDto.SongId));

            // single check
            var fistCheck = afterCopySongDto.Voices.First(v => v.SongVoiceId == firstVoice.SongVoiceId).Bars.First(b => b.Position == 1).ChordsAndNotes.FirstOrDefault(n => n.NoteId != null).Notes;
            fistCheck.ShouldBe(new[] { "C","D" }, "First copied note value not as expected");

            var secondCheck = afterCopySongDto.Voices.First(v => v.SongVoiceId == firstVoice.SongVoiceId).Bars.First(b => b.Position == 4).ChordsAndNotes.FirstOrDefault(n => n.NoteId != null).Notes;
            secondCheck.ShouldBe(new[] { "C","H" }, "Second copied note value not as expected");

            // check index
            afterCopySongDto.Voices.First(v => v.SongVoiceId == firstVoiceFirstBar.SongVoiceId).Bars.Length.ShouldBe(4, "It should be 4 bars after moving");
            var index = 1;
            foreach (var bar in afterCopySongDto.Voices.First().Bars)
            {
                bar.Position.ShouldBe(index++, "Index is not as expected after copying");
            }

        }
    }
}

