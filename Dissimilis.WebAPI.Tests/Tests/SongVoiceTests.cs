using System.Linq;
using System.Threading.Tasks;
using Dissimilis.WebAPI.Controllers.BoSong;
using Dissimilis.WebAPI.Controllers.BoVoice;
using Dissimilis.WebAPI.xUnit.Setup;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;
using static Dissimilis.WebAPI.xUnit.Extensions;

namespace Dissimilis.WebAPI.xUnit.Tests
{
    [Collection("Serial")]
    [CollectionDefinition("Serial", DisableParallelization = true)]
    public class SongVoiceTests : BaseTestClass
    {
        public SongVoiceTests(TestServerFixture testServerFixture) : base(testServerFixture)
        {
        }

        [Fact]
        public async Task TestNewSongVoice()
        {
            var mediator = _testServerFixture.GetServiceProvider().GetService<IMediator>();

            var createSongDto = CreateSongDto(4, 4);

            var updatedSongCommandDto = await mediator.Send(new CreateSongCommand(createSongDto));
            var songDto = await mediator.Send(new QuerySongById(updatedSongCommandDto.SongId));

            songDto.Voices.FirstOrDefault()?.Bars.FirstOrDefault()?.Chords.FirstOrDefault()?.Length.ShouldBe(8, "Length of standard pause chord is not correct");

            songDto.Voices.Length.ShouldBe(1, "It should only be one voice created by default");
            var voice = songDto.Voices.First();

            voice.Bars.Length.ShouldBe(1, "It should only be one bar created by default");
            var bar = voice.Bars.First();

            bar.Chords.Length.ShouldBe(1, "It should only be one cord/note created by default");
            var cordNote = bar.Chords.First();
            cordNote.Position.ShouldBe(0, "The default empty position should be 0");
            cordNote.Length.ShouldBe(8, "The default length should be 8");
        }

        [Fact]
        public async Task TestUpdatingBar()
        {
            var mediator = _testServerFixture.GetServiceProvider().GetService<IMediator>();

            var createSongDto = CreateSongDto(4, 4);

            var updatedSongCommandDto = await mediator.Send(new CreateSongCommand(createSongDto));
            var songDto = await mediator.Send(new QuerySongById(updatedSongCommandDto.SongId));

            songDto.Voices.FirstOrDefault()?.Bars.FirstOrDefault()?.Chords.FirstOrDefault()?.Length.ShouldBe(8, "Length of standard pause note is not correct");

            var voice = songDto.Voices.First();
            var bar = voice.Bars.First();

            var note1Post = await mediator.Send(new CreateSongNoteCommand(songDto.SongId, voice.SongVoiceId, bar.BarId, CreateNoteDto(6, 2)));
            songDto = await mediator.Send(new QuerySongById(updatedSongCommandDto.SongId));
            songDto.Voices.First().Bars.First().Chords.Length.ShouldBe(2);
            var zNote1Update1 = songDto.Voices.First().Bars.First().Chords.FirstOrDefault(n => n.Notes.Any(n => n == "Z"));
            zNote1Update1.Position.ShouldBe(0, "The pause should start at position 0");
            zNote1Update1.Length.ShouldBe(6, "The length of the pause should be 6");

            var note1Update1 = songDto.Voices.First().Bars.First().Chords.FirstOrDefault(n => n.ChordId == note1Post.SongChordId);
            note1Update1.Position.ShouldBe(6, "Position of note1Update1 should be 6");
            note1Update1.Length.ShouldBe(2, "Length of note1Update1 should be 2");

            await mediator.Send(new UpdateSongNoteCommand(songDto.SongId, voice.SongVoiceId, bar.BarId, note1Post.SongChordId, GetUpdateNoteDto(note1Update1, position: 7, length: 1)));
            await mediator.Send(new UpdateSongNoteCommand(songDto.SongId, voice.SongVoiceId, bar.BarId, note1Post.SongChordId, GetUpdateNoteDto(note1Update1, position: 5, length: 3)));
            await mediator.Send(new UpdateSongNoteCommand(songDto.SongId, voice.SongVoiceId, bar.BarId, note1Post.SongChordId, GetUpdateNoteDto(note1Update1, position: 4, length: 4)));
            await mediator.Send(new UpdateSongNoteCommand(songDto.SongId, voice.SongVoiceId, bar.BarId, note1Post.SongChordId, GetUpdateNoteDto(note1Update1, position: 3, length: 5)));
            await mediator.Send(new UpdateSongNoteCommand(songDto.SongId, voice.SongVoiceId, bar.BarId, note1Post.SongChordId, GetUpdateNoteDto(note1Update1, position: 2, length: 6)));
            await mediator.Send(new UpdateSongNoteCommand(songDto.SongId, voice.SongVoiceId, bar.BarId, note1Post.SongChordId, GetUpdateNoteDto(note1Update1, position: 1, length: 7)));
            await mediator.Send(new UpdateSongNoteCommand(songDto.SongId, voice.SongVoiceId, bar.BarId, note1Post.SongChordId, GetUpdateNoteDto(note1Update1, position: 0, length: 8)));
        }

        [Fact]
        public async Task TestSyncBetweenVoices()
        {
            var mediator = _testServerFixture.GetServiceProvider().GetService<IMediator>();

            var createSongDto = CreateSongDto(4, 4);

            var updatedSongCommandDto = await mediator.Send(new CreateSongCommand(createSongDto));
            var songDto = await mediator.Send(new QuerySongById(updatedSongCommandDto.SongId));

            songDto.Voices.FirstOrDefault()?.Bars.FirstOrDefault()?.Chords.FirstOrDefault()?.Length.ShouldBe(8, "Length of standard pause chord is not correct");

            var voice = songDto.Voices.First();
            var bar = voice.Bars.First();

            await mediator.Send(new CreateSongVoiceCommand(songDto.SongId, CreateSongVoiceDto("FirstVoice")));
            await mediator.Send(new CreateSongVoiceCommand(songDto.SongId, CreateSongVoiceDto("SecondVoice")));
            CheckSongVoiceIntegrity(await mediator.Send(new QuerySongById(updatedSongCommandDto.SongId)), "After first and second voice");

            await mediator.Send(new CreateSongNoteCommand(songDto.SongId, voice.SongVoiceId, bar.BarId, CreateNoteDto(6, 2)));
            var firstSongBar = await mediator.Send(new CreateSongBarCommand(songDto.SongId, voice.SongVoiceId, CreateBarDto()));
            CheckSongVoiceIntegrity(await mediator.Send(new QuerySongById(updatedSongCommandDto.SongId)), "After a note and a bar");

            await mediator.Send(new CreateSongBarCommand(songDto.SongId, voice.SongVoiceId, CreateBarDto(repBefore: true)));
            await mediator.Send(new CreateSongBarCommand(songDto.SongId, voice.SongVoiceId, CreateBarDto(repAfter: true)));
            await mediator.Send(new CreateSongBarCommand(songDto.SongId, voice.SongVoiceId, CreateBarDto(house: 1)));
            await mediator.Send(new CreateSongBarCommand(songDto.SongId, voice.SongVoiceId, CreateBarDto(house: 1)));
            await mediator.Send(new CreateSongBarCommand(songDto.SongId, voice.SongVoiceId, CreateBarDto(house: 2)));
            await mediator.Send(new CreateSongBarCommand(songDto.SongId, voice.SongVoiceId, CreateBarDto(house: 2)));
            CheckSongVoiceIntegrity(await mediator.Send(new QuerySongById(updatedSongCommandDto.SongId)), "After adding several bars");

            await mediator.Send(new CreateSongVoiceCommand(songDto.SongId, CreateSongVoiceDto("ThirdVoice")));
            CheckSongVoiceIntegrity(await mediator.Send(new QuerySongById(updatedSongCommandDto.SongId)), "After adding third voice");

            await mediator.Send(new UpdateSongBarCommand(songDto.SongId, voice.SongVoiceId, firstSongBar.SongBarId, CreateUpdateBarDto(repBefore: true, repAfter: true)));
            CheckSongVoiceIntegrity(await mediator.Send(new QuerySongById(updatedSongCommandDto.SongId)), "After last songBarUpdate");
        }

        [Fact]
        public async Task TestCopyComponentInterval()
        {
            var mediator = _testServerFixture.GetServiceProvider().GetService<IMediator>();

            var createSongDto = CreateSongDto(4, 4);
            var updatedSongCommandDto = await mediator.Send(new CreateSongCommand(createSongDto));
            var songDto = await mediator.Send(new QuerySongById(updatedSongCommandDto.SongId));

            //Setup source voice to copy component interval from.
            var sourceVoice = songDto.Voices.First();
            var firstSourceBar = sourceVoice.Bars.First();

            await mediator.Send(new CreateSongNoteCommand(songDto.SongId, sourceVoice.SongId, firstSourceBar.SongId, CreateNoteDto(0, 1, new string[] { "C", "E", "G" })));
            await mediator.Send(new CreateSongNoteCommand(songDto.SongId, sourceVoice.SongId, firstSourceBar.SongId, CreateNoteDto(1, 1, new string[] { "C", "E", "G" })));
            await mediator.Send(new CreateSongNoteCommand(songDto.SongId, sourceVoice.SongId, firstSourceBar.SongId, CreateNoteDto(2, 2, new string[] { "C", "E", "G" })));

            //Test root copy.
            var destinationVoiceRoot = await mediator.Send(new CreateSongVoiceCommand(songDto.SongId, CreateSongVoiceDto("Root voice")));

            songDto = await mediator.Send(new QuerySongById(updatedSongCommandDto.SongId));
            songDto.Voices.Length.ShouldBe(2, "Number of voices should be 2.");

            var updatedDestinationVoiceDto = await mediator.Send(
                new DuplicateAllChordsCommand(songDto.SongId, destinationVoiceRoot.SongVoiceId, DuplicateComponentIntervalDto(0, sourceVoice.SongVoiceId)));
            var updatedDestinationVoice = await mediator.Send(
                new QuerySongVoiceById(updatedDestinationVoiceDto.SongId, updatedDestinationVoiceDto.SongVoiceId));
            
            var expectedNotesCopied = new string[] { "C" };
            var expectedNotesNotCopied = new string[] { "Z" };

            updatedDestinationVoice.Title.ShouldBe("Root voice", "The title of the destination voice is incorrect.");
            foreach (var chord in updatedDestinationVoice.Bars[0].Chords)
            {
                if (chord.Position < 4)
                    //Verify that correct notes are copied over from source voice.
                {
                    chord.Notes.ShouldBe(expectedNotesCopied, $"Copy failed for note {chord.Position}.");
                }
                else
                    //Verify that notes that should not receive copy do not have a value. 
                {
                    chord.Notes.ShouldBe(expectedNotesNotCopied, $"Unexpected copy happened for note {chord.Position}.");
                }
            }

            //Test third copy.
            var destinationVoiceThird = await mediator.Send(new CreateSongVoiceCommand(songDto.SongId, CreateSongVoiceDto("Third voice")));

            songDto = await mediator.Send(new QuerySongById(updatedSongCommandDto.SongId));
            songDto.Voices.Length.ShouldBe(3, "Number of voices should be 3.");

            updatedDestinationVoiceDto = await mediator.Send(
                new DuplicateAllChordsCommand(songDto.SongId, destinationVoiceThird.SongVoiceId, DuplicateComponentIntervalDto(1, sourceVoice.SongVoiceId)));
            updatedDestinationVoice = await mediator.Send(
                new QuerySongVoiceById(updatedDestinationVoiceDto.SongId, updatedDestinationVoiceDto.SongVoiceId));

            expectedNotesCopied = new string[] { "E" };
            expectedNotesNotCopied = new string[] { "Z" };

            updatedDestinationVoice.Title.ShouldBe("Third voice", "The title of the destination voice is incorrect.");
            foreach (var chord in updatedDestinationVoice.Bars[0].Chords)
            {
                if (chord.Position < 4)
                //Verify that correct notes are copied over from source voice.
                {
                    chord.Notes.ShouldBe(expectedNotesCopied, $"Copy failed for note {chord.Position}.");
                }
                else
                //Verify that notes that should not receive copy do not have a value. 
                {
                    chord.Notes.ShouldBe(expectedNotesNotCopied, $"Unexpected copy happened for note {chord.Position}.");
                }
            }

            //Test fifth copy.
            var destinationVoiceFifth = await mediator.Send(new CreateSongVoiceCommand(songDto.SongId, CreateSongVoiceDto("Fifth voice")));

            songDto = await mediator.Send(new QuerySongById(updatedSongCommandDto.SongId));
            songDto.Voices.Length.ShouldBe(4, "Number of voices should be 4.");

            updatedDestinationVoiceDto = await mediator.Send(
                new DuplicateAllChordsCommand(songDto.SongId, destinationVoiceFifth.SongVoiceId, DuplicateComponentIntervalDto(2, sourceVoice.SongVoiceId)));
            updatedDestinationVoice = await mediator.Send(
                new QuerySongVoiceById(updatedDestinationVoiceDto.SongId, updatedDestinationVoiceDto.SongVoiceId));

            expectedNotesCopied = new string[] { "G" };
            expectedNotesNotCopied = new string[] { "Z" };

            updatedDestinationVoice.Title.ShouldBe("Fifth voice", "The title of the destination voice is incorrect.");
            foreach (var chord in updatedDestinationVoice.Bars[0].Chords)
            {
                if (chord.Position < 4)
                //Verify that correct notes are copied over from source voice.
                {
                    chord.Notes.ShouldBe(expectedNotesCopied, $"Copy failed for note {chord.Position}.");
                }
                else
                //Verify that notes that should not receive copy do not have a value. 
                {
                    chord.Notes.ShouldBe(expectedNotesNotCopied, $"Unexpected copy happened for note {chord.Position}.");
                }
            }

            //Test copy third when root is present.
            songDto = await mediator.Send(new QuerySongById(updatedSongCommandDto.SongId));

            updatedDestinationVoiceDto = await mediator.Send(
                new DuplicateAllChordsCommand(songDto.SongId, destinationVoiceRoot.SongVoiceId, DuplicateComponentIntervalDto(2, sourceVoice.SongVoiceId)));
            updatedDestinationVoice = await mediator.Send(
                new QuerySongVoiceById(updatedDestinationVoiceDto.SongId, updatedDestinationVoiceDto.SongVoiceId));

            expectedNotesCopied = new string[] { "C", "E" };
            expectedNotesNotCopied = new string[] { "Z" };

            foreach (var chord in updatedDestinationVoice.Bars[0].Chords)
            {
                if (chord.Position < 4)
                //Verify that correct notes are copied over from source voice.
                {
                    chord.Notes.ShouldBe(expectedNotesCopied, $"Copy failed for note {chord.Position}.");
                }
                else
                //Verify that notes that should not receive copy do not have a value. 
                {
                    chord.Notes.ShouldBe(expectedNotesNotCopied, $"Unexpected copy happened for note {chord.Position}.");
                }
            }
        }

        [Fact]
        public async Task TestDuplicateVoice()
        {
            var mediator = _testServerFixture.GetServiceProvider().GetService<IMediator>();

            var createSongDto = CreateSongDto(4, 4);

            var updatedSongCommandDto = await mediator.Send(new CreateSongCommand(createSongDto));
            var songDto = await mediator.Send(new QuerySongById(updatedSongCommandDto.SongId));
            
            var baseVoice = songDto.Voices.First();
            var bar = baseVoice.Bars.First();

            songDto.Voices.Length.ShouldBe(1, "More voices than expected");

            var duplicatedVoice1 = await mediator.Send(new DuplicateVoiceCommand(songDto.SongId, baseVoice.SongVoiceId));
            songDto = await mediator.Send(new QuerySongById(updatedSongCommandDto.SongId));
            songDto.Voices.First(v => v.SongVoiceId == duplicatedVoice1.SongVoiceId).Title.ShouldBe("Main 1", "Duplicated voice didn't have expected name");
            baseVoice.Title.ShouldBe("Main");


            await mediator.Send(new UpdateSongVoiceCommand(songDto.SongId, baseVoice.SongVoiceId, UpdateSongVoiceDto("Piano", 1)));
            await mediator.Send(new CreateSongNoteCommand(songDto.SongId, baseVoice.SongVoiceId, bar.BarId, CreateNoteDto(1, 4)));

            songDto = await mediator.Send(new QuerySongById(updatedSongCommandDto.SongId));
            songDto.Voices.First().Title.ShouldBe("Piano");
            await mediator.Send(new UpdateSongVoiceCommand(songDto.SongId, baseVoice.SongVoiceId, UpdateSongVoiceDto("Piano", 1)));

            var duplicatedVoice2 = await mediator.Send(new DuplicateVoiceCommand(songDto.SongId, baseVoice.SongVoiceId));
            songDto = await mediator.Send(new QuerySongById(updatedSongCommandDto.SongId));
            songDto.Voices.First(v => v.SongVoiceId == duplicatedVoice2.SongVoiceId).Title.ShouldBe("Piano 1", "Duplicated voice didn't have expected name");

            songDto.Voices.First(v => v.SongVoiceId == baseVoice.SongVoiceId).CheckVoiceBarsEqualTo(songDto.Voices.First(v => v.SongVoiceId == duplicatedVoice2.SongVoiceId), true);

            var duplicatedVoice3 = await mediator.Send(new DuplicateVoiceCommand(songDto.SongId, duplicatedVoice2.SongVoiceId));
            songDto = await mediator.Send(new QuerySongById(updatedSongCommandDto.SongId));
            songDto.Voices.First(v => v.SongVoiceId == duplicatedVoice3.SongVoiceId).Title.ShouldBe("Piano 2", "Second duplicated voice didn't have expected name");

            songDto.Voices.First(v => v.SongVoiceId == duplicatedVoice2.SongVoiceId).CheckVoiceBarsEqualTo(songDto.Voices.First(v => v.SongVoiceId == duplicatedVoice3.SongVoiceId), true);
        }

        [Fact]
        public async Task TestDuplicateSong()
        {
            var mediator = _testServerFixture.GetServiceProvider().GetService<IMediator>();

            var createSongDto = CreateSongDto(title: "Song 1");

            var updatedSongCommandDto = await mediator.Send(new CreateSongCommand(createSongDto));
            var songDto = await mediator.Send(new QuerySongById(updatedSongCommandDto.SongId));
            songDto.Title.ShouldBe("Song 1", "Song title not as expected");

            var baseVoice = songDto.Voices.First();
            var bar = baseVoice.Bars.First();
            await mediator.Send(new UpdateSongVoiceCommand(songDto.SongId, baseVoice.SongVoiceId, UpdateSongVoiceDto("Piano", 1)));
            await mediator.Send(new CreateSongNoteCommand(songDto.SongId, baseVoice.SongVoiceId, bar.BarId, CreateNoteDto(1, 4)));
            await mediator.Send(new CreateSongVoiceCommand(songDto.SongId, CreateSongVoiceDto("FirstVoice")));
            songDto = await mediator.Send(new QuerySongById(updatedSongCommandDto.SongId));

            var duplicatedSong = await mediator.Send(new DuplicateSongCommand(songDto.SongId, DuplicateSongDto("Song 2")));
            var duplicatedSongDto = await mediator.Send(new QuerySongById(duplicatedSong.SongId));
            duplicatedSongDto.Title.ShouldBe("Song 2", "Title og duplicated son not as expected");

            songDto.Voices.Length.ShouldBe(duplicatedSongDto.Voices.Length, "Not same amount of voices");
            var voicesLength = songDto.Voices.Length;
            for (var i = 0; i < voicesLength; i++)
            {
                songDto.Voices[i].CheckVoiceBarsEqualTo(duplicatedSongDto.Voices[i], true);
            }
        }

    }


}

