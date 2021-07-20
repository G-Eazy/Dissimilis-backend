using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using Dissimilis.DbContext.Models;
using Dissimilis.DbContext.Models.Enums;
using Dissimilis.DbContext.Models.Song;
using Dissimilis.WebAPI.Extensions.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.PlatformAbstractions;

namespace Dissimilis.WebAPI.xUnit.Setup
{
    public class TestServerFixture : IDisposable
    {
        private readonly TestServer _testServer;
        public HttpClient Client { get; }
        public static int CurrentUserId;

        public TestServerFixture()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile(Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "TestEnviromentVariables.json"), optional: true)
                .Build();

            _testServer = new TestServer(
                new WebHostBuilder()
                    .UseConfiguration(config)
                    .UseStartup<TestStartup>());

            Client = _testServer.CreateClient();
        }

        internal IServiceProvider GetServiceProvider()
        {
            return _testServer.Host.Services;
        }

        public void Dispose()
        {
            Client.Dispose();
            _testServer.Dispose();
        }

        public static List<User> GetTestUsers()
        {
            return new List<User>()
            {
                new User()
                {
                    Name = "SysAdminUser",
                    Email = "SysAdmin@Norway.no",
                    IsSystemAdmin = true,
                },
                new User()
                {
                    Name = "OrgAdminNorwayUser",
                    Email = "Admin@Norway.no",
                },
                new User()
                {
                    Name = "GroupAdminSandvikaUser",
                    Email = "Admin@Sandvika_Norway.no",
                },
                new User()
                {
                    Name = "GroupAdminSandvikaUser",
                    Email = "Admin@Bergen_Norway.no",
                },
                new User()
                {
                    Name = "GroupAdminSandvikaUser",
                    Email = "Admin@Trondheim_Norway.no",
                },
                new User()
                {
                    Name = "OrgAdminGuatamalaUser",
                    Email = "Admin@Guatamala.no",
                },
                new User()
                {
                    Name = "Justin Bieber fan",
                    Email = "Justin_Bieber_fan@Norway.no",
                },
                new User()
                {
                    Name = "Edvard Grieg fan",
                    Email = "Edvard_Grieg_fan@Sandvika_Norway.no",
                },
                new User()
                {
                    Name = "Deep Purple fan",
                    Email = "Deep_Purple_fan@Trondheim_Norway.no",
                },
                new User()
                {
                    Name = "Rammstein fan",
                    Email = "Rammstein_fan@Norway.no",
                }
            };
        }

        public static void ChangeCurrentUserId(int newCurrentUserId)
        {
            TestServerFixture.CurrentUserId = newCurrentUserId;
        }

        public static List<Organisation> GetTestOrganisations()
        {
            return new List<Organisation>()
            {
                new Organisation()
                {
                    Name = "Norway"
                },
                new Organisation()
                {
                    Name = "Guatamala"
                },
            };
        }

        public static List<Group> GetTestGroups()
        {
            return new List<Group>()
            {
                new Group()
                {
                    Name = "Sandvika_Norway",
                },
                new Group()
                {
                    Name = "Bergen_Norway",
                },
                new Group()
                {
                    Name = "Trondheim_Norway",
                },
            };
        }

        public static List<Song> GetTestSongs()
        {
            return new List<Song>()
            {
                new Song()
                {
                    Title = "Lisa gikk til skolen",
                    Numerator = 4,
                    Denominator = 4,
                    ProtectionLevel = ProtectionLevels.Public
                },
                new Song()
                {
                    Title = "Smoke on the water",
                    Composer = "Deep_Purple",
                    Numerator = 4,
                    Denominator = 4,
                    ProtectionLevel = ProtectionLevels.Private
                },
                new Song()
                {
                    Title = "Speed King",
                    Composer = "Deep_Purple",
                    Numerator = 4,
                    Denominator = 4,
                    ProtectionLevel = ProtectionLevels.Public
                },
                new Song()
                {
                    Title = "Du hast",
                    Composer = "Rammstein",
                    Numerator = 4,
                    Denominator = 4,
                    ProtectionLevel = ProtectionLevels.Private
                },
                new Song()
                {
                    Title = "Baby",
                    Composer = "Justin_Bieber",
                    Numerator = 4,
                    Denominator = 4,
                    ProtectionLevel = ProtectionLevels.Private
                },
                new Song()
                {
                    Title = "Dovregubbens hall",
                    Composer = "Edvard_Grieg",
                    Numerator = 4,
                    Denominator = 4,
                    ProtectionLevel = ProtectionLevels.Public
                },
            };
        }


        public static List<SongVoice> GetTestSongVoices()
        {
            return new List<SongVoice>()
            {
                new SongVoice()
                {
                    VoiceName = "Main",
                    IsMainVoice = true,
                    VoiceNumber = 1,
                },
                new SongVoice()
                {
                    VoiceName = "First voice",
                    IsMainVoice = false,
                    VoiceNumber = 2,
                },
                new SongVoice()
                {
                    VoiceName = "Second voice",
                    IsMainVoice = false,
                    VoiceNumber = 3,
                },
            };
        }

        public static SongBar[] GetTestSongBars()
        {
            return new SongBar[]
            {
                new SongBar()
                {
                    House = null,
                    RepAfter = false,
                    RepBefore = false,
                    Position = 1,
                },
                new SongBar()
                {
                    House = null,
                    RepAfter = false,
                    RepBefore = false,
                    Position = 2,
                },
                new SongBar()
                {
                    House = 1,
                    RepAfter = false,
                    RepBefore = false,
                    Position = 1,
                },
                new SongBar()
                {
                    House = 1,
                    RepAfter = false,
                    RepBefore = false,
                    Position = 2,
                }
            };
        }

        public static List<SongNote> GetTestSongNotes()
        {
            return new List<SongNote>()
            {
                new SongNote()
                {
                    Position = 0,
                    Length = 1,
                    ChordName = "D",
                    NoteValues = String.Join("|", SongNoteExtension.GetNoteValuesFromChordName("D")),
                },
                new SongNote()
                {
                    Position = 1,
                    Length = 1,
                    ChordName = "F#m",
                    NoteValues = String.Join("|", SongNoteExtension.GetNoteValuesFromChordName("F#m")),
                },
                new SongNote()
                {
                    Position = 2,
                    Length = 2,
                    ChordName = "D#13",
                    NoteValues = String.Join("|", SongNoteExtension.GetNoteValuesFromChordName("D#13")),
                },
                new SongNote()
                {
                    Position = 3,
                    Length = 2,
                    ChordName = "A",
                    NoteValues = String.Join("|", SongNoteExtension.GetNoteValuesFromChordName("A")),
                },
            };
        }
    }
}
