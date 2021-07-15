using Dissimilis.DbContext;
using Dissimilis.DbContext.Models.Enums;
using Dissimilis.DbContext.Models.Song;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dissimilis.WebAPI.xUnit.Setup
{
    public class DbInitializer
    {
        private static DissimilisDbContext _dbContext;

        public static void Initialize(IServiceProvider serviceProvider)
        {
            _dbContext = (DissimilisDbContext)serviceProvider.GetService(typeof(DissimilisDbContext));
            _dbContext.Database.EnsureCreated();
            //important that these commands run in this order, don't change the order because of dependencies
            SeedTestUser();

            _dbContext.SaveChanges();

            SeedSupplementTestUsers();

            _dbContext.SaveChanges();

            SeedTestOrganisation();

            _dbContext.SaveChanges();

            SeedTestSupplementOrganisation();
            
            _dbContext.SaveChanges();

            SeedTestGroups();

            _dbContext.SaveChanges();

            SeedTestSong();

            _dbContext.SaveChanges();

            SeedTestSupplementSongs();

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
            var organisation = _dbContext.Organisations.SingleOrDefault(org =>
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

                var otherUsers = _dbContext.Users.Where(user => user.Id != adminUser.Id).ToArray();

                //adding all the other users to the "main" organisation
                foreach (var user in otherUsers)
                {
                    var normalOrgUser = TestServerFixture.GetSupplementedTestOrganisationUsers().First();
                    normalOrgUser.User = user;
                    normalOrgUser.UserId = user.Id;
                    normalOrgUser.Organisation = organisation;
                    normalOrgUser.OrganisationId = organisation.Id;
                    user.Organisations.Add(normalOrgUser);
                    organisation.Users.Add(orgUser);
                }



            }
        }
        private static void SeedTestSupplementOrganisation()
        {
            var organisation2 = _dbContext.Organisations.SingleOrDefault(org => 
            org.Name == TestServerFixture.GetSupplementOrganisation().Name);
            if(organisation2 == null)
            {
                //Create another organisation
                organisation2 = TestServerFixture.GetSupplementOrganisation();

                _dbContext.Organisations.Add(organisation2);
                _dbContext.SaveChanges();

                //Fetch another user to use as admin user in organisation.
                var adminUser = _dbContext.Users.Where(user => user.Id != TestServerFixture.CurrentUserId).First();

                //Create the organisation user with admin role, and add to db.
                var orgUser = TestServerFixture.GetDefaultTestOrganisationUser();
                orgUser.User = adminUser;
                orgUser.Organisation = organisation2;
                orgUser.UserId = adminUser.Id;
                orgUser.OrganisationId = organisation2.Id;

                adminUser.Organisations.Add(orgUser);
                organisation2.Users.Add(orgUser);

                _dbContext.OrganisationUsers.Add(orgUser);
                _dbContext.SaveChanges();
            }

        }
        private static void SeedTestGroups()
        {

            var ContainGroups = _dbContext.Groups.FirstOrDefault();
            if (ContainGroups == null)
            {
                //Create three groups with adminUsers
                var groups = TestServerFixture.GetTestGroups();

                var org = _dbContext.Organisations.SingleOrDefault(org =>
                org.Name == TestServerFixture.GetDefaultTestOrganisation().Name);
                
                var org2 = _dbContext.Organisations.SingleOrDefault(org =>
                org.Name == TestServerFixture.GetSupplementOrganisation().Name);
                
                var adminUser = _dbContext.Users.SingleOrDefault(user =>
                    user.Id == TestServerFixture.CurrentUserId);
                
                var otherUsers = _dbContext.Users.Where(user => user.Id != adminUser.Id ).ToArray();

                for(int i= 0; i< groups.Count(); i++)
                {
                    if (i < 2)
                    {
                        //add the two first groups to main org
                        groups[i].Organisation = org;
                        groups[i].OrganisationId = org.Id;
                        _dbContext.Groups.Add(groups[i]);
                        org.Groups.Add(groups[i]);
                        _dbContext.SaveChanges();
                        if (i == 0)
                        {
                            //add the adminUser as admin in the first group
                            var groupUserCurrent = TestServerFixture.GetDefaultTestGroupUser();
                            groupUserCurrent.User = adminUser;
                            groupUserCurrent.UserId = adminUser.Id;
                            groupUserCurrent.Group = groups[i];
                            groupUserCurrent.GroupId = groups[i].Id;
                            adminUser.Groups.Add(groupUserCurrent);
                            groups[i].Users.Add(groupUserCurrent);
                            _dbContext.SaveChanges();
                            
                        }
                    }
                    else
                    {
                        //add one of the groups to org2 and the otherUser to org2
                        groups[i].Organisation = org2;
                        groups[i].OrganisationId = org2.Id;
                        _dbContext.Groups.Add(groups[i]);
                        org2.Groups.Add(groups[i]);
                        _dbContext.SaveChanges();
                        var OrgUser = TestServerFixture.GetSupplementedTestOrganisationUsers().First();
                        OrgUser.User = otherUsers[i-1];
                        OrgUser.UserId = otherUsers[i - 1].Id;
                        OrgUser.Organisation = org2;
                        OrgUser.OrganisationId = org2.Id;
                        otherUsers[i-1].Organisations.Add(OrgUser);
                        org2.Users.Add(OrgUser);
                        _dbContext.SaveChanges();
                    }
                    if (i != 0)
                    {
                        //add the other normal testusers as admins in the two last groups and to the org
                        var groupUser = TestServerFixture.GetDefaultTestGroupUser();
                        groupUser.User = otherUsers[i-1];
                        groupUser.UserId = otherUsers[i-1].Id;
                        groupUser.Group = groups[i];
                        groupUser.GroupId = groups[i].Id;
                        otherUsers[i-1].Groups.Add(groupUser);
                        groups[i].Users.Add(groupUser);
                    }
                    _dbContext.SaveChanges();
                }

                _dbContext.SaveChanges();

                //add currentUser as member to OtherGroup and org
                var normalUser = TestServerFixture.GetSupplementedTestGroupUsers().First();
                normalUser.User = adminUser;
                normalUser.UserId = adminUser.Id;
                normalUser.Group = groups[2];
                normalUser.GroupId = groups[2].Id;
                adminUser.Groups.Add(normalUser);
                groups[2].Users.Add(normalUser);
                _dbContext.SaveChanges();


                var normalOrgUser = TestServerFixture.GetSupplementedTestOrganisationUsers().First();
                normalOrgUser.User = adminUser;
                normalOrgUser.UserId = adminUser.Id;
                normalOrgUser.Organisation = org2;
                normalOrgUser.OrganisationId = org2.Id;
                adminUser.Organisations.Add(normalOrgUser);
                org2.Users.Add(normalOrgUser);
                _dbContext.SaveChanges();
                
            }
        }

        private static void SeedTestSupplementSongs()
        {
            var songsCreated = _dbContext.Songs.Count();
            if (songsCreated > 1)
            {
                var songOwners = _dbContext.Users.Where(user => user.Id != TestServerFixture.CurrentUserId).ToArray();
                var songs = TestServerFixture.GetSupplementTestSongs();
                var shareSongsOrg = TestServerFixture.GetDefaultSongSharedOrganisations();
                var shareSongsGroup = TestServerFixture.GetDefaultSongSharedGroups();
                var defOrg = _dbContext.Organisations.Where(org => 
                org.Name == TestServerFixture.GetDefaultTestOrganisation().Name).SingleOrDefault();
                var suppOrg = _dbContext.Organisations.Where(org => 
                org.Name != TestServerFixture.GetDefaultTestOrganisation().Name).SingleOrDefault();
                var groups = _dbContext.Groups.ToArray();
                //make 4 songs
                for (int i = 0; i < songs.Count; i++)
                {
                    var songVoice = TestServerFixture.GetDefaultTestSongVoice();
                    var songBars = TestServerFixture.GetDefaultTestSongBars();
                    var songNotes = TestServerFixture.GetDefaultTestSongNotes();

                    //its 4 songs and 3 users
                    songs[i].Arranger = songOwners[i % 3];
                    songs[i].ArrangerId = songOwners[i % 3].Id;
                    songs[i].CreatedBy = songOwners[i % 3];
                    songs[i].CreatedById = songOwners[i % 3].Id;

                    _dbContext.Songs.Add(songs[i]);
                    _dbContext.SaveChanges();

                    songVoice.Song = songs[i];
                    songVoice.SongId = songs[i].Id;

                    _dbContext.SongVoices.Add(songVoice);
                    AddBarAndNotes(songBars, songNotes, songVoice);
                }
                //add the 4 songs to orgs and groups and make one of them private
                //song 0 with songOwner 0 as owner
                songs[0].ProtectionLevel = ProtectionLevels.Private;
                //song 1 with songOwner 1 as owner
                //song 2 with songOwner 2 as owner
                //song 3 with songOwner 0 as owner(owns 2 songs)

                //songOwner 0 is the orgAdmin in org2
                //group 0 and group 1 is in default org
                //group 2 is in supplement org

                //add two songs to suppOrg and 2 songs to defOrg
                //add song 0 to group 2 and supporg
                shareSongsGroup[0].Group = groups[2];
                shareSongsGroup[0].GroupId = groups[2].Id;
                shareSongsGroup[0].Song = songs[0];
                shareSongsGroup[0].SongId = songs[0].Id;
                groups[2].SharedSongs.Add(shareSongsGroup[0]);
                songs[0].SharedGroups.Add(shareSongsGroup[0]);
                _dbContext.SaveChanges();

                shareSongsOrg[0].Organisation = suppOrg;
                shareSongsOrg[0].OrganisationId = suppOrg.Id;
                shareSongsOrg[0].Song = songs[0];
                shareSongsOrg[0].SongId = songs[0].Id;
                suppOrg.SharedSongs.Add(shareSongsOrg[0]);
                songs[0].SharedOrganisations.Add(shareSongsOrg[0]);
                _dbContext.SaveChanges();

                //add song 3 only to suppOrg
                shareSongsOrg[1].Organisation = suppOrg;
                shareSongsOrg[1].OrganisationId = suppOrg.Id;
                shareSongsOrg[1].Song = songs[3];
                shareSongsOrg[1].SongId = songs[3].Id;
                suppOrg.SharedSongs.Add(shareSongsOrg[1]);
                songs[3].SharedOrganisations.Add(shareSongsOrg[1]);
                _dbContext.SaveChanges();

                //add song 1 to group 0 and defOrg
                shareSongsGroup[1].Group = groups[0];
                shareSongsGroup[1].GroupId = groups[0].Id;
                shareSongsGroup[1].Song = songs[1];
                shareSongsGroup[1].SongId = songs[1].Id;
                groups[0].SharedSongs.Add(shareSongsGroup[1]);
                songs[1].SharedGroups.Add(shareSongsGroup[1]);
                _dbContext.SaveChanges();

                shareSongsOrg[2].Organisation = defOrg;
                shareSongsOrg[2].OrganisationId = defOrg.Id;
                shareSongsOrg[2].Song = songs[1];
                shareSongsOrg[2].SongId = songs[1].Id;
                defOrg.SharedSongs.Add(shareSongsOrg[2]);
                songs[1].SharedOrganisations.Add(shareSongsOrg[2]);
                _dbContext.SaveChanges();

                //add song 2 only to defOrg
                shareSongsOrg[3].Organisation = defOrg;
                shareSongsOrg[3].OrganisationId = defOrg.Id;
                shareSongsOrg[3].Song = songs[2];
                shareSongsOrg[3].SongId = songs[2].Id;
                defOrg.SharedSongs.Add(shareSongsOrg[3]);
                songs[2].SharedOrganisations.Add(shareSongsOrg[3]);
                _dbContext.SaveChanges();
            }
        }

        private static void AddBarAndNotes(SongBar[] songBars, List<SongNote> songNotes, SongVoice songVoice)
        {
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
        }

        private static void SeedTestSong()
        {
            var song = TestServerFixture.GetDefaultTestSong();
            var songVoice = TestServerFixture.GetDefaultTestSongVoice();
            var songBars = TestServerFixture.GetDefaultTestSongBars();
            var songNotes = TestServerFixture.GetDefaultTestSongNotes();
            var defOrg = _dbContext.Organisations.Where(org => 
            org.Name == TestServerFixture.GetDefaultTestOrganisation().Name)
                .SingleOrDefault();

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

            AddBarAndNotes(songBars, songNotes, songVoice);

            TestServerFixture.TestSongId = song.Id;

            var shareSongOrg = TestServerFixture.GetDefaultSongSharedOrganisations().First();
            shareSongOrg.Organisation = defOrg;
            shareSongOrg.OrganisationId = defOrg.Id;
            shareSongOrg.Song = song;
            shareSongOrg.SongId = song.Id;
            defOrg.SharedSongs.Add(shareSongOrg);
            song.SharedOrganisations.Add(shareSongOrg);
            _dbContext.SaveChanges();
        }

    }
}
