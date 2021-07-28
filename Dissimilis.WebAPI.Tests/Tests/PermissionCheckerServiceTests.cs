using Dissimilis.WebAPI.xUnit.Setup;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Dissimilis.DbContext.Models;
using Dissimilis.DbContext.Models.Enums;
using System.Threading;
using Dissimilis.DbContext.Models.Song;

namespace Dissimilis.WebAPI.xUnit.Tests
{
    [Collection("Serial")]
    [CollectionDefinition("Serial", DisableParallelization = true)]
    public class PermissionCheckerServiceTests : BaseTestClass
    {
        CancellationToken c = new CancellationToken();
        public PermissionCheckerServiceTests(TestServerFixture testServerFixture) : base(testServerFixture)
        {
        }

        /*
         * ***********************************************
         * 
         * 
         *         ORGANISATION PERMISSION TESTS
         * 
         * 
         * ***********************************************
         */ 


        // Create

        [Fact]
        public async Task CreateOrgAsSysAdminShouldReturnTrue()
        {
            Organisation org = new Organisation("testOrg69", NorwayAdminUser.Id);
            var allowed = await _permissionChecker.CheckPermission(org, SysAdminUser, Operation.Create, c);
            Assert.True(allowed);
        }

        [Fact]
        public async Task CreateOrgAsOrgAdminShouldReturnFalse()
        {
            Organisation org = new Organisation("testOrg69", NorwayAdminUser.Id);
            var allowed = await _permissionChecker.CheckPermission(org, NorwayAdminUser, Operation.Create, c);
            Assert.False(allowed);
        }

        [Fact]
        public async Task CreateOrgAsGroupAdminShouldReturnFalse()
        {
            Organisation org = new Organisation("testOrg69", NorwayAdminUser.Id);
            var allowed = await _permissionChecker.CheckPermission(org, TrondheimAdminUser, Operation.Create, c);
            Assert.False(allowed);
        }

        [Fact]
        public async Task CreateOrgAsOrgMemberShouldReturnFalse()
        {
            Organisation org = new Organisation("testOrg69", NorwayAdminUser.Id);
            var allowed = await _permissionChecker.CheckPermission(org, DeepPurpleFanUser, Operation.Create, c);
            Assert.False(allowed);
        }

        // Update

        [Fact]
        public async Task UpdateOrgAsSysAdminShouldReturnTrue()
        {
            var allowed = await _permissionChecker.CheckPermission(NorwayOrganisation, SysAdminUser, Operation.Modify, c);
            Assert.True(allowed);
        }

        [Fact]
        public async Task UpdateOrgAsSameOrgAdminShouldReturnTrue()
        {
            var allowed = await _permissionChecker.CheckPermission(NorwayOrganisation, NorwayAdminUser, Operation.Modify, c);
            Assert.True(allowed);
        }

        [Fact]
        public async Task UpdateOrgAsDifferentOrgAdminShouldReturnFalse()
        {
            var allowed = await _permissionChecker.CheckPermission(NorwayOrganisation, GuatemalaAdminUser, Operation.Modify, c);
            Assert.False(allowed);
        }

        [Fact]
        public async Task UpdateOrgAsGroupAdminShouldReturnFalse()
        {
            var allowed = await _permissionChecker.CheckPermission(NorwayOrganisation, TrondheimAdminUser, Operation.Modify, c);
            Assert.False(allowed, $"TrondheimAdminUser is role: {BergenAdminUser.Organisations.ElementAt(0).Role}"); ;
        }

        [Fact]
        public async Task UpdateOrgAsOrgMemberShouldReturnFalse()
        {
            var allowed = await _permissionChecker.CheckPermission(NorwayOrganisation, DeepPurpleFanUser, Operation.Modify, c);
            Assert.False(allowed);
        }

        // Delete

        [Fact]
        public async Task DeleteOrgAsSysAdminShouldReturnTrue()
        {
            var allowed = await _permissionChecker.CheckPermission(NorwayOrganisation, SysAdminUser, Operation.Delete, c);
            Assert.True(allowed);
        }

        [Fact]
        public async Task DeleteOrgAsOrgAdminShouldReturnFalse()
        {
            var allowed = await _permissionChecker.CheckPermission(NorwayOrganisation, NorwayAdminUser, Operation.Delete, c);
            Assert.False(allowed);
        }

        [Fact]
        public async Task DeleteOrgAsGroupAdminShouldReturnFalse()
        {
            var allowed = await _permissionChecker.CheckPermission(NorwayOrganisation, TrondheimAdminUser, Operation.Delete, c);
            Assert.False(allowed);
        }

        [Fact]
        public async Task DeleteOrgAsOrgMemberShouldReturnFalse()
        {
            var allowed = await _permissionChecker.CheckPermission(NorwayOrganisation, DeepPurpleFanUser, Operation.Delete, c);
            Assert.False(allowed);
        }

        // Get

        [Fact]
        public async Task GetOrgAsSysAdminShouldReturnTrue()
        {
            var allowed = await _permissionChecker.CheckPermission(NorwayOrganisation, SysAdminUser, Operation.Get, c);
            Assert.True(allowed);
        }

        [Fact]
        public async Task GetOrgAsOrgAdminShouldReturnTrue()
        {
            var allowed = await _permissionChecker.CheckPermission(NorwayOrganisation, NorwayAdminUser, Operation.Get, c);
            Assert.True(allowed);
        }

        [Fact]
        public async Task GetOrgAsGroupAdminShouldReturnTrue()
        {
            var allowed = await _permissionChecker.CheckPermission(NorwayOrganisation, TrondheimAdminUser, Operation.Get, c);
            Assert.True(allowed);
        }

        [Fact]
        public async Task GetOrgAsOrgMemberShouldReturnFalse()
        {
            var allowed = await _permissionChecker.CheckPermission(NorwayOrganisation, DeepPurpleFanUser, Operation.Get, c);
            Assert.False(allowed);
        }

        /*
         * ***********************************************
         * 
         * 
         *             GROUP PERMISSION TESTS
         * 
         * 
         * ***********************************************
         */




         /*
         * ***********************************************
         * 
         * 
         *             SONG PERMISSION TESTS
         * 
         * 
         * ***********************************************
         */
         [Fact]
         public async Task GetPublicSongAsSongownerReturnTrue()
        {
            var allowed = await _permissionChecker.CheckPermission(SpeedKingSong, DeepPurpleFanUser, Operation.Get, c);
            Assert.True(allowed);
        }
        [Fact]
        public async Task GetPrivateSongsAsSongownerReturnTrue()
        {
            var allowed = await _permissionChecker.CheckPermission(BabySong, JustinBieberFanUser, Operation.Get, c);
            Assert.True(allowed);
        }
        [Fact]
        public async Task GetPrivateSongAsSongModifierReturnTrue()
        {
            CreateAndAddSharedUserIfNotExisting(BabySong.Id, DeepPurpleFanUser.Id);
            var allowed = await _permissionChecker.CheckPermission(SpeedKingSong, DeepPurpleFanUser, Operation.Get, c);
            Assert.True(allowed);
        }
        [Fact]
        public async Task GetPublicSongAsNormalUserReturnTrue()
        {
            var allowed = await _permissionChecker.CheckPermission(SpeedKingSong, NoSongsUser, Operation.Get, c);
            Assert.True(allowed);
        }
        [Fact]
        public async Task GetDeletedPublicSongsAsNoSongUserReturnFalse()
        {
            var deletedSong = new Song()
            {
                ArrangerId = SysAdminUser.Id,
                CreatedById = SysAdminUser.Id,
                ProtectionLevel = ProtectionLevels.Public,
                Deleted = System.DateTimeOffset.Now,
            };
            var allowed = await _permissionChecker.CheckPermission(deletedSong, NoSongsUser, Operation.Get, c);
            Assert.False(allowed);
        }
        [Fact]
        public async Task GetPrivateSongAsSysadminReturnFalse()
        {
            var allowed = await _permissionChecker.CheckPermission(BabySong, SysAdminUser, Operation.Get, c);
            Assert.False(allowed);
        }
        [Fact]
        public async Task ModifySongAsSongownerReturnTrue()
        {
            var allowed = await _permissionChecker.CheckPermission(SpeedKingSong, DeepPurpleFanUser, Operation.Modify, c);
            Assert.True(allowed);
        }
        [Fact]
        public async Task ModifyPublicSongAsNormalUserReturnFalse()
        {
            var allowed = await _permissionChecker.CheckPermission(SpeedKingSong, NoSongsUser, Operation.Modify, c);
            Assert.False(allowed);
        }
        [Fact]
        public async Task ModifySongAsModifierReturnTrue()
        {
            CreateAndAddSharedUserIfNotExisting(SpeedKingSong.Id, JustinBieberFanUser.Id);
            var allowed = await _permissionChecker.CheckPermission(SpeedKingSong, JustinBieberFanUser, Operation.Modify, c);
            Assert.True(allowed);
        }
        [Fact]
        public async Task GDeleteSongAsSongownerReturnTrue()
        {
            var allowed = await _permissionChecker.CheckPermission(SpeedKingSong, DeepPurpleFanUser, Operation.Delete, c);
            Assert.True(allowed);
        }
        [Fact]
        public async Task DeleteSongAsModifierReturnFalse()
        {
            CreateAndAddSharedUserIfNotExisting(SpeedKingSong.Id, JustinBieberFanUser.Id);
            var allowed = await _permissionChecker.CheckPermission(SpeedKingSong, JustinBieberFanUser, Operation.Delete, c);
            Assert.False(allowed);
        }
        [Fact]
        public async Task RestoreSongAsModifierReturnFalse()
        {
            CreateAndAddSharedUserIfNotExisting(SpeedKingSong.Id, JustinBieberFanUser.Id);
            var allowed = await _permissionChecker.CheckPermission(SpeedKingSong, JustinBieberFanUser, Operation.Restore, c);
            Assert.False(allowed);
        }
        [Fact]
        public async Task RestorePublicSongAsSysadminReturnTrue()
        {
            var allowed = await _permissionChecker.CheckPermission(SpeedKingSong, SysAdminUser, Operation.Restore, c);
            Assert.True(allowed);
        }
        [Fact]
        public async Task RestoreSongAsOwnerReturnTrue()
        {
            var allowed = await _permissionChecker.CheckPermission(BabySong, JustinBieberFanUser, Operation.Restore, c);
            Assert.True(allowed);
        }
        [Fact]
        public async Task RestorePrivateSongAsSysadminReturnFalse()
        {
            var allowed = await _permissionChecker.CheckPermission(BabySong, SysAdminUser, Operation.Restore, c);
            Assert.False(allowed);
        }
        [Fact]
        public async Task DeletePrivateSongAsSysadminReturnFalse()
        {
            var allowed = await _permissionChecker.CheckPermission(BabySong, SysAdminUser, Operation.Delete, c);
            Assert.False(allowed);
        }
        [Fact]
        public async Task CreateSongReturnTrue()
        {
            var allowed = await _permissionChecker.CheckPermission(new Song(), NoSongsUser, Operation.Create, c);
            Assert.True(allowed);
        }
        [Fact]
        public async Task InviteSongAsOwnerReturnTrue()
        {
            var allowed = await _permissionChecker.CheckPermission(BabySong, JustinBieberFanUser, Operation.Invite, c);
            Assert.True(allowed);
        }
        [Fact]
        public async Task KickSongAsOwnerReturnTrue()
        {
            var allowed = await _permissionChecker.CheckPermission(BabySong, JustinBieberFanUser, Operation.Kick, c);
            Assert.True(allowed);
        }
        [Fact]
        public async Task InviteSongAsModifierReturnTrue()
        {
            CreateAndAddSharedUserIfNotExisting(SpeedKingSong.Id, JustinBieberFanUser.Id);
            var allowed = await _permissionChecker.CheckPermission(SpeedKingSong, JustinBieberFanUser, Operation.Invite, c);
            Assert.True(allowed);
        }
        [Fact]
        public async Task KickSongAsModifierReturnTrue()
        {
            CreateAndAddSharedUserIfNotExisting(SpeedKingSong.Id, JustinBieberFanUser.Id);
            var allowed = await _permissionChecker.CheckPermission(SpeedKingSong, JustinBieberFanUser, Operation.Kick, c);
            Assert.True(allowed);
        }
        [Fact]
        public async Task InviteSongAsNoSongUserReturnFalse()
        {
            var allowed = await _permissionChecker.CheckPermission(SpeedKingSong, NoSongsUser, Operation.Invite, c);
            Assert.False(allowed);
        }
        [Fact]
        public async Task KickSongAsNoSongUserReturnFalse()
        {
            var allowed = await _permissionChecker.CheckPermission(SpeedKingSong, NoSongsUser, Operation.Kick, c);
            Assert.False(allowed);
        }
        [Fact]
        public async Task InvitePrivateSongAsSysAdminReturnFalse()
        {
            var allowed = await _permissionChecker.CheckPermission(BabySong, SysAdminUser, Operation.Invite, c);
            Assert.False(allowed);
        }
        [Fact]
        public async Task KickPrivateSongAsSysAdminReturnFalse()
        {
            var allowed = await _permissionChecker.CheckPermission(BabySong, SysAdminUser, Operation.Kick, c);
            Assert.False(allowed);
        }
        [Fact]
        public async Task InvitePublicSongAsSysAdminReturnTrue()
        {
            var allowed = await _permissionChecker.CheckPermission(SpeedKingSong, SysAdminUser, Operation.Invite, c);
            Assert.True(allowed);
        }
        [Fact]
        public async Task KickPublicSongAsSysAdminReturnTrue()
        {
            var allowed = await _permissionChecker.CheckPermission(SpeedKingSong, SysAdminUser, Operation.Kick, c);
            Assert.True(allowed);
        }
    }
}

