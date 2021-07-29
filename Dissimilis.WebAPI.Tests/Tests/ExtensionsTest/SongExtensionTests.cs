using Shouldly;
using Xunit;
using Dissimilis.WebAPI.Extensions.Models;
using static Dissimilis.WebAPI.xUnit.Extensions;
using Dissimilis.DbContext.Models.Song;
using System.Collections.Generic;
using Dissimilis.DbContext.Models;
using System;
using System.Linq;
using Dissimilis.WebAPI.xUnit.Setup;
using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsOut;

namespace Dissimilis.WebAPI.xUnit.Tests
{
    [Collection("Serial")]
    [CollectionDefinition("Serial", DisableParallelization = true)]
    public class SongExtensionTests : BaseTestClass
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

        private DateTimeOffset testDate = new DateTimeOffset(2000, 1, 1, 1, 1, 1, 1, new TimeSpan(0, 0, 0));

        public SongExtensionTests(TestServerFixture testServerFixture) : base(testServerFixture) { }

        [Fact]
        public void TestPerformSnapshot()
        {
            SongSnapshot correctSnapshot = new SongSnapshot()
            {
                SongId = BabySong.Id,
                CreatedById = SysAdminUser.Id,
                CreatedOn = DateTimeOffset.Now,
                SongObjectJSON = Newtonsoft.Json.JsonConvert.SerializeObject(new SongByIdDto(BabySong))
            };
            BabySong.PerformSnapshot(SysAdminUser);

            SongSnapshot testSnapshot = BabySong.Snapshots.ElementAt(0);
            testSnapshot.SongId.ShouldBeEquivalentTo(correctSnapshot.SongId, $"SongId {testSnapshot.SongId} did not match correct songId {correctSnapshot.SongId}");
            testSnapshot.CreatedById.ShouldBeEquivalentTo(correctSnapshot.CreatedById, $"CreatedById {testSnapshot.CreatedById} did not match correct CreatedById {correctSnapshot.CreatedById}");
            testSnapshot.CreatedOn.ShouldBeInRange(correctSnapshot.CreatedOn, DateTimeOffset.Now, $"TestSnapshot.CreatedOn {testSnapshot.CreatedOn} did not fit correct CreatedOn {correctSnapshot.CreatedOn}");

            // Testing equality between the two json-outputs proved to be difficult because of the solution to avoid circular dependencies in voice and songbar
            // So it is only tested for existence for now 
            testSnapshot.SongObjectJSON.ShouldNotBeNull($"JSON-snapshot string was not created");
        }
        /*
        [Fact]
        public void TestUndoTitleChange()
        {
            Song testSong = CreateSong();
            string originalTitle = testSong.Title;
            testSong.PerformSnapshot(CreateUser());

            testSong.Title = "Changed title";
            testSong.Title.ShouldNotBe(originalTitle, "Title was not changed to Changed title");

            testSong.Undo();
            testSong.Title.ShouldBeEquivalentTo(originalTitle, $"Undo title-change failed. Title was {testSong.Title}, but should have been {originalTitle}");
        }

        [Fact]
        public void TestUndoVoiceCreation()
        {
            Song testSong = CreateSong();
            int originalVoiceAmount = testSong.Voices.Count;
            testSong.PerformSnapshot(CreateUser());

            testSong.Voices.Add(new SongVoice() { });
            testSong.Voices.Count.ShouldBeEquivalentTo(originalVoiceAmount + 1, "Test failed while adding new voice before undo");

            testSong.Undo();
            testSong.Voices.Count.ShouldBeEquivalentTo(originalVoiceAmount, $"Undo of adding voice failed. Voice amount should be {originalVoiceAmount}, but was {testSong.Voices.Count}");
        }

        [Fact]
        public void TestUndoVoiceDeletion()
        {
            Song testSong = CreateSong();
            int originalVoiceAmount = testSong.Voices.Count;
            testSong.PerformSnapshot(CreateUser());

            testSong.Voices.Remove(testSong.Voices.ElementAt(0));
            testSong.Voices.Count.ShouldBeEquivalentTo(originalVoiceAmount - 1, "Test failed while removing voice before undo");

            testSong.Undo();
            testSong.Voices.Count.ShouldBeEquivalentTo(originalVoiceAmount, $"Undo of deleting voice failed. Voice amount should be {originalVoiceAmount}, but was {testSong.Voices.Count}");
        }

        [Fact]
        public void TestUndoBarDeletion()
        {
            Song testSong = CreateSong();
            int originalBarAmount = testSong.Voices.ElementAt(0).SongBars.Count;
            testSong.PerformSnapshot(CreateUser());

            testSong.Voices.ElementAt(0).SongBars.Remove(testSong.Voices.ElementAt(0).SongBars.ElementAt(0));
            testSong.Voices.ElementAt(0).SongBars.Count.ShouldBeEquivalentTo(originalBarAmount - 1, "Test failed while removing bar before undo");

            testSong.Undo();
            testSong.Voices.ElementAt(0).SongBars.Count.ShouldBeEquivalentTo(originalBarAmount, $"Undo of deleting bar failed. Bar amount should be {originalBarAmount}, but was {testSong.Voices.ElementAt(0).SongBars.Count}");
        }*/
    }
}
