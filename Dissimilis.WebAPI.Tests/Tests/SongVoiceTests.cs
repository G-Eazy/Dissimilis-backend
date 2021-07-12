using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dissimilis.WebAPI.Controllers.BoBar.Commands;
using Dissimilis.WebAPI.Controllers.BoBar.Query;
using Dissimilis.WebAPI.Controllers.BoNote.Commands;
using Dissimilis.WebAPI.Controllers.BoSong;
using Dissimilis.WebAPI.Controllers.BoSong.Commands;
using Dissimilis.WebAPI.Controllers.BoSong.Query;
using Dissimilis.WebAPI.Controllers.BoVoice;
using Dissimilis.WebAPI.Controllers.BoVoice.Commands;
using Dissimilis.WebAPI.Controllers.BoVoice.Query;
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
            voice.Instrument.ShouldBe(null, "A songVoice should start without an instrument");
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

            var note1Post = await mediator.Send(new CreateSongNoteCommand(songDto.SongId, voice.SongVoiceId, bar.BarId, CreateNoteDto(6, 2, null)));
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

            await mediator.Send(new CreateSongNoteCommand(songDto.SongId, voice.SongVoiceId, bar.BarId, CreateNoteDto(6, 2, null)));
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
        public async Task TestDuplicateAllChords()
        {
            var mediator = _testServerFixture.GetServiceProvider().GetService<IMediator>();

            var createSongDto = CreateSongDto(4, 4);
            var updatedSongCommandDto = await mediator.Send(new CreateSongCommand(createSongDto));
            var songDto = await mediator.Send(new QuerySongById(updatedSongCommandDto.SongId));

            //Setup source voice to duplicate chords from.
            var sourceVoice = songDto.Voices.First();
            var firstSourceBar = sourceVoice.Bars.First();

            await mediator.Send(new CreateSongNoteCommand(songDto.SongId, sourceVoice.SongVoiceId, firstSourceBar.BarId, CreateNoteDto(0, 1, "C")));
            await mediator.Send(new CreateSongNoteCommand(songDto.SongId, sourceVoice.SongVoiceId, firstSourceBar.BarId, CreateNoteDto(1, 1, "Dm9")));
            await mediator.Send(new CreateSongNoteCommand(songDto.SongId, sourceVoice.SongVoiceId, firstSourceBar.BarId, CreateNoteDto(2, 2, "G#maj7")));

            firstSourceBar = await mediator.Send(new QueryBarById(songDto.SongId, sourceVoice.SongVoiceId, firstSourceBar.BarId));

            //Test duplicate with component intervals included.
            var destinationVoiceWith = await mediator.Send(new CreateSongVoiceCommand(songDto.SongId, CreateSongVoiceDto("Duplicate w voice")));

            songDto = await mediator.Send(new QuerySongById(updatedSongCommandDto.SongId));
            songDto.Voices.Length.ShouldBe(2, "Number of voices should be 2.");

            var updatedDestinationVoiceDto = await mediator.Send(
                new DuplicateAllChordsCommand(songDto.SongId, destinationVoiceWith.SongVoiceId, DuplicateAllChordsDto(sourceVoice.SongVoiceId, true)));
            var updatedDestinationVoice = await mediator.Send(
                new QuerySongVoiceById(updatedDestinationVoiceDto.SongId, updatedDestinationVoiceDto.SongVoiceId));


            foreach (var chord in updatedDestinationVoice.Bars[0].Chords)
            {
                if (chord.Position < 4)
                {
                    var expectedChord = firstSourceBar.Chords.First(srcChord => srcChord.Position == chord.Position);
                    chord.ChordName.ShouldBe(expectedChord.ChordName, $"The chordname was incorrect for chord on position {chord.Position}");
                    chord.Length.ShouldBe(expectedChord.Length, $"The length was incorrect for chord on position {chord.Position}");
                    chord.Notes.ShouldBeEquivalentTo(expectedChord.Notes, $"The notes was incorrect for chord on position {chord.Position}");
                }
                else
                {
                    chord.ChordName.ShouldBe(null, $"The chordname was incorrect for chord on position {chord.Position}");
                    chord.Notes.ShouldBeEquivalentTo(new string[] { "Z" }, $"The otes was incorrect for chord on position {chord.Position}");
                }
            }

            //Test duplicate without component intervals included.
            var destinationVoiceWithout = await mediator.Send(new CreateSongVoiceCommand(songDto.SongId, CreateSongVoiceDto("Duplicate wo voice")));

            songDto = await mediator.Send(new QuerySongById(updatedSongCommandDto.SongId));
            songDto.Voices.Length.ShouldBe(3, "Number of voices should be 3.");

            updatedDestinationVoiceDto = await mediator.Send(
                new DuplicateAllChordsCommand(songDto.SongId, destinationVoiceWithout.SongVoiceId, DuplicateAllChordsDto(sourceVoice.SongVoiceId, false)));
            updatedDestinationVoice = await mediator.Send(
                new QuerySongVoiceById(updatedDestinationVoiceDto.SongId, updatedDestinationVoiceDto.SongVoiceId));


            foreach (var chord in updatedDestinationVoice.Bars[0].Chords)
            {
                if (chord.Position < 4)
                //Verify that correct notes are copied over from source voice.
                {
                    var expectedChord = firstSourceBar.Chords.First(srcChord => srcChord.Position == chord.Position);
                    chord.ChordName.ShouldBe(expectedChord.ChordName, $"The chordname was incorrect for chord on position {chord.Position}");
                    chord.Length.ShouldBe(expectedChord.Length, $"The length was incorrect for chord on position {chord.Position}");
                    chord.Notes.ShouldBeEquivalentTo(Enumerable.Repeat("X", expectedChord.Notes.Length).ToArray(), $"The notes was incorrect for chord on position {chord.Position}");
                }
                else
                //Verify that notes that should not receive copy do not have a value. 
                {
                    chord.ChordName.ShouldBe(null, $"The chordname was incorrect for chord on position {chord.Position}");
                    chord.Notes.ShouldBeEquivalentTo(new string[] { "Z" }, $"The otes was incorrect for chord on position {chord.Position}");
                }
            }
        }

        [Fact]
        public async Task TestRemoveComponentInterval()
        {
            var mediator = _testServerFixture.GetServiceProvider().GetService<IMediator>();

            var createSongDto = CreateSongDto(4, 4);
            var updatedSongCommandDto = await mediator.Send(new CreateSongCommand(createSongDto));
            var songDto = await mediator.Send(new QuerySongById(updatedSongCommandDto.SongId));

            //Setup source voice to duplicate chords from.
            var sourceVoice = songDto.Voices.First();
            var firstSourceBar = sourceVoice.Bars.First();

            await mediator.Send(new CreateSongNoteCommand(songDto.SongId, sourceVoice.SongVoiceId, firstSourceBar.BarId, CreateNoteDto(0, 1, "C")));
            await mediator.Send(new CreateSongNoteCommand(songDto.SongId, sourceVoice.SongVoiceId, firstSourceBar.BarId, CreateNoteDto(1, 1, "Dm9")));
            await mediator.Send(new CreateSongNoteCommand(songDto.SongId, sourceVoice.SongVoiceId, firstSourceBar.BarId, CreateNoteDto(2, 2, "G#maj7")));

            //Test remove root component interval

            var expectedChordNames = new string[] { "C", "Dm9", "G#maj7" };
            var expectedNoteValues = new List<string[]>()
            {
                new string[] { "X", "E", "G" },
                new string[] { "X", "F", "A", "C", "E" },
                new string[] { "X", "C", "D#", "G" },
            };
            
            await mediator.Send(new RemoveComponentIntervalCommand(songDto.SongId, sourceVoice.SongVoiceId, RemoveComponentIntervalDto(0)));

            //Verify updated chords
            firstSourceBar = await mediator.Send(new QueryBarById(songDto.SongId, sourceVoice.SongVoiceId, firstSourceBar.BarId));

            foreach (var chord in firstSourceBar.Chords)
            {
                //Verify that all chords are updated.
                if (chord.Position < 4)
                {
                    chord.ChordName.ShouldBe(expectedChordNames[chord.Position], $"The chordname was incorrect for chord on position {chord.Position}");
                    chord.Notes.ShouldBeEquivalentTo(expectedNoteValues[chord.Position], $"The notes was incorrect for chord on position {chord.Position}");
                }
                else
                //Verify that notes that does not contain a chord remain the same. 
                {
                    chord.ChordName.ShouldBe(null, $"The chordname was incorrect for chord on position {chord.Position}");
                    chord.Notes.ShouldBeEquivalentTo(new string[] { "Z" }, $"The otes was incorrect for chord on position {chord.Position}");
                }
            }

            //Test remove fifth component interval

            expectedChordNames = new string[] { "C", "Dm9", "G#maj7" };
            expectedNoteValues = new List<string[]>()
            {
                new string[] { "X", "E", "X" },
                new string[] { "X", "F", "X", "C", "E" },
                new string[] { "X", "C", "X", "G" },
            };

            await mediator.Send(new RemoveComponentIntervalCommand(songDto.SongId, sourceVoice.SongVoiceId, RemoveComponentIntervalDto(2)));

            //Verify updated chords
            firstSourceBar = await mediator.Send(new QueryBarById(songDto.SongId, sourceVoice.SongVoiceId, firstSourceBar.BarId));

            foreach (var chord in firstSourceBar.Chords)
            {
                //Verify that all chords are updated.
                if (chord.Position < 4)
                {
                    chord.ChordName.ShouldBe(expectedChordNames[chord.Position], $"The chordname was incorrect for chord on position {chord.Position}");
                    chord.Notes.ShouldBeEquivalentTo(expectedNoteValues[chord.Position], $"The notes was incorrect for chord on position {chord.Position}");
                }
                else
                //Verify that notes that does not contain a chord remain the same. 
                {
                    chord.ChordName.ShouldBe(null, $"The chordname was incorrect for chord on position {chord.Position}");
                    chord.Notes.ShouldBeEquivalentTo(new string[] { "Z" }, $"The otes was incorrect for chord on position {chord.Position}");
                }
            }

            //Test remove seventh component interval

            expectedChordNames = new string[] { "C", "Dm9", "G#maj7" };
            expectedNoteValues = new List<string[]>()
            {
                new string[] { "X", "E", "X" },
                new string[] { "X", "F", "X", "X", "E" },
                new string[] { "X", "C", "X", "X" },
            };

            await mediator.Send(new RemoveComponentIntervalCommand(songDto.SongId, sourceVoice.SongVoiceId, RemoveComponentIntervalDto(3)));

            //Verify updated chords
            firstSourceBar = await mediator.Send(new QueryBarById(songDto.SongId, sourceVoice.SongVoiceId, firstSourceBar.BarId));

            foreach (var chord in firstSourceBar.Chords)
            {
                //Verify that all chords are updated.
                if (chord.Position < 4)
                {
                    chord.ChordName.ShouldBe(expectedChordNames[chord.Position], $"The chordname was incorrect for chord on position {chord.Position}");
                    chord.Notes.ShouldBeEquivalentTo(expectedNoteValues[chord.Position], $"The notes was incorrect for chord on position {chord.Position}");
                }
                else
                //Verify that notes that does not contain a chord remain the same. 
                {
                    chord.ChordName.ShouldBe(null, $"The chordname was incorrect for chord on position {chord.Position}");
                    chord.Notes.ShouldBeEquivalentTo(new string[] { "Z" }, $"The otes was incorrect for chord on position {chord.Position}");
                }
            }
        }

        [Fact]
        public async Task TestAddComponentInterval()
        {
            var mediator = _testServerFixture.GetServiceProvider().GetService<IMediator>();

            var createSongDto = CreateSongDto(4, 4);
            var updatedSongCommandDto = await mediator.Send(new CreateSongCommand(createSongDto));
            var songDto = await mediator.Send(new QuerySongById(updatedSongCommandDto.SongId));

            //Setup source voice to duplicate chords from.
            var sourceVoice = songDto.Voices.First();
            var firstSourceBar = sourceVoice.Bars.First();

            await mediator.Send(new CreateSongNoteCommand(songDto.SongId, sourceVoice.SongVoiceId, firstSourceBar.BarId, CreateNoteDto(0, 1, "C")));
            await mediator.Send(new CreateSongNoteCommand(songDto.SongId, sourceVoice.SongVoiceId, firstSourceBar.BarId, CreateNoteDto(1, 1, "Dm9")));
            await mediator.Send(new CreateSongNoteCommand(songDto.SongId, sourceVoice.SongVoiceId, firstSourceBar.BarId, CreateNoteDto(2, 2, "G#maj7")));

            var newVoiceResponse = await mediator.Send(new CreateSongVoiceCommand(songDto.SongId, CreateSongVoiceDto("New voice")));
            await mediator.Send(new DuplicateAllChordsCommand(songDto.SongId, newVoiceResponse.SongVoiceId, DuplicateAllChordsDto(sourceVoice.SongVoiceId, false)));
            var newVoice = await mediator.Send(new QuerySongVoiceById(songDto.SongId, newVoiceResponse.SongVoiceId));

            var firstNewVoiceBar = newVoice.Bars.First();

            //Test add root component interval

            var expectedChordNames = new string[] { "C", "Dm9", "G#maj7" };
            var expectedNoteValues = new List<string[]>()
            {
                new string[] { "C", "X", "X" },
                new string[] { "D", "X", "X", "X", "X" },
                new string[] { "G#", "X", "X", "X" },
            };

            await mediator.Send(new AddComponentIntervalCommand(songDto.SongId, newVoice.SongVoiceId, AddComponentIntervalDto(0)));

            //Verify updated chords
            firstNewVoiceBar = await mediator.Send(new QueryBarById(songDto.SongId, newVoice.SongVoiceId, firstNewVoiceBar.BarId));

            foreach (var chord in firstNewVoiceBar.Chords)
            {
                //Verify that all chords are updated.
                if (chord.Position < 4)
                {
                    chord.ChordName.ShouldBe(expectedChordNames[chord.Position], $"The chordname was incorrect for chord on position {chord.Position}");
                    chord.Notes.ShouldBeEquivalentTo(expectedNoteValues[chord.Position], $"The notes was incorrect for chord on position {chord.Position}");
                }
                else
                //Verify that notes that does not contain a chord remain the same. 
                {
                    chord.ChordName.ShouldBe(null, $"The chordname was incorrect for chord on position {chord.Position}");
                    chord.Notes.ShouldBeEquivalentTo(new string[] { "Z" }, $"The otes was incorrect for chord on position {chord.Position}");
                }
            }

            //Test add fifth component interval

            expectedChordNames = new string[] { "C", "Dm9", "G#maj7" };
            expectedNoteValues = new List<string[]>()
            {
                new string[] { "C", "X", "G" },
                new string[] { "D", "X", "A", "X", "X" },
                new string[] { "G#", "X", "D#", "X" },
            };

            await mediator.Send(new AddComponentIntervalCommand(songDto.SongId, newVoice.SongVoiceId, AddComponentIntervalDto(2)));

            //Verify updated chords
            firstNewVoiceBar = await mediator.Send(new QueryBarById(songDto.SongId, newVoice.SongVoiceId, firstNewVoiceBar.BarId));

            foreach (var chord in firstNewVoiceBar.Chords)
            {
                //Verify that all chords are updated.
                if (chord.Position < 4)
                {
                    chord.ChordName.ShouldBe(expectedChordNames[chord.Position], $"The chordname was incorrect for chord on position {chord.Position}");
                    chord.Notes.ShouldBeEquivalentTo(expectedNoteValues[chord.Position], $"The notes was incorrect for chord on position {chord.Position}");
                }
                else
                //Verify that notes that does not contain a chord remain the same. 
                {
                    chord.ChordName.ShouldBe(null, $"The chordname was incorrect for chord on position {chord.Position}");
                    chord.Notes.ShouldBeEquivalentTo(new string[] { "Z" }, $"The otes was incorrect for chord on position {chord.Position}");
                }
            }

            //Test add seventh component interval

            expectedChordNames = new string[] { "C", "Dm9", "G#maj7" };
            expectedNoteValues = new List<string[]>()
            {
                new string[] { "C", "X", "G" },
                new string[] { "D", "X", "A", "C", "X" },
                new string[] { "G#", "X", "D#", "G" },
            };

            await mediator.Send(new AddComponentIntervalCommand(songDto.SongId, newVoice.SongVoiceId, AddComponentIntervalDto(3)));

            //Verify updated chords
            firstNewVoiceBar = await mediator.Send(new QueryBarById(songDto.SongId, newVoice.SongVoiceId, firstNewVoiceBar.BarId));

            foreach (var chord in firstNewVoiceBar.Chords)
            {
                //Verify that all chords are updated.
                if (chord.Position < 4)
                {
                    chord.ChordName.ShouldBe(expectedChordNames[chord.Position], $"The chordname was incorrect for chord on position {chord.Position}");
                    chord.Notes.ShouldBeEquivalentTo(expectedNoteValues[chord.Position], $"The notes was incorrect for chord on position {chord.Position}");
                }
                else
                //Verify that notes that does not contain a chord remain the same. 
                {
                    chord.ChordName.ShouldBe(null, $"The chordname was incorrect for chord on position {chord.Position}");
                    chord.Notes.ShouldBeEquivalentTo(new string[] { "Z" }, $"The otes was incorrect for chord on position {chord.Position}");
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
            var createSongVoiceDto = CreateSongVoiceDto("Per", baseVoice.PartNumber);
            var duplicatedVoice1 = await mediator.Send(new DuplicateVoiceCommand(songDto.SongId, baseVoice.SongVoiceId, createSongVoiceDto));
            songDto = await mediator.Send(new QuerySongById(updatedSongCommandDto.SongId));
            songDto.Voices.Length.ShouldBe(2, "More voices than expected");
            songDto.Voices.First(v => v.SongVoiceId == duplicatedVoice1.SongVoiceId).VoiceName.ShouldBe("Per", "Duplicated voice didn't have expected name");
            baseVoice.VoiceName.ShouldBe("Main");


            await mediator.Send(new UpdateSongVoiceCommand(songDto.SongId, baseVoice.SongVoiceId, CreateSongVoiceDto("Piano", 1)));
            await mediator.Send(new CreateSongNoteCommand(songDto.SongId, baseVoice.SongVoiceId, bar.BarId, CreateNoteDto(1, 4, null)));

            songDto = await mediator.Send(new QuerySongById(updatedSongCommandDto.SongId));
            songDto.Voices.First().VoiceName.ShouldBe("Piano");
            await mediator.Send(new UpdateSongVoiceCommand(songDto.SongId, baseVoice.SongVoiceId, CreateSongVoiceDto("Piano", 1)));
            
            songDto.Voices.First().Instrument.ShouldBe(null);

            var createSongVoiceDto2 = CreateSongVoiceDto("Frank", baseVoice.PartNumber);

            var duplicatedVoice2 = await mediator.Send(new DuplicateVoiceCommand(songDto.SongId, baseVoice.SongVoiceId, createSongVoiceDto2));
            songDto = await mediator.Send(new QuerySongById(updatedSongCommandDto.SongId));
            songDto.Voices.First(v => v.SongVoiceId == duplicatedVoice2.SongVoiceId).VoiceName.ShouldBe("Frank", "Duplicated voice didn't have expected name");

            songDto.Voices.First(v => v.SongVoiceId == baseVoice.SongVoiceId).CheckVoiceBarsEqualTo(songDto.Voices.First(v => v.SongVoiceId == duplicatedVoice2.SongVoiceId), true);
            var createSongVoiceDto3 = CreateSongVoiceDto("Siri", baseVoice.PartNumber);

            var duplicatedVoice3 = await mediator.Send(new DuplicateVoiceCommand(songDto.SongId, duplicatedVoice2.SongVoiceId, createSongVoiceDto3));
            songDto = await mediator.Send(new QuerySongById(updatedSongCommandDto.SongId));
            songDto.Voices.First(v => v.SongVoiceId == duplicatedVoice3.SongVoiceId).VoiceName.ShouldBe("Siri", "Second duplicated voice didn't have expected name");

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
            await mediator.Send(new UpdateSongVoiceCommand(songDto.SongId, baseVoice.SongVoiceId, CreateSongVoiceDto("Piano", 1)));
            await mediator.Send(new CreateSongNoteCommand(songDto.SongId, baseVoice.SongVoiceId, bar.BarId, CreateNoteDto(1, 4, null)));
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

