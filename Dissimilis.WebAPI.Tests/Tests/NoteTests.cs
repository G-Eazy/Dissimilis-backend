using System;
using System.Linq;
using System.Threading.Tasks;
using Dissimilis.WebAPI.Controllers.BoNote.DtoModelsIn;
using Dissimilis.WebAPI.Controllers.BoBar.Commands;
using Dissimilis.WebAPI.Controllers.BoNote.Commands;
using Dissimilis.WebAPI.Controllers.BoNote.Query;
using Dissimilis.WebAPI.Controllers.BoSong.Commands;
using Dissimilis.WebAPI.Controllers.BoSong.Query;
using Dissimilis.WebAPI.Controllers.BoUser.Queries;
using Dissimilis.WebAPI.xUnit.Setup;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;
using static Dissimilis.WebAPI.xUnit.Extensions;
using Dissimilis.WebAPI.Exceptions;

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
            var mediator = _testServerFixture.GetServiceProvider().GetService<IMediator>();
            var users = await mediator.Send(new QueryAll());

            users.Length.ShouldBe(4);

            var songOwnerUser = users.SingleOrDefault(user => user.Email == "test@test.no");
            var songSharedUser = users.SingleOrDefault(user => user.Email == "supUser1@test.no");
            var sameOrgUser = users.SingleOrDefault(user => user.Email == "supUser2@test.no");
            var sameGroupUser = users.SingleOrDefault(user => user.Email == "supUser3@test.no");

            songOwnerUser.ShouldNotBeNull();
            songSharedUser.ShouldNotBeNull();
            sameOrgUser.ShouldNotBeNull();
            sameGroupUser.ShouldNotBeNull();

            var updatedSongCommandDto = await mediator.Send(new CreateSongCommand(CreateSongDto(4, 4)));
            var songDto = await mediator.Send(new QuerySongById(updatedSongCommandDto.SongId));
            var songVoice = songDto.Voices.FirstOrDefault();

            var newBar = await mediator.Send(new CreateSongBarCommand(songDto.SongId, songVoice.SongVoiceId, CreateBarDto()));

            var firstNoteUpdated = await mediator.Send(
                new CreateSongNoteCommand(songDto.SongId, songVoice.SongVoiceId, newBar.SongBarId, CreateNoteDto(0, 1, "Emaj7")));
            var secondNoteUpdated = await mediator.Send(
                new CreateSongNoteCommand(songDto.SongId, songVoice.SongVoiceId, newBar.SongBarId, CreateNoteDto(1, 1, "D#")));
            var thirdNoteUpdated = await mediator.Send(
                new CreateSongNoteCommand(songDto.SongId, songVoice.SongVoiceId, newBar.SongBarId, CreateNoteDto(2, 1, "Cm")));

            var firstNote = await mediator.Send(new QuerySongNoteById(firstNoteUpdated.SongChordId));
            firstNote.ChordName.ShouldBe("Emaj7", $"Failed for chord Emaj7 with position 0.");
            var secondNote = await mediator.Send(new QuerySongNoteById(secondNoteUpdated.SongChordId));
            secondNote.ChordName.ShouldBe("D#", $"Failed for chord Emaj7 with position 1.");
            var thirdNote = await mediator.Send(new QuerySongNoteById(thirdNoteUpdated.SongChordId));
            thirdNote.ChordName.ShouldBe("Cm", $"Failed for chord Emaj7 with position 2.");

            _testServerFixture.ChangeCurrentUserId(sameOrgUser.UserId);

            Should.Throw<UnauthorizedAccessException>(() =>
                mediator.Send(new CreateSongNoteCommand(songDto.SongId, songVoice.SongVoiceId, newBar.SongBarId, CreateNoteDto(3, 1, "C"))));

            _testServerFixture.ChangeCurrentUserId(songOwnerUser.UserId);
        }

        [Fact]
        public async Task TestUpdateSongNoteCommand()
        {
            var mediator = _testServerFixture.GetServiceProvider().GetService<IMediator>();
            var users = await mediator.Send(new QueryAll());

            users.Length.ShouldBe(4);

            var songOwnerUser = users.SingleOrDefault(user => user.Email == "test@test.no");
            var songSharedUser = users.SingleOrDefault(user => user.Email == "supUser1@test.no");
            var sameOrgUser = users.SingleOrDefault(user => user.Email == "supUser2@test.no");
            var sameGroupUser = users.SingleOrDefault(user => user.Email == "supUser3@test.no");

            songOwnerUser.ShouldNotBeNull();
            songSharedUser.ShouldNotBeNull();
            sameOrgUser.ShouldNotBeNull();
            sameGroupUser.ShouldNotBeNull();

            var updatedSongCommandDto = await mediator.Send(new CreateSongCommand(CreateSongDto(4, 4)));
            var songDto = await mediator.Send(new QuerySongById(updatedSongCommandDto.SongId));
            var songVoice = songDto.Voices.FirstOrDefault();

            var newBar = await mediator.Send(new CreateSongBarCommand(songDto.SongId, songVoice.SongVoiceId, CreateBarDto()));

            var firstNoteUpdated = await mediator.Send(
                new CreateSongNoteCommand(songDto.SongId, songVoice.SongVoiceId, newBar.SongBarId, CreateNoteDto(0, 1, "Emaj7")));
            var secondNoteUpdated = await mediator.Send(
                new CreateSongNoteCommand(songDto.SongId, songVoice.SongVoiceId, newBar.SongBarId, CreateNoteDto(1, 1, "D#")));
            var thirdNoteUpdated = await mediator.Send(
                new CreateSongNoteCommand(songDto.SongId, songVoice.SongVoiceId, newBar.SongBarId, CreateNoteDto(2, 1, "Cm")));

            var firstNote = await mediator.Send(new QuerySongNoteById(firstNoteUpdated.SongChordId));
            firstNote.ChordName.ShouldBe("Emaj7", $"Failed for chord Emaj7 with position 0.");
            var secondNote = await mediator.Send(new QuerySongNoteById(secondNoteUpdated.SongChordId));
            secondNote.ChordName.ShouldBe("D#", $"Failed for chord Emaj7 with position 1.");
            var thirdNote = await mediator.Send(new QuerySongNoteById(thirdNoteUpdated.SongChordId));
            thirdNote.ChordName.ShouldBe("Cm", $"Failed for chord Emaj7 with position 2.");

            firstNoteUpdated = await mediator.Send(
                new UpdateSongNoteCommand(songDto.SongId, firstNoteUpdated.SongChordId, new UpdateNoteDto() { ChordName = "Gm13", Position = 0 }));
            secondNoteUpdated = await mediator.Send(
                new UpdateSongNoteCommand(songDto.SongId, secondNoteUpdated.SongChordId, new UpdateNoteDto() { ChordName = "A", Position = 1 }));
            thirdNoteUpdated = await mediator.Send(
                new UpdateSongNoteCommand(songDto.SongId, thirdNoteUpdated.SongChordId, new UpdateNoteDto() { ChordName = "F#m", Position = 2 }));

            firstNote = await mediator.Send(new QuerySongNoteById(firstNoteUpdated.SongChordId));
            firstNote.ChordName.ShouldBe("Gm13", $"Failed for chord Gm13 with position 0.");
            secondNote = await mediator.Send(new QuerySongNoteById(secondNoteUpdated.SongChordId));
            secondNote.ChordName.ShouldBe("A", $"Failed for chord A with position 1.");
            thirdNote = await mediator.Send(new QuerySongNoteById(thirdNoteUpdated.SongChordId));
            thirdNote.ChordName.ShouldBe("F#m", $"Failed for chord F#m with position 2.");

            _testServerFixture.ChangeCurrentUserId(sameOrgUser.UserId);

            Should.Throw<UnauthorizedAccessException>(() =>
                mediator.Send(new UpdateSongNoteCommand(songDto.SongId, firstNoteUpdated.SongChordId, new UpdateNoteDto() { ChordName = "G" })));

            _testServerFixture.ChangeCurrentUserId(songOwnerUser.UserId);
        }

        [Fact]
        public async Task TestDeleteSongNoteCommand()
        {
            var mediator = _testServerFixture.GetServiceProvider().GetService<IMediator>();
            var users = await mediator.Send(new QueryAll());

            users.Length.ShouldBe(4);

            var songOwnerUser = users.SingleOrDefault(user => user.Email == "test@test.no");
            var songSharedUser = users.SingleOrDefault(user => user.Email == "supUser1@test.no");
            var sameOrgUser = users.SingleOrDefault(user => user.Email == "supUser2@test.no");
            var sameGroupUser = users.SingleOrDefault(user => user.Email == "supUser3@test.no");

            songOwnerUser.ShouldNotBeNull();
            songSharedUser.ShouldNotBeNull();
            sameOrgUser.ShouldNotBeNull();
            sameGroupUser.ShouldNotBeNull();

            var updatedSongCommandDto = await mediator.Send(new CreateSongCommand(CreateSongDto(4, 4)));
            var songDto = await mediator.Send(new QuerySongById(updatedSongCommandDto.SongId));
            var songVoice = songDto.Voices.FirstOrDefault();

            var newBar = await mediator.Send(new CreateSongBarCommand(songDto.SongId, songVoice.SongVoiceId, CreateBarDto()));

            var firstNote = await mediator.Send(
                new CreateSongNoteCommand(songDto.SongId, songVoice.SongVoiceId, newBar.SongBarId, CreateNoteDto(0, 1, "Emaj7")));
            var secondNote = await mediator.Send(
                new CreateSongNoteCommand(songDto.SongId, songVoice.SongVoiceId, newBar.SongBarId, CreateNoteDto(1, 1, "D#")));
            var thirdNote = await mediator.Send(
                new CreateSongNoteCommand(songDto.SongId, songVoice.SongVoiceId, newBar.SongBarId, CreateNoteDto(2, 1, "Cm")));

            mediator.Send(new QuerySongNoteById(firstNote.SongChordId)).Result.ChordName.ShouldBe("Emaj7", $"Failed for chord Emaj7 with position 0.");
            mediator.Send(new QuerySongNoteById(secondNote.SongChordId)).Result.ChordName.ShouldBe("D#", $"Failed for chord Emaj7 with position 1.");
            mediator.Send(new QuerySongNoteById(thirdNote.SongChordId)).Result.ChordName.ShouldBe("Cm", $"Failed for chord Emaj7 with position 2.");

            await mediator.Send(new DeleteSongNoteCommand(songDto.SongId, songVoice.SongVoiceId, newBar.SongBarId, firstNote.SongChordId));

            Should.Throw<NotFoundException>(() =>
                mediator.Send(new QuerySongNoteById(firstNote.SongChordId)));

            _testServerFixture.ChangeCurrentUserId(sameOrgUser.UserId);

            Should.Throw<UnauthorizedAccessException>(() =>
                mediator.Send(new DeleteSongNoteCommand(songDto.SongId, songVoice.SongVoiceId, newBar.SongBarId, secondNote.SongChordId)));

            _testServerFixture.ChangeCurrentUserId(songOwnerUser.UserId);
        }
    }
}
