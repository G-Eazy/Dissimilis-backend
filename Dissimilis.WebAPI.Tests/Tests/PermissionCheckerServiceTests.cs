using Dissimilis.WebAPI.xUnit.Setup;
using MediatR;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using Dissimilis.WebAPI.Controllers.BoUser.DtoModelsOut;
using Dissimilis.WebAPI.Controllers.BoUser.Queries;
using Dissimilis.WebAPI.Controllers.BoOrganisation.Commands;
using Dissimilis.WebAPI.Controllers.Boorganisation.Query;
using Shouldly;
using Dissimilis.WebAPI.Controllers.BoOrganisation.DtoModelsIn;
using Dissimilis.WebAPI.Controllers.Bousers.Query;
using Dissimilis.WebAPI.Controllers.MultiUseDtos.DtoModelsIn;
using Dissimilis.WebAPI.Services;
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
            var allowed = await _permissionChecker.CheckPermission(org, BergenAdminUser, Operation.Create, c);
            Assert.False(allowed);
        }

        [Fact]
        public async Task CreateOrgAsOrgMemberShouldReturnFalse()
        {
            Organisation org = new Organisation("testOrg69", NorwayAdminUser.Id);
            var allowed = await _permissionChecker.CheckPermission(org, DeepPurpleFanUser, Operation.Create, c);
            Assert.False(allowed);
        }

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

        [Fact]
        public async Task DeleteOrgAsSysAdminShouldReturnTrue()
        {
            var allowed = await _permissionChecker.CheckPermission(NorwayOrganisation, SysAdminUser, Operation.Delete, c);
            Assert.True(allowed);
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

