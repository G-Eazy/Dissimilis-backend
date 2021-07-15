using System;
using System.Linq;
using Dissimilis.DbContext;

namespace Dissimilis.WebAPI.xUnit.Setup
{
    public class DbInitializer
    {
        private static DissimilisDbContext _dbContext;

        public static void Initialize(IServiceProvider serviceProvider)
        {
            _dbContext = (DissimilisDbContext)serviceProvider.GetService(typeof(DissimilisDbContext));
            _dbContext.Database.EnsureCreated();

            SeedTestUser();

            _dbContext.SaveChanges();

            SeedSupplementTestUsers();

            _dbContext.SaveChanges();

            SeedTestOrganisation();

            _dbContext.SaveChanges();

            SeedTestSong();

            _dbContext.SaveChanges();
        }

        private static void SeedTestUser()
        {
            var user = _dbContext.Users.FirstOrDefault(uid =>
                uid.Email == TestServerFixture.GetDefaultTestUser().Email);
            if (user == null)
            {
                user = TestServerFixture.GetDefaultTestUser();
                _dbContext.Users.Add(user);
                _dbContext.SaveChanges();

                TestServerFixture.CurrentUserId = user.Id;
            }
        }

        private static void SeedSupplementTestUsers()
        {
            foreach (var newSuppUser in TestServerFixture.GetSupplementedTestUsers())
            {
                var user = _dbContext.Users.FirstOrDefault(dbUser =>
                    dbUser.Email == newSuppUser.Email);
                if (user == null)
                {
                    user = newSuppUser;
                    _dbContext.Users.Add(user);
                    _dbContext.SaveChanges();
                }
            }
        }

        private static void SeedTestOrganisation()
        {
            var organisation = _dbContext.Organisations.FirstOrDefault(org =>
                org.Name == TestServerFixture.GetDefaultTestOrganisation().Name);
            if (organisation == null)
            {
                //Create new organisation and add to db.
                organisation = TestServerFixture.GetDefaultTestOrganisation();

                _dbContext.Organisations.Add(organisation);
                _dbContext.SaveChanges();

                //Fetch the currentuser to use as admin user in organisation.
                var adminUser = _dbContext.Users.SingleOrDefault(user =>
                    user.Id == TestServerFixture.CurrentUserId);

                //Create the organisation user with admin role, and add to db.
                var orgUser = TestServerFixture.GetDefaultTestOrganisationUser();
                orgUser.User = adminUser;
                orgUser.Organisation = organisation;
                orgUser.UserId = adminUser.Id;
                orgUser.OrganisationId = organisation.Id;

                adminUser.Organisations.Add(orgUser);
                organisation.Users.Add(orgUser);

                _dbContext.OrganisationUsers.Add(orgUser);
                _dbContext.SaveChanges();

            }
        }

        private static void SeedTestSong()
        {
            var song = TestServerFixture.GetDefaultTestSong();
            var songVoice = TestServerFixture.GetDefaultTestSongVoice();
            var songBars = TestServerFixture.GetDefaultTestSongBars();
            var songNotes = TestServerFixture.GetDefaultTestSongNotes();

            var songOwner = _dbContext.Users.SingleOrDefault(user =>
                user.Id == TestServerFixture.CurrentUserId);

            song.Arranger = songOwner;
            song.ArrangerId = songOwner.Id;

            _dbContext.Songs.Add(song);
            _dbContext.SaveChanges();

            songVoice.Song = song;
            songVoice.SongId = song.Id;

            _dbContext.SongVoices.Add(songVoice);
            _dbContext.SaveChanges();

            foreach (var songBar in songBars)
            {
                songBar.SongVoice = songVoice;
                songBar.SongVoiceId = songVoice.Id;

                _dbContext.SongBars.Add(songBar);
                _dbContext.SaveChanges();
            }

            foreach (var songNote in songNotes)
            {
                songNote.SongBar = songBars[0];
                songNote.BarId = songBars[0].Id;

                _dbContext.SongNotes.Add(songNote);
                _dbContext.SaveChanges();
            }

            TestServerFixture.TestSongId = song.Id;
        }

    }
}
