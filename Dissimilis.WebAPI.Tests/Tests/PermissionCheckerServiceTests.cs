﻿using Dissimilis.WebAPI.xUnit.Setup;
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

        // TODO: Implement tests for groups as well!!!

        /*[Fact]
        public async Task CreateGroupAsSysAdminShouldReturnTrue()
        {
            Group group = new Group("testGroup69", NorwayOrganisation.Id, NorwayAdminUser.Id);
            var allowed = await _permissionChecker.CheckPermission(org, SysAdminUser, Operation.Create, c);
            Assert.True(allowed);
        }

        [Fact]
        public async Task CreateOrgAsOrgAdminShouldReturnFalse()
        {
            Group org = new Group("testOrg69", NorwayAdminUser.Id);
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
        }*/

        // Delete

        [Fact]
        public async Task DeleteGroupAsSysAdminShouldReturnTrue()
        {
            var allowed = await _permissionChecker.CheckPermission(TrondheimGroup, SysAdminUser, Operation.Delete, c);
            Assert.True(allowed);
        }

        [Fact]
        public async Task DeleteGroupAsOrgAdminShouldReturnTrue()
        {
            var allowed = await _permissionChecker.CheckPermission(TrondheimGroup, NorwayAdminUser, Operation.Delete, c);
            Assert.True(allowed);
        }

        [Fact]
        public async Task DeleteGroupAsGroupAdminShouldReturnFalse()
        {
            var allowed = await _permissionChecker.CheckPermission(TrondheimGroup, TrondheimAdminUser, Operation.Delete, c);
            Assert.False(allowed);
        }

        [Fact]
        public async Task DeleteGroupAsGroupMemberShouldReturnFalse()
        {
            var allowed = await _permissionChecker.CheckPermission(TrondheimGroup, DeepPurpleFanUser, Operation.Delete, c);
            Assert.False(allowed);
        }

        // Get

        /*[Fact]
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
        }*/
    }
}

