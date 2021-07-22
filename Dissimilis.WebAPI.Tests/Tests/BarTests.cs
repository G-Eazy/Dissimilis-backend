using Dissimilis.WebAPI.Controllers.BoBar.Commands;
using Dissimilis.WebAPI.xUnit.Setup;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Shouldly;
using static Dissimilis.WebAPI.xUnit.Extensions;
using System;
using Dissimilis.DbContext.Models.Song;
using Microsoft.EntityFrameworkCore;

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
        public async Task TestCreateSongBarCommand()
        {
            using (var transaction = _testServerFixture.GetContext().Database.BeginTransaction())
            {
                try
                {
                    transaction.CreateSavepoint("BeforeTestRun");
                    //Fetch voice to add bars to.
                    var voice = SmokeOnTheWaterSong
                        .Voices.First();

                    //Test edgecases
                    await TestCreateBarWhenCurrentUserIsArrangerShouldCreate(SmokeOnTheWaterSong.Id, voice.Id);
                    await TestCreateBarWhenCurrentUserDoesNotHaveWriteAccessShouldNotCreate(SmokeOnTheWaterSong.Id, voice.Id);
                }
                finally
                {
                    transaction.RollbackToSavepoint("BeforeTestRun");
                }
            };
        }

        private async Task TestCreateBarWhenCurrentUserIsArrangerShouldCreate(int songId, int voiceId)
        {
            //Set current user to arranger of song.
            TestServerFixture.ChangeCurrentUserId(DeepPurpleFanUser.Id);

            //Execute command for creating bar.
            var barUpdatedDto = await _mediator.Send(new CreateSongBarCommand(songId, voiceId, CreateBarDto()));

            //Fetch song and verify that bar is added to database.
            UpdateAllSongs();

            SmokeOnTheWaterSong
                .Voices.SingleOrDefault(voice => voice.Id == voiceId)
                .SongBars.Any(bar => bar.Id == barUpdatedDto.SongBarId)
                .ShouldBeTrue();
        }

        private async Task TestCreateBarWhenCurrentUserDoesNotHaveWriteAccessShouldNotCreate(int songId, int voiceId)
        {
            //Set current user to a user without write access.
            TestServerFixture.ChangeCurrentUserId(EdvardGriegFanUser.Id);

            //Execute command for creating bar and verify that it throws correct exception.
            await Should.ThrowAsync<UnauthorizedAccessException>(async () =>
                await _mediator.Send(new CreateSongBarCommand(songId, voiceId, CreateBarDto())));
        }

        [Fact]
        public async Task TestUpdateBarCommand()
        {
            using (var transaction = _testServerFixture.GetContext().Database.BeginTransaction())
            {
                try
                {
                    transaction.CreateSavepoint("BeforeTestRun");
                    //Fetch voice to add bars to.
                    var voice = SmokeOnTheWaterSong
                        .Voices.First();
                    var barToUpdate = voice
                        .SongBars.First();

                    //Test edgecases
                    await TestUpdateBarWhenCurrentUserIsArrangerShouldUpdate(SmokeOnTheWaterSong.Id, voice.Id, barToUpdate);
                    await TestUpdateBarWhenCurrentUserDoesNotHaveWriteAccessShouldNotUpdate(SmokeOnTheWaterSong.Id, voice.Id, barToUpdate);
                }
                finally
                {
                    transaction.RollbackToSavepoint("BeforeTestRun");
                }
            };
            
        }

        private async Task TestUpdateBarWhenCurrentUserIsArrangerShouldUpdate(int songId, int voiceId, SongBar barBeforeUpdate)
        {
            //Set current user to arranger of song.
            TestServerFixture.ChangeCurrentUserId(DeepPurpleFanUser.Id);

            //Execute command for updating bar.
            var barUpdatedDto = await _mediator.Send(new UpdateSongBarCommand(songId, voiceId, barBeforeUpdate.Id, CreateUpdateBarDto(2, true, false)));

            //Fetch song and verify that changes to bar is added to database.
            UpdateAllSongs();

            var updatedBar = SmokeOnTheWaterSong
                .Voices.SingleOrDefault(voice => voice.Id == voiceId)
                .SongBars.SingleOrDefault(bar => bar.Id == barBeforeUpdate.Id);

            updatedBar.House.ShouldBe(2);
            updatedBar.RepAfter.ShouldBeTrue();
            updatedBar.RepBefore.ShouldBeFalse();
        }

        private async Task TestUpdateBarWhenCurrentUserDoesNotHaveWriteAccessShouldNotUpdate(int songId, int voiceId, SongBar barBeforeUpdate)
        {
            //Set current user to a user without write access.
            TestServerFixture.ChangeCurrentUserId(EdvardGriegFanUser.Id);

            //Execute command for creating bar and verify that it throws correct exception.
            await Should.ThrowAsync<UnauthorizedAccessException>(async () =>
                await _mediator.Send(new UpdateSongBarCommand(songId, voiceId, barBeforeUpdate.Id, CreateUpdateBarDto(2, true, false))));

            //Fetch song and verify that changes to bar is not added to database.
            UpdateAllSongs();

            var updatedBar = SmokeOnTheWaterSong
                .Voices.SingleOrDefault(voice => voice.Id == voiceId)
                .SongBars.SingleOrDefault(bar => bar.Id == barBeforeUpdate.Id);

            updatedBar.House.ShouldBe(barBeforeUpdate.House);
            updatedBar.RepAfter.ShouldBe(barBeforeUpdate.RepAfter);
            updatedBar.RepBefore.ShouldBe(barBeforeUpdate.RepBefore);
        }

        [Fact]
        public async Task TestDeleteSongBarCommand()
        {
            using (var transaction = _testServerFixture.GetContext().Database.BeginTransaction())
            {
                try
                {
                    transaction.CreateSavepoint("BeforeTestRun");
                    //Fetch bar to use for tests
                    var voice = SpeedKingSong
                        .Voices.FirstOrDefault();
                    var barToDeleteWithPermittedUser = voice
                        .SongBars.FirstOrDefault(bar => bar.Notes.Count == 4);
                    var barToDeleteWithUnpermittedUser = voice
                        .SongBars.FirstOrDefault(bar => bar.Notes.Count == 0);

                    //Test edgecases
                    await TestDeleteBarWhenCurrentUserIsArrangerShouldDelete(SpeedKingSong.Id, voice.Id, barToDeleteWithPermittedUser.Id);
                    await TestDeleteBarWhenCurrentUserDoesNotHaveWriteAccessShouldNotDelete(SpeedKingSong.Id, voice.Id, barToDeleteWithUnpermittedUser.Id);
                }
                finally
                {
                    transaction.RollbackToSavepoint("BeforeTestRun");
                }
            };
        }

        private async Task TestDeleteBarWhenCurrentUserIsArrangerShouldDelete(int songId, int voiceId, int barId)
        {
            //Change current user to arranger of song.
            TestServerFixture.ChangeCurrentUserId(DeepPurpleFanUser.Id);

            //Execute command for deleting bar.
            var updatedSongNote = await _mediator.Send(new DeleteSongBarCommand(songId, voiceId, barId));

            //Fetch song and verify that bar is deleted from database.
            UpdateAllSongs();

            SpeedKingSong
                .Voices.SingleOrDefault(voice => voice.Id == voiceId)
                .SongBars.Any(bar => bar.Id == barId)
                .ShouldBeFalse();
        }

        private async Task TestDeleteBarWhenCurrentUserDoesNotHaveWriteAccessShouldNotDelete(int songId, int voiceId, int barId)
        {
            //Change user to one without write access.
            TestServerFixture.ChangeCurrentUserId(EdvardGriegFanUser.Id);

            //Verify that correct exception is thrown.
            await Should.ThrowAsync<UnauthorizedAccessException>(async () =>
                await _mediator.Send(new DeleteSongBarCommand(songId, voiceId, barId)));

            //Fetch song and verify that bar is still in database.
            UpdateAllSongs();

            SpeedKingSong
                .Voices.SingleOrDefault(voice => voice.Id == voiceId)
                .SongBars.Any(bar => bar.Id == barId)
                .ShouldBeTrue();
        }

    }
}
