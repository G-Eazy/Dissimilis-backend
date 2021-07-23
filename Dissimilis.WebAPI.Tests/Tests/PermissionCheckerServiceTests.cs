using Dissimilis.WebAPI.xUnit.Setup;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Dissimilis.DbContext.Models;
using Dissimilis.DbContext.Models.Enums;
using System.Threading;

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


        [Fact]
        public async Task GetSysAdminStatusSysAdminShouldBeTrue()
        {
            var adminDto = await _permissionChecker.CheckUserAdminStatus(SysAdminUser, c);
            Assert.True(adminDto.SystemAdmin);
        }

        [Fact]
        public async Task GetSysAdminStatusNotSysAdminShouldBeFalse()
        {
            var adminDto = await _permissionChecker.CheckUserAdminStatus(NorwayAdminUser, c);
            Assert.False(adminDto.SystemAdmin);
        }

        [Fact]
        public async Task GetOrgAdminStatusOrgAdminShouldBeTrue()
        {
            var adminDto = await _permissionChecker.CheckUserAdminStatus(NorwayAdminUser, c);
            Assert.True(adminDto.OrganisationAdmin);
        }

        [Fact]
        public async Task GetOrgAdminStatusNotOrgAdminShouldBeFalse()
        {
            var adminDto = await _permissionChecker.CheckUserAdminStatus(TrondheimAdminUser, c);
            Assert.False(adminDto.OrganisationAdmin);
        }

        [Fact]
        public async Task GetGroupAdminStatusGroupAdminShouldBeTrue()
        {
            var adminDto = await _permissionChecker.CheckUserAdminStatus(TrondheimAdminUser, c);
            Assert.True(adminDto.GroupAdmin);
        }

        [Fact]
        public async Task GetGroupAdminStatusNotGroupAdminShouldBeFalse()
        {
            var adminDto = await _permissionChecker.CheckUserAdminStatus(DeepPurpleFanUser, c);
            Assert.False(adminDto.GroupAdmin);
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
        public async Task GetOrgAsGroupAdminShouldReturnFalse()
        {
            var allowed = await _permissionChecker.CheckPermission(NorwayOrganisation, TrondheimAdminUser, Operation.Get, c);
            Assert.False(allowed);
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
    }
}

