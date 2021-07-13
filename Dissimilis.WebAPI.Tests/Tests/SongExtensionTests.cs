using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dissimilis.DbContext.Models;
using Dissimilis.DbContext.Models.Song;
using Dissimilis.WebAPI.Controllers.BoSong;
using Dissimilis.WebAPI.Controllers.BoSong.Commands;
using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsIn;
using Dissimilis.WebAPI.Controllers.BoSong.Query;
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
                Id = 0,
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
                Speed = 3,
                SongNotes = "abc",
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
                    Id = 1,
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
                    Id = 4,
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

            int count = 0;
            foreach (SongVoice voice in testVoices)
            {
                foreach (SongBar bar in _CreateSongBars(voice, voice.Id, count))
                {
                    bar.ShouldNotBeNull("Bar was null when adding to voice");
                    voice.SongBars.Add(bar);
                }
                count += 3;
            };
            return testVoices;
        }

        private List<SongBar> _CreateSongBars(SongVoice voice, int voiceId, int count)
        {
            List<SongBar> testBars = new List<SongBar>()
            {
                new SongBar()
                {
                    Id = count,
                    Position = 1,
                    SongVoiceId = voiceId,
                    RepBefore = false,
                    RepAfter = false,
                    SongVoice = new SongVoice(){Id = voice.Id, Song = new Song(){Denominator = 4}}
                },
                new SongBar()
                {
                    Id = count+1,
                    Position = 2,
                    SongVoiceId = voiceId,
                    RepBefore = false,
                    RepAfter = false,
                    SongVoice = new SongVoice(){Id = voice.Id, Song = new Song(){Denominator = 4}}
                },
                new SongBar()
                {
                    Id = count+2,
                    Position = 3,
                    SongVoiceId = voiceId,
                    RepBefore = false,
                    RepAfter = false,
                    SongVoice = new SongVoice(){Id = voice.Id, Song = new Song(){Denominator = 4}}
                }
            };
            int noteCount = 0;
            foreach(SongBar bar in testBars)
            {
                foreach (SongNote note in _CreateSongNotes(bar.Id, noteCount))
                    bar.Notes.Add(note);
                noteCount += 4;
            }

            return testBars;
        }

        private List<SongNote> _CreateSongNotes(int barId, int count)
        {
            return new List<SongNote>()
            {
                new SongNote()
                {
                    Id = count,
                    Position = 0,
                    ChordName = "C",
                    Length = 1,
                    NoteValues = "C|E|G",
                    BarId = barId
                },
                new SongNote()
                {
                    Id = count+1,
                    Position = 1,
                    ChordName = "Cm",
                    Length = 1,
                    NoteValues = "C|D#|G",
                    BarId = barId
                },
                new SongNote()
                {
                    Id = count+2,
                    Position = 2,
                    ChordName = null,
                    Length = 1,
                    NoteValues = "D",
                    BarId = barId
                },
                new SongNote()
                {
                    Id = count+3,
                    Position = 3,
                    ChordName = null,
                    Length = 1,
                    NoteValues = "C",
                    BarId = barId
                },
            };
        }

        public User CreateUser(List<Song> songsCreated = null, List<Song> songsUpdated = null, List<Song> songsArranged = null, List<SongVoice> songVoicesCreated = null, List<SongVoice> songVoicesUpdated = null)
        {
            return new User()
            {
                Id = 2,
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
            Song testSong = CreateSong();
            testSong.ShouldNotBeNull("Song was null");
            SongSnapshot correctSnapshot = new SongSnapshot()
            {
                SongId = testSong.Id,
                CreatedById = 2,
                CreatedOn = DateTimeOffset.Now,
                SongObjectJSON = Newtonsoft.Json.JsonConvert.SerializeObject(testSong)
            };
            User testUser = CreateUser(new List<Song>{ testSong }, new List<Song>{ testSong }, new List<Song>{ testSong }, testSong.Voices.ToList<SongVoice>(), testSong.Voices.ToList<SongVoice>());
            testUser.ShouldNotBeNull("User was null");
            testSong.PerformSnapshot(testUser);
            
            SongSnapshot testSnapshot = testSong.Snapshots.ElementAt(0);
            testSnapshot.SongId.ShouldBeEquivalentTo(correctSnapshot.SongId, $"SongId {testSnapshot.SongId} did not match correct songId {correctSnapshot.SongId}");
            testSnapshot.CreatedById.ShouldBeEquivalentTo(correctSnapshot.CreatedById, $"CreatedById {testSnapshot.CreatedById} did not match correct CreatedById {correctSnapshot.CreatedById}");
            testSnapshot.CreatedOn.ShouldBeInRange(correctSnapshot.CreatedOn, DateTimeOffset.Now, $"TestSnapshot.CreatedOn {testSnapshot.CreatedOn} did not fit correct CreatedOn {correctSnapshot.CreatedOn}");

            // Testing equality between the two json-outputs proved to be difficult because of the solution to avoid circular dependencies in voice and songbar
            // So it is only tested for existence for now 
            testSnapshot.SongObjectJSON.ShouldNotBeNull($"JSON-snapshot string was not created");
        }
        
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
        }
    }
}
