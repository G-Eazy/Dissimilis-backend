using System;
using System.Linq;
using System.Threading.Tasks;
using Dissimilis.WebAPI.Controllers.BoNote.DtoModelsIn;
using Dissimilis.WebAPI.Controllers.BoNote.Commands;
using Dissimilis.WebAPI.Controllers.BoNote.Query;
using Dissimilis.WebAPI.Controllers.BoSong.Query;
using Dissimilis.WebAPI.Controllers.BoUser.Queries;
using Dissimilis.WebAPI.xUnit.Setup;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;
using static Dissimilis.WebAPI.xUnit.Extensions;
using Dissimilis.WebAPI.Exceptions;
using Dissimilis.WebAPI.Controllers.BoUser.DtoModelsOut;
using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsOut;
using Dissimilis.WebAPI.Controllers.BoVoice.DtoModelsOut;
using Dissimilis.WebAPI.Controllers.BoBar.DtoModelsOut;
using Dissimilis.WebAPI.Controllers.BoNote.DtoModelsOut;
using System.ComponentModel.DataAnnotations;

namespace Dissimilis.WebAPI.xUnit.Tests
{
    [Collection("Serial")]
    [CollectionDefinition("Serial", DisableParallelization = true)]
    public class NoteTests : BaseTestClass
    {
        private readonly IMediator _mediator;
        private UserDto AdminUser;
        private UserDto SuppUser1;
        private UserDto SuppUser2;
        private UserDto SuppUser3;

        private SongByIdDto TestSong;
        private SongVoiceDto TestSongVoice;
        private BarDto TestSongBarWithNotes;
        private BarDto TestSongBarWithoutNotes;
        private NoteDto[] TestSongNotes;

        public NoteTests(TestServerFixture testServerFixture) : base(testServerFixture)
        {
            _mediator = _testServerFixture.GetServiceProvider().GetService<IMediator>();
            var users = GetAllUsers().Result;
            AdminUser = users.SingleOrDefault(user => user.Email == "test@test.no");
            SuppUser1 = users.SingleOrDefault(user => user.Email == "supUser1@test.no");
            SuppUser2 = users.SingleOrDefault(user => user.Email == "supUser2@test.no");
            SuppUser3 = users.SingleOrDefault(user => user.Email == "supUser3@test.no");

            var songRequestResult = GetTestSong().Result;
            TestSong = songRequestResult.song;
            TestSongVoice = songRequestResult.voice;
            TestSongBarWithNotes = songRequestResult.barWithNotes;
            TestSongBarWithoutNotes = songRequestResult.barWithoutNotes;
            TestSongNotes = songRequestResult.notes;
        }

        private async Task<UserDto[]> GetAllUsers()
        {
            return await _mediator.Send(new QueryAll());
        }

        private async Task<(SongByIdDto song, SongVoiceDto voice, BarDto barWithNotes, BarDto barWithoutNotes, NoteDto[] notes)> GetTestSong()
        {
            var songByIdDto = await _mediator.Send(new QuerySongById(TestServerFixture.TestSongId));
            var songVoice = songByIdDto.Voices.FirstOrDefault();
            var songBarWithNotes = songVoice.Bars.FirstOrDefault(bar => bar.Position == 1);
            var songBarWithoutNotes = songVoice.Bars.FirstOrDefault(bar => bar.Position == 2);
            var songNotes = songBarWithNotes.Chords;
            return (songByIdDto, songVoice, songBarWithNotes, songBarWithoutNotes, songNotes);
        }

        [Fact]
        public async Task TestCreateSongNoteCommand()
        {
            //Execute commands to test.
            var firstNoteUpdated = await _mediator.Send(
                new CreateSongNoteCommand(TestSong.SongId, TestSongVoice.SongVoiceId, TestSongBarWithoutNotes.BarId, CreateNoteDto(0, 1, "Emaj7")));
            var secondNoteUpdated = await _mediator.Send(
                new CreateSongNoteCommand(TestSong.SongId, TestSongVoice.SongVoiceId, TestSongBarWithoutNotes.BarId, CreateNoteDto(1, 1, "D#")));
            var thirdNoteUpdated = await _mediator.Send(
                new CreateSongNoteCommand(TestSong.SongId, TestSongVoice.SongVoiceId, TestSongBarWithoutNotes.BarId, CreateNoteDto(2, 1, "Cm")));

            //Fetch data after commands have executed.
            var firstNote = await _mediator.Send(new QuerySongNoteById(firstNoteUpdated.SongChordId));
            var secondNote = await _mediator.Send(new QuerySongNoteById(secondNoteUpdated.SongChordId));
            var thirdNote = await _mediator.Send(new QuerySongNoteById(thirdNoteUpdated.SongChordId));

            //Verify that the data have been stored with correct values.
            firstNote.ChordName.ShouldBe("Emaj7", $"Failed for chord Emaj7 with position 0.");
            secondNote.ChordName.ShouldBe("D#", $"Failed for chord Emaj7 with position 1.");
            thirdNote.ChordName.ShouldBe("Cm", $"Failed for chord Emaj7 with position 2.");

            //Verify that a note cannot be created where there already exists a note.
            Should.Throw<ValidationException>(() =>
                _mediator.Send(
                    new CreateSongNoteCommand(TestSong.SongId, TestSongVoice.SongVoiceId, TestSongBarWithNotes.BarId, CreateNoteDto(2, 1, "Emaj7"))),
                "Note number already in use");

            //Verify that a user without writeaccess is not able to create data.
            _testServerFixture.ChangeCurrentUserId(SuppUser2.UserId);

            Should.Throw<UnauthorizedAccessException>(() =>
                _mediator.Send(new CreateSongNoteCommand(TestSong.SongId, TestSongVoice.SongVoiceId, TestSongBarWithoutNotes.BarId, CreateNoteDto(3, 1, "C"))));

            _testServerFixture.ChangeCurrentUserId(AdminUser.UserId);
        }

        [Fact]
        public async Task TestUpdateSongNoteCommand()
        {
            //Execute commands to test.
            var firstNoteUpdated = await _mediator.Send(
                new UpdateSongNoteCommand(TestSong.SongId, TestSongNotes[0].ChordId ?? 0, new UpdateNoteDto() { ChordName = "Gm13", Position = 0 }));
            var secondNoteUpdated = await _mediator.Send(
                new UpdateSongNoteCommand(TestSong.SongId, TestSongNotes[1].ChordId ?? 1, new UpdateNoteDto() { ChordName = "A", Position = 1 }));
            var thirdNoteUpdated = await _mediator.Send(
                new UpdateSongNoteCommand(TestSong.SongId, TestSongNotes[2].ChordId ?? 2, new UpdateNoteDto() { ChordName = "F#m", Position = 2 }));

            //Fetch data after commands have executed.
            var firstNote = await _mediator.Send(new QuerySongNoteById(firstNoteUpdated.SongChordId));
            var secondNote = await _mediator.Send(new QuerySongNoteById(secondNoteUpdated.SongChordId));
            var thirdNote = await _mediator.Send(new QuerySongNoteById(thirdNoteUpdated.SongChordId));

            //Verify that the data have been stored with correct values.
            firstNote.ChordName.ShouldBe("Gm13", $"Failed for chord Gm13 with position 0.");
            secondNote.ChordName.ShouldBe("A", $"Failed for chord A with position 1.");
            thirdNote.ChordName.ShouldBe("F#m", $"Failed for chord F#m with position 2.");

            //Verify that a user without write access is not able to change data.
            _testServerFixture.ChangeCurrentUserId(SuppUser2.UserId);

            Should.Throw<UnauthorizedAccessException>(() =>
                _mediator.Send(new UpdateSongNoteCommand(TestSong.SongId, firstNoteUpdated.SongChordId, new UpdateNoteDto() { ChordName = "G" })));

            _testServerFixture.ChangeCurrentUserId(AdminUser.UserId);
        }

        [Fact]
        public async Task TestDeleteSongNoteCommand()
        {
            //Execute command to test.
            var updatedSongNote = await _mediator.Send(new DeleteSongNoteCommand(TestSong.SongId, TestSongVoice.SongVoiceId, TestSongBarWithNotes.BarId, TestSongNotes[0].ChordId ?? 0));

            //Try fetch deleted data and verify that it is indeed deleted.
            Should.Throw<NotFoundException>(() =>
                _mediator.Send(new QuerySongNoteById(updatedSongNote.SongChordId)));

            //Verify that a user without write access is note able to delete data.
            _testServerFixture.ChangeCurrentUserId(SuppUser2.UserId);

            Should.Throw<UnauthorizedAccessException>(() =>
                _mediator.Send(new DeleteSongNoteCommand(TestSong.SongId, TestSongVoice.SongVoiceId, TestSongBarWithNotes.BarId, TestSongNotes[1].ChordId ?? 1)));

            _testServerFixture.ChangeCurrentUserId(AdminUser.UserId);
        }
    }
}
