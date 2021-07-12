using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dissimilis.DbContext.Models;
using Dissimilis.DbContext.Models.Song;
using Dissimilis.WebAPI.Controllers.BoSong;
using Dissimilis.WebAPI.Controllers.BoSong.Commands;
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
    /*
     * These tests arent working for some reason... The song's voices do not get loaded properly when trying to take a snapshot.
     * A quick check on the song object via a snapshot before passing the song as a param to the snapshot method does however show that every propery and voice is properly inserted...
     * Would be nice if someone could check on this, because I don't know what is causing the null-reference exception.
     */
    [Collection("Serial")]
    [CollectionDefinition("Serial", DisableParallelization = true)]
    public class SongExtensionTests : BaseTestClass
    {  
        private DateTimeOffset testDate = new DateTimeOffset(2000, 1, 1, 1, 1, 1, 1, new TimeSpan(0, 0, 0));

        public SongExtensionTests(TestServerFixture testServerFixture) : base(testServerFixture) { }

        public Song CreateSong()
        {
            Song testSong = new Song()
            {
                Title = "Test song",
                Composer = "Ole Bull",
                Numerator = 4,
                Denominator = 4,
                ArrangerId = 2,
                CreatedById = 2,
                CreatedOn = testDate,
                UpdatedById = 1,
                UpdatedOn = testDate,
                DegreeOfDifficulty = 1,
            };
            foreach (SongVoice voice in _CreateVoices(testSong.Id))
                testSong.Voices.Add(voice);

            return testSong;
        }
   
        private List<SongVoice> _CreateVoices(int songId = 0)
        {
            List<SongVoice> testVoices = new List<SongVoice>()
            {
                new SongVoice()
                {
                    VoiceName = "Song",
                    VoiceNumber = 1,
                    IsMainVoice = true,
                    CreatedById = 2,
                    CreatedOn = testDate,
                    UpdatedById = 1,
                    UpdatedOn = testDate,
                    SongId = songId,
                },
                new SongVoice()
                {
                    VoiceName = "Test voice 2",
                    VoiceNumber = 2,
                    IsMainVoice = false,
                    CreatedById = 2,
                    CreatedOn = testDate,
                    UpdatedById = 1,
                    UpdatedOn = testDate,
                    SongId = songId,
                }
            };
            foreach(SongVoice voice in testVoices)
            {
                foreach (SongBar bar in _CreateSongBars(voice.Id))
                {
                    bar.ShouldNotBeNull("Bar was null when adding to voice");
                    voice.SongBars.Add(bar);
                }
                
            };

            return testVoices;
        }

        private List<SongBar> _CreateSongBars(int voiceId)
        {
            List<SongBar> testBars = new List<SongBar>()
            {
                new SongBar()
                {
                    Position = 1,
                    SongVoiceId = voiceId,
                },
                new SongBar()
                {
                    Position = 2,
                    SongVoiceId = voiceId,
                },
                new SongBar()
                {
                    Position = 3,
                    SongVoiceId = voiceId,
                }
            };
            foreach(SongBar bar in testBars)
            {
                foreach (SongNote note in _CreateSongNotes(bar.Id))
                    bar.Notes.Add(note);
            }

            return testBars;
        }

        private List<SongNote> _CreateSongNotes(int barId)
        {
            return new List<SongNote>()
            {
                new SongNote()
                {
                    Position = 0,
                    ChordName = "C",
                    Length = 1,
                    NoteValues = "C|E|G",
                    BarId = barId
                },
                new SongNote()
                {
                    Position = 1,
                    ChordName = "Cm",
                    Length = 1,
                    NoteValues = "C|D#|G",
                    BarId = barId
                },
                new SongNote()
                {
                    Position = 2,
                    ChordName = null,
                    Length = 1,
                    NoteValues = "D",
                    BarId = barId
                },
                new SongNote()
                {
                    Position = 3,
                    ChordName = null,
                    Length = 1,
                    NoteValues = "C",
                    BarId = barId
                },
            };
        }

        public User CreateUser(List<Song> songsCreated, List<Song> songsUpdated, List<Song> songsArranged, List<SongVoice> songVoicesCreated, List<SongVoice> songVoicesUpdated)
        {
            return new User()
            {
                Email = "ole_bull@test.no",
                Name = "Ole Bull",
                SongsCreated = songsCreated,
                SongsArranged = songsArranged,
                SongsUpdated = songsUpdated,
                SongVoiceCreated = songVoicesCreated,
                SongVoiceUpdated = songVoicesUpdated
            };
        }

        [Fact]
        public void TestPerformSnapshot()
        {
            //var mediator = _testServerFixture.GetServiceProvider().GetService<IMediator>();

            Song testSong = CreateSong();
            var test = 0;
            testSong.ShouldNotBeNull("Song was null");
            SongSnapshot correctSnapshot = new SongSnapshot()
            {
                SongId = testSong.Id,
                CreatedById = (int)testSong.UpdatedById,
                CreatedOn = DateTimeOffset.Now,
                SongObjectJSON = Newtonsoft.Json.JsonConvert.SerializeObject(testSong)
            };
            testSong.ShouldBeNull($"Snapshot: {correctSnapshot.SongObjectJSON}");
            User testUser = CreateUser(new List<Song>{ testSong }, new List<Song>{ testSong }, new List<Song>{ testSong }, testSong.Voices.ToList<SongVoice>(), testSong.Voices.ToList<SongVoice>());
            testUser.ShouldNotBeNull("User was null");

            testSong.PerformSnapshot(testUser);
            SongSnapshot testSnapshot = testSong.Snapshots.ElementAt(0);
            testSnapshot.SongId.ShouldBeEquivalentTo(correctSnapshot.SongId, $"SongId {testSnapshot.SongId} did not match correct songId {correctSnapshot.SongId}");
            testSnapshot.CreatedById.ShouldBeEquivalentTo(correctSnapshot.SongId, $"CreatedById {testSnapshot.CreatedBy} did not match correct CreatedById {correctSnapshot.CreatedById}");
            testSnapshot.CreatedOn.ShouldBeInRange(correctSnapshot.CreatedOn, DateTimeOffset.Now, $"TestSnapshot.CreatedOn {testSnapshot.CreatedOn} did not fit correct CreatedOn {correctSnapshot.CreatedOn}");
            testSnapshot.SongObjectJSON.ShouldBeEquivalentTo(correctSnapshot.SongObjectJSON, $"JSON-snapshot strings did not match");
        }

        [Fact]
        public void TestUndoTitleChange()
        {
            Song testSong = CreateSong();
            string originalTitle = testSong.Title;
            User testUser = CreateUser(new List<Song> { testSong }, new List<Song> { testSong }, new List<Song> { testSong }, testSong.Voices.ToList<SongVoice>(), testSong.Voices.ToList<SongVoice>());
            testSong.PerformSnapshot(testUser);

            testSong.Title = "Changed title";
            testSong.Title.ShouldBeEquivalentTo("Changed title", $"Test failed while initially changing title value");

            testSong.Undo();
            testSong.Title.ShouldBeEquivalentTo(originalTitle, $"Undo title-change failed. Title was {testSong.Title}, but should have been {originalTitle}");
        }

        [Fact]
        public void TestUndoVoiceCreation()
        {
            Song testSong = CreateSong();
            int originalVoiceAmount = testSong.Voices.Count;

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

            testSong.Voices.ElementAt(0).SongBars.Remove(testSong.Voices.ElementAt(0).SongBars.ElementAt(0));
            testSong.Voices.ElementAt(0).SongBars.Count.ShouldBeEquivalentTo(originalBarAmount - 1, "Test failed while removing bar before undo");

            testSong.Undo();
            testSong.Voices.ElementAt(0).SongBars.Count.ShouldBeEquivalentTo(originalBarAmount, $"Undo of deleting bar failed. Bar amount should be {originalBarAmount}, but was {testSong.Voices.ElementAt(0).SongBars.Count}");
        }
    }
}
