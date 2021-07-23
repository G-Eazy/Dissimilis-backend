using Dissimilis.DbContext;
using Dissimilis.DbContext.Models;
using Dissimilis.DbContext.Models.Enums;
using Dissimilis.DbContext.Models.Song;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dissimilis.WebAPI.xUnit.Setup
{
    public class DbInitializer
    {
        private static readonly object _lock = new();
        private static DissimilisDbContext _dbContext;

        public static void Initialize(IServiceProvider serviceProvider)
        {
            lock (_lock)
            {
                _dbContext = (DissimilisDbContext)serviceProvider.GetService(typeof(DissimilisDbContext));
                _dbContext.Database.EnsureDeleted();
                _dbContext.Database.EnsureCreated();

                //Important that these commands run in this order, don't change the order because of dependencies
                SeedTestUsers();

                _dbContext.SaveChanges();

                SeedTestOrganisations();

                _dbContext.SaveChanges();

                SeedTestGroups();

                _dbContext.SaveChanges();

                SeedTestSongs();

                _dbContext.SaveChanges();
            }
        }

        private static void SeedTestUser(User userToAdd)
        {
            var user = _dbContext.Users.FirstOrDefault(dbUser => dbUser.Email == userToAdd.Email);

            if (user == null)
            {
                user = userToAdd;
                _dbContext.Users.Add(user);
                _dbContext.SaveChanges();
            }
        }

        private static void SeedTestUsers()
        {
            var users = TestServerFixture.GetTestUsers();
            foreach (var userToAdd in users)
            {
                SeedTestUser(userToAdd);
            }
        }

        private static void SeedTestOrganisations()
        {
            var organisations = TestServerFixture.GetTestOrganisations();
            foreach (var organisation in organisations)
            {
                SeedTestOrganisation(organisation);
            }
        }
        private static void SeedTestOrganisation(Organisation organisationToAdd)
        {
            var organisation = _dbContext.Organisations.SingleOrDefault(dbOrg => dbOrg.Name == organisationToAdd.Name);
            if (organisation == null)
            {
                organisation = organisationToAdd;
                _dbContext.Organisations.Add(organisation);
                _dbContext.SaveChanges();

                string orgEmailDomain = organisation.Name + ".no";
                var usersToAdd = _dbContext.Users.Where(dbUser => dbUser.Email.Contains(orgEmailDomain));
                foreach (var user in usersToAdd)
                {
                    var role = user.Email.Split("@")[0] == "OrgAdmin" ? Role.Admin : Role.Member;
                    AddMemberToOrganisation(organisation, user, role);
                }
            }
        }

        private static void AddMemberToOrganisation(Organisation organisation, User userToAdd, Role role)
        {
            var orgUser = new OrganisationUser()
            {
                Role = role,
                UserId = userToAdd.Id,
                OrganisationId = organisation.Id,
            };
            _dbContext.OrganisationUsers.Add(orgUser);
            _dbContext.SaveChanges();
        }

        private static void SeedTestGroups()
        {
            var groups = TestServerFixture.GetTestGroups();
            foreach (var group in groups)
            {
                SeedTestGroup(group);
            }
        }

        private static void SeedTestGroup(Group groupToAdd)
        {
            var group = _dbContext.Groups.SingleOrDefault(dbGroup => dbGroup.Name == groupToAdd.Name);
            if (group == null)
            {
                group = groupToAdd;

                string orgName = group.Name.Split("_")[1];
                var organisation = _dbContext.Organisations.SingleOrDefault(dbOrg => dbOrg.Name == orgName);
                group.OrganisationId = organisation.Id;

                _dbContext.Groups.Add(group);
                _dbContext.SaveChanges();

                string groupEmailDomain = group.Name + ".no";
                var usersToAdd = _dbContext.Users.Where(dbUser => dbUser.Email.Contains(groupEmailDomain));
                foreach (var user in usersToAdd)
                {
                    var role = user.Email.Split("@")[0] == "GroupAdmin" ? Role.Admin : Role.Member;
                    AddMemberToGroup(group, user, role);
                }
            }
        }

        private static void AddMemberToGroup(Group group, User userToAdd, Role role)
        {
            var groupUser = new GroupUser()
            {
                Role = role,
                UserId = userToAdd.Id,
                GroupId = group.Id,
            };
            group.Users.Add(groupUser);
            userToAdd.Groups.Add(groupUser);

            _dbContext.GroupUsers.Add(groupUser);
            _dbContext.SaveChanges();
        }

        private static void SeedTestSongs()
        {
            var songs = TestServerFixture.GetTestSongs();
            var users = _dbContext.Users.ToList();
            foreach (var song in songs)
            {
                var arrangerEmailName = song?.Composer ?? "SysAdmin";
                var arranger = users.SingleOrDefault(user =>
                    user.Email.Contains(arrangerEmailName));
                SeedTestSong(song, arranger);
            }
        }

        private static void SeedTestSong(Song song, User arranger)
        {
            song.ArrangerId = arranger.Id;
            song.CreatedById = arranger.Id;

            _dbContext.Songs.Add(song);
            _dbContext.SaveChanges();

            var voices = TestServerFixture.GetTestSongVoices();
            foreach (var voice in voices)
            {
                SeedTestVoice(voice, song);
            }
        }

        private static void SeedTestVoice(SongVoice voice, Song song)
        {
            voice.Song = song;
            voice.SongId = song.Id;

            _dbContext.SongVoices.Add(voice);
            _dbContext.SaveChanges();

            var bars = TestServerFixture.GetTestSongBars();
            foreach (var bar in bars)
            {
                SeedTestBar(bar, voice, bar.Position % 2 == 0);
            }
        }

        private static void SeedTestBar(SongBar bar, SongVoice voice, bool includeNotes)
        {
            bar.SongVoice = voice;
            bar.SongVoiceId = voice.Id;

            _dbContext.SongBars.Add(bar);
            _dbContext.SaveChanges();

            if (includeNotes)
            {
                var notes = TestServerFixture.GetTestSongNotes();
                foreach (var note in notes)
                {
                    SeedTestNote(note, bar);
                }
            }
        }

        private static void SeedTestNote(SongNote note, SongBar bar)
        {
            note.SongBar = bar;
            note.BarId = bar.Id;

            _dbContext.SongNotes.Add(note);
            _dbContext.SaveChanges();
        }

    }
}
