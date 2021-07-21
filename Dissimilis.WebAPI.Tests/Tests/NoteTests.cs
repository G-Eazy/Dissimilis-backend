using System;
using System.Linq;
using System.Threading.Tasks;
using Dissimilis.WebAPI.Controllers.BoNote.DtoModelsIn;
using Dissimilis.WebAPI.Controllers.BoNote.Commands;
using Dissimilis.WebAPI.Controllers.BoNote.Query;
using Dissimilis.WebAPI.xUnit.Setup;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;
using static Dissimilis.WebAPI.xUnit.Extensions;
using Dissimilis.WebAPI.Exceptions;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Dissimilis.DbContext.Models.Song;
using Dissimilis.DbContext.Models;
using System.Collections.Generic;

namespace Dissimilis.WebAPI.xUnit.Tests
{
    [Collection("Serial")]
    [CollectionDefinition("Serial", DisableParallelization = true)]
    public class NoteTests : BaseTestClass
    {
        public NoteTests(TestServerFixture testServerFixture) : base(testServerFixture)
        {
        }

        [Fact]
        public async Task TestCreateSongNoteCommand()
        {
            var voice = SmokeOnTheWaterSong.Voices.FirstOrDefault();
            var bar = voice.SongBars.FirstOrDefault(bar => bar.Notes.Count == 0);

            TestServerFixture.ChangeCurrentUserId(DeepPurpleFanUser.Id);

            //Execute commands to test.
            var firstNoteUpdated = await _mediator.Send(
                new CreateSongNoteCommand(SmokeOnTheWaterSong.Id, voice.Id, bar.Id, CreateNoteDto(0, 1, "Emaj7")));
            var secondNoteUpdated = await _mediator.Send(
                new CreateSongNoteCommand(SmokeOnTheWaterSong.Id, voice.Id, bar.Id, CreateNoteDto(1, 1, "D#")));
            var thirdNoteUpdated = await _mediator.Send(
                new CreateSongNoteCommand(SmokeOnTheWaterSong.Id, voice.Id, bar.Id, CreateNoteDto(2, 1, "Cm")));

            //Fetch data after commands have executed.
            var firstNote = _testServerFixture.GetContext().SongNotes.SingleOrDefault(note =>
                note.Id == firstNoteUpdated.SongChordId);
            var secondNote = _testServerFixture.GetContext().SongNotes.SingleOrDefault(note =>
                note.Id == secondNoteUpdated.SongChordId);
            var thirdNote = _testServerFixture.GetContext().SongNotes.SingleOrDefault(note =>
                note.Id == thirdNoteUpdated.SongChordId);

            //Verify that the data have been stored with correct values.
            firstNote.ChordName.ShouldBe("Emaj7", $"Failed for chord Emaj7 with position 0.");
            secondNote.ChordName.ShouldBe("D#", $"Failed for chord Emaj7 with position 1.");
            thirdNote.ChordName.ShouldBe("Cm", $"Failed for chord Emaj7 with position 2.");

            //Verify that a note cannot be created where there already exists a note.
            Should.Throw<ValidationException>(() =>
                _mediator.Send(
                    new CreateSongNoteCommand(SmokeOnTheWaterSong.Id, voice.Id, bar.Id, CreateNoteDto(2, 1, "Emaj7"))),
                "Note number already in use");

            //Verify that a user without writeaccess is not able to create data.
            TestServerFixture.ChangeCurrentUserId(EdvardGriegFanUser.Id);

            Should.Throw<UnauthorizedAccessException>(() =>
                _mediator.Send(new CreateSongNoteCommand(SmokeOnTheWaterSong.Id, voice.Id, bar.Id, CreateNoteDto(3, 1, "C"))));

            TestServerFixture.ChangeCurrentUserId(SysAdminUser.Id);

            _testServerFixture.GetContext().Database.CurrentTransaction.Rollback();
        }

        [Fact]
        public async Task TestUpdateSongNoteCommand()
        {
            var notes = SmokeOnTheWaterSong
                .Voices.FirstOrDefault()
                .SongBars.FirstOrDefault(bar => bar.Notes.Count == 4)
                .Notes.Take(3).ToArray();

            var firstNote = notes[0];
            var secondNote = notes[1];
            var thirdNote = notes[2];

            TestServerFixture.ChangeCurrentUserId(DeepPurpleFanUser.Id);

            //Execute commands to test.
            var firstNoteUpdated = await _mediator.Send(
                new UpdateSongNoteCommand(SmokeOnTheWaterSong.Id, firstNote.Id, new UpdateNoteDto() { ChordName = "Gm13", Position = 0 }));
            var secondNoteUpdated = await _mediator.Send(
                new UpdateSongNoteCommand(SmokeOnTheWaterSong.Id, secondNote.Id, new UpdateNoteDto() { ChordName = "A", Position = 1 }));
            var thirdNoteUpdated = await _mediator.Send(
                new UpdateSongNoteCommand(SmokeOnTheWaterSong.Id, thirdNote.Id, new UpdateNoteDto() { ChordName = "F#m", Position = 2 }));

            //Fetch data after commands have executed.
            var firstNoteAfterAction = _testServerFixture.GetContext()
                .SongNotes.SingleOrDefault(note =>
                    note.Id == firstNote.Id);
            var secondNoteAfterAction = _testServerFixture.GetContext()
                .SongNotes.SingleOrDefault(note =>
                    note.Id == secondNote.Id);
            var thirdNoteAfterAction = _testServerFixture.GetContext()
                .SongNotes.SingleOrDefault(note =>
                    note.Id == thirdNote.Id);

            //Verify that the data have been stored with correct values.
            firstNote.ChordName.ShouldBe("Gm13", $"Failed for chord Gm13 with position 0.");
            secondNote.ChordName.ShouldBe("A", $"Failed for chord A with position 1.");
            thirdNote.ChordName.ShouldBe("F#m", $"Failed for chord F#m with position 2.");

            //Verify that a user without write access is not able to change data.
            TestServerFixture.ChangeCurrentUserId(EdvardGriegFanUser.Id);

            Should.Throw<UnauthorizedAccessException>(() =>
                _mediator.Send(new UpdateSongNoteCommand(SmokeOnTheWaterSong.Id, firstNoteUpdated.SongChordId, new UpdateNoteDto() { ChordName = "G" })));

            TestServerFixture.ChangeCurrentUserId(SysAdminUser.Id);

            _testServerFixture.GetContext().Database.CurrentTransaction.Rollback();
        }

        [Fact]
        public async Task TestDeleteSongNoteCommand()
        {
            var voice = SmokeOnTheWaterSong.Voices.FirstOrDefault();
            var bar = voice.SongBars.FirstOrDefault(bar => bar.Notes.Count == 4);
            var notes = bar
                .Notes.Take(3).ToArray();

            var firstNote = notes[0];
            var secondNote = notes[1];

            TestServerFixture.ChangeCurrentUserId(DeepPurpleFanUser.Id);
            //Execute command to test.
            var updatedSongNote = await _mediator.Send(new DeleteSongNoteCommand(SmokeOnTheWaterSong.Id, voice.Id, bar.Id, firstNote.Id));

            //Try fetch deleted data and verify that it is indeed deleted.
            Should.Throw<NotFoundException>(() =>
                _mediator.Send(new QuerySongNoteById(firstNote.Id)));

            //Verify that a user without write access is note able to delete data.
            TestServerFixture.ChangeCurrentUserId(EdvardGriegFanUser.Id);

            Should.Throw<UnauthorizedAccessException>(() =>
                _mediator.Send(new DeleteSongNoteCommand(SmokeOnTheWaterSong.Id, voice.Id, bar.Id, secondNote.Id)));

            TestServerFixture.ChangeCurrentUserId(SysAdminUser.Id);

            _testServerFixture.GetContext().Database.CurrentTransaction.Rollback();
        }
    }
}
