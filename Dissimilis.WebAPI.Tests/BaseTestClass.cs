using Dissimilis.DbContext.Models;
using Dissimilis.DbContext.Models.Song;
using Dissimilis.WebAPI.Services;
using Dissimilis.WebAPI.xUnit.Setup;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Dissimilis.WebAPI.xUnit
{
    public class BaseTestClass : IClassFixture<TestServerFixture>
    {
        internal readonly TestServerFixture _testServerFixture;
        internal readonly IMediator _mediator;
        internal readonly IPermissionCheckerService _permissionChecker;

        internal User SysAdminUser;
        internal User NorwayAdminUser;
        internal User NorwayAdminUser2;
        internal User GuatemalaAdminUser;
        internal User SandvikaAdminUser;
        internal User SandvikaAdminUser2;
        internal User TrondheimAdminUser;
        internal User BergenAdminUser;
        internal User QuetzaltenangoAdminUser;
        internal User DeepPurpleFanUser;
        internal User EdvardGriegFanUser;
        internal User JustinBieberFanUser;
        internal User RammsteinFanUser;
        internal User U2FanUser;
        internal User NoSongsUser;
        internal User DeleteOrgUser;
        internal User DeleteGroupUser;
        internal User OralBeeFanUser;
        internal User RemoveFromOrgUser;
        internal User CheckSysAdminStatusUser;

        internal Song LisaGikkTilSkolenSong;
        internal Song SmokeOnTheWaterSong;
        internal Song SpeedKingSong;
        internal Song DuHastSong;
        internal Song BabySong;
        internal Song DovregubbensHallSong;
        internal Song BegyntePåBunnen;
        internal Song Baris;

        internal Organisation NorwayOrganisation;
        internal Organisation GuatemalaOrganisation;
        internal Organisation DeleteOrganisation;

        internal Group SandvikaGroup;
        internal Group TrondheimGroup;
        internal Group BergenGroup;
        internal Group QuetzaltenangoGroup;
        internal Group DeleteGroup;

        public BaseTestClass(TestServerFixture testServerFixture)
        {
            _testServerFixture = testServerFixture;

            _mediator = _testServerFixture.GetServiceProvider().GetService<IMediator>();
            _permissionChecker = _testServerFixture.GetServiceProvider().GetService<IPermissionCheckerService>();

            UpdateAllUsers();
            UpdateAllSongs();
            UpdateAllOrganisations();
            UpdateAllGroups();
        }

        internal void UpdateAllUsers()
        {
            var users = GetAllUsers();
            SysAdminUser = users.SingleOrDefault(user => user.Email == "SysAdmin@Norway.no");
            NorwayAdminUser = users.SingleOrDefault(user => user.Email == "OrgAdmin@Norway.no");
            NorwayAdminUser2 = users.SingleOrDefault(user => user.Email == "OrgAdmin2@Norway.no");
            GuatemalaAdminUser = users.SingleOrDefault(user => user.Email == "OrgAdmin@Guatemala.no");
            SandvikaAdminUser = users.SingleOrDefault(user => user.Email == "GroupAdmin@Sandvika_Norway.no");
            SandvikaAdminUser2 = users.SingleOrDefault(user => user.Email == "GroupAdmin2@Sandvika_Norway.no");
            TrondheimAdminUser = users.SingleOrDefault(user => user.Email == "GroupAdmin@Trondheim_Norway.no");
            BergenAdminUser = users.SingleOrDefault(user => user.Email == "GroupAdmin@Bergen_Norway.no");
            DeepPurpleFanUser = users.SingleOrDefault(user => user.Email == "Deep_Purple_fan@Trondheim_Norway.no");
            EdvardGriegFanUser = users.SingleOrDefault(user => user.Email == "Edvard_Grieg_fan@Sandvika_Norway.no");
            QuetzaltenangoAdminUser = users.SingleOrDefault(user => user.Email == "GroupAdmin@Quetzaltenango_Guatemala.no");
            JustinBieberFanUser = users.SingleOrDefault(user => user.Email == "Justin_Bieber_fan@Norway.no");
            RammsteinFanUser = users.SingleOrDefault(user => user.Email == "Rammstein_fan@Norway.no");
            U2FanUser = users.SingleOrDefault(user => user.Email == "U2_fan@Sandvika_Norway.no");
            NoSongsUser = users.SingleOrDefault(user => user.Email == "NoSongs@Norway.no");
            DeleteOrgUser = users.SingleOrDefault(user => user.Email == "DeleteOrgUser@Delete.no");
            DeleteGroupUser = users.SingleOrDefault(user => user.Email == "DeleteGroupUser@Delete_Delete.no");
            OralBeeFanUser = users.SingleOrDefault(user => user.Email == "Oral_Bee_fan@Quetzaltenango_Guatemala.no");
            RemoveFromOrgUser = users.SingleOrDefault(user => user.Email == "RemoveUser@Norway.no");
            CheckSysAdminStatusUser = users.SingleOrDefault(user => user.Email == "CheckSystemAdminStatus@Norway.no");
        }

        private List<User> GetAllUsers()
        {
            return _testServerFixture.GetContext().Users.ToList();
        }
        internal List<Song> UpdateAllSongs()
        {
            var songs = GetAllSongs();
            LisaGikkTilSkolenSong = songs.SingleOrDefault(song => song.Title == "Lisa gikk til skolen");
            SmokeOnTheWaterSong = songs.SingleOrDefault(song => song.Title == "Smoke on the water");
            SpeedKingSong = songs.SingleOrDefault(song => song.Title == "Speed King");
            DuHastSong = songs.SingleOrDefault(song => song.Title == "Du hast");
            BabySong = songs.SingleOrDefault(song => song.Title == "Baby");
            DovregubbensHallSong = songs.SingleOrDefault(song => song.Title == "Dovregubbens hall");
            BegyntePåBunnen = songs.SingleOrDefault(song => song.Title == "Begynte på bunnen");
            Baris = songs.SingleOrDefault(song => song.Title == "Baris");
            return songs;
        }

        private List<Song> GetAllSongs()
        {
            return _testServerFixture.GetContext().Songs
                .Include(song => song.Voices)
                .ThenInclude(voice => voice.SongBars)
                .ThenInclude(bar => bar.Notes)
                .ToList();
        }

        internal List<Organisation> UpdateAllOrganisations()
        {
            var organisations = GetAllOrganisations();
            NorwayOrganisation = organisations.SingleOrDefault(organisation => organisation.Name == "Norway");
            GuatemalaOrganisation = organisations.SingleOrDefault(organisation => organisation.Name == "Guatemala");
            DeleteOrganisation = organisations.SingleOrDefault(organisation => organisation.Name == "Delete");

            return organisations;
        }

        public List<Organisation> GetAllOrganisations()
        {
            return _testServerFixture.GetContext().Organisations
                .ToList();
        }

        internal List<Group> UpdateAllGroups()
        {
            var groups = GetAllGroups();
            SandvikaGroup = groups.SingleOrDefault(group => group.Name == "Sandvika_Norway");
            TrondheimGroup = groups.SingleOrDefault(group => group.Name == "Trondheim_Norway");
            BergenGroup = groups.SingleOrDefault(group => group.Name == "Bergen_Norway");
            DeleteGroup = groups.SingleOrDefault(group => group.Name == "Delete_Delete");
            QuetzaltenangoGroup = groups.SingleOrDefault(group => group.Name == "Quetzaltenango_Guatemala");

            return groups;
        }

        public List<Group> GetAllGroups()
        {
            return _testServerFixture.GetContext().Groups
                .ToList();
        }
        internal void CreateAndAddGroupTagIfNotExsisting(int songId, int groupId)
        {
            var SongGroupTag = _testServerFixture.GetContext().SongGroupTags.SingleOrDefault(s => s.GroupId == groupId && s.SongId == songId);
            if(SongGroupTag == null)
            {
            SongGroupTag = new SongGroupTag()
                {
                    SongId = songId,
                    GroupId = groupId
                };
                _testServerFixture.GetContext().SongGroupTags.Add(SongGroupTag);
                _testServerFixture.GetContext().SaveChanges();
            }

        }

        internal void CreateAndAddOrganisationTagIfNotExisting(int songId, int orgId)
        {
            var SongOrganisationTag = _testServerFixture.GetContext().SongOrganisationTags.SingleOrDefault(s => s.OrganisationId == orgId && s.SongId == songId);
            if (SongOrganisationTag == null)
            {
                SongOrganisationTag = new SongOrganisationTag()
                {
                    SongId = songId,
                    OrganisationId = orgId
                };
                _testServerFixture.GetContext().SongOrganisationTags.Add(SongOrganisationTag);
                _testServerFixture.GetContext().SaveChanges();
            }
        }

        internal void CreateAndAddSharedUserIfNotExisting(int songId, int userId)
        {
            var SharedSongUser = _testServerFixture.GetContext().SongSharedUser.SingleOrDefault(s => s.UserId== userId && s.SongId == songId);
            if (SharedSongUser == null)
            {
                SharedSongUser = new SongSharedUser()
                {
                    SongId = songId,
                    UserId = userId
                };
                _testServerFixture.GetContext().SongSharedUser.Add(SharedSongUser);
                _testServerFixture.GetContext().SaveChanges();
            }

        }
    }
}
