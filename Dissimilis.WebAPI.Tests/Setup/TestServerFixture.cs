using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using Dissimilis.DbContext.Models;
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
        public static int CurrentUserId = GetDefaultTestUser().Id;
        public static int TestSongId = GetDefaultTestSong().Id;

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

        public static User GetDefaultTestUser()
        {
            return new User()
            {
                Name = "Testuser",
                Email = "test@test.no",
                IsSystemAdmin = true,
            };
        }

        public static List<User> GetSupplementedTestUsers()
        {
            return new List<User>()
            {
                new User()
                {
                    Name = "SupUser1",
                    Email = "supUser1@test.no",
                },
                new User()
                {
                    Name = "SupUser2",
                    Email = "supUser2@test.no",
                },
                new User()
                {
                    Name = "SupUser3",
                    Email = "supUser3@test.no",
                }
            };
        }

        public static void ChangeCurrentUserId(int newCurrentUserId)
        {
            TestServerFixture.CurrentUserId = newCurrentUserId;
        }

        public static Organisation GetDefaultTestOrganisation()
        {
            return new Organisation()
            {
                Name = "Norway",
            };
        }

        public static Organisation GetSupplementOrganisation()
        {
            return new Organisation(){
                    Name = "Guatamala"
            };
        }

        public static OrganisationUser GetDefaultTestOrganisationUser()
        {
            return new OrganisationUser()
            {
                Role = DbContext.Models.Enums.Role.Admin,
            };
        }

        public static List<OrganisationUser> GetSupplementedTestOrganisationUsers()
        {
            return new List<OrganisationUser>()
            {
                new OrganisationUser()
                {
                    Role = DbContext.Models.Enums.Role.Member,
                },
                new OrganisationUser()
                {
                    Role = DbContext.Models.Enums.Role.Member,
                },
                new OrganisationUser()
                {
                    Role = DbContext.Models.Enums.Role.Member,
                },
            };
        }

        public static List<SongSharedOrganisation> GetDefaultSongSharedOrganisations()
        {
            return new List<SongSharedOrganisation>()
            {
                new SongSharedOrganisation() { },
                new SongSharedOrganisation() { },
                new SongSharedOrganisation() { },
                new SongSharedOrganisation() { },
            };
        }

        public static List<SongSharedGroup> GetDefaultSongSharedGroups()
        {
            return new List<SongSharedGroup>()
            {
                new SongSharedGroup() { },
                new SongSharedGroup() { },
                new SongSharedGroup() { },
                new SongSharedGroup() { },
            };
        }

        public static SongSharedUser GetDefaultSongSharedUser()
        {
            return new SongSharedUser() { };
        }

        public static List<Group> GetTestGroups()
        {
            return new List<Group>()
            {
                new Group()
                {
                    Name = "Dissimilis Sandvika",
                },
                new Group()
                {
                    Name = "Dissimilis Bergen",
                },new Group()
                {
                    Name = "Dissimilis Trondheim",
                },
            };
        }

        public static GroupUser GetDefaultTestGroupUser()
        {
            return new GroupUser()
            {
                Role = DbContext.Models.Enums.Role.Admin,
            };
        }

        public static List<GroupUser> GetSupplementedTestGroupUsers()
        {
            return new List<GroupUser>()
            {
                new GroupUser()
                {
                    Role = DbContext.Models.Enums.Role.Member,
                },
                new GroupUser()
                {
                    Role = DbContext.Models.Enums.Role.Member,
                },
                new GroupUser()
                {
                    Role = DbContext.Models.Enums.Role.Member,
                },
            };
        }

        public static Song GetDefaultTestSong()
        {
            return new Song()
            {
                Title = "Default test song",
                Numerator = 4,
                Denominator = 4,
            };
        }

        public static List<Song> GetSupplementTestSongs()
        {
            return new List<Song>()
            {
                new Song()
                {
                    Title = "Supplement test song 1",
                    Numerator = 4,
                    Denominator = 4,
                },
                new Song()
                {
                    Title = "Supplement test song 2",
                    Numerator = 4,
                    Denominator = 4,
                },
                new Song()
                {
                    Title = "Supplement test song 3",
                    Numerator = 4,
                    Denominator = 4,
                },
                new Song()
                {
                    Title = "Supplement test song 4",
                    Numerator = 4,
                    Denominator = 4,
                },
            };
        }


        public static SongVoice GetDefaultTestSongVoice()
        {
            return new SongVoice()
            {
                VoiceName = "Main",
                IsMainVoice = true,
                VoiceNumber = 1,
            };
        }

        public static SongBar[] GetDefaultTestSongBars()
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
                }
            };
        }

        public static List<SongNote> GetDefaultTestSongNotes()
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
                },new SongNote()
                {
                    Position = 2,
                    Length = 2,
                    ChordName = "Gmaj7",
                    NoteValues = String.Join("|", SongNoteExtension.GetNoteValuesFromChordName("Gmaj7")),
                },
            };
        }
    }
}
