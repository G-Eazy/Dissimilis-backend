using Dissimilis.WebAPI.xUnit.Setup;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Shouldly;
using Dissimilis.WebAPI.Controllers.BoGroup.DtoModelsIn;
using Dissimilis.WebAPI.Controllers.BoGroup.Commands;
using Dissimilis.WebAPI.Controllers.Bogroup.Query;
using Dissimilis.DbContext.Models.Enums;
using static Dissimilis.WebAPI.xUnit.Extensions;
using Dissimilis.WebAPI.Controllers.BoGroup.Query;
using Dissimilis.WebAPI.Controllers.BoUser.Queries;
using Dissimilis.WebAPI.Controllers.BoOrganisation.Commands;
using Dissimilis.WebAPI.Controllers.BoUser.DtoModelsIn;
using Dissimilis.WebAPI.Controllers.BoUser.Commands;

namespace Dissimilis.WebAPI.xUnit.Tests
{
    [Collection("Serial")]
    [CollectionDefinition("Serial", DisableParallelization = true)]
    public class UserTests : BaseTestClass
    {
        public UserTests(TestServerFixture testServerFixture) : base(testServerFixture)
        {
        }

        [Fact]
        public async Task GetAllSysAdminsShouldNotBeNull()
        {
            var sysAdmins = await _mediator.Send(new QuerySysAdmins());
            sysAdmins.ShouldNotBeNull("Sysadmins were not fetched correctly");
            //Assert.True(sysAdmins.Length == 2, $"Not all sysAdmins were fetched. Only {sysAdmins.Length} were fetched...");
        }

        [Fact]
        public async Task UpdateSysAdminStatusShouldSucceed()
        {
            TestServerFixture.ChangeCurrentUserId(SysAdminUser.Id);
            CheckSysAdminStatusUser.IsSystemAdmin.ShouldBeFalse("User was already sysadmin");

            await _mediator.Send(new UpdateSysAdminStatusCommand(CheckSysAdminStatusUser.Id, new UpdateSysAdminStatusDto() { IsSystemAdmin = true }));
            CheckSysAdminStatusUser.IsSystemAdmin.ShouldBeTrue("User sysAdmin status did not change");
        }

        [Fact]
        public async Task DeleteUserShouldSucceed()
        {
            TestServerFixture.ChangeCurrentUserId(SysAdminUser.Id);
            var deleteUser = _testServerFixture.GetContext().Users.SingleOrDefault(u => u.Email == "RemoveUser@Norway.no");
            deleteUser.ShouldNotBeNull("User was null");

            await _mediator.Send(new DeleteUserCommand(deleteUser.Id));
            deleteUser = _testServerFixture.GetContext().Users.SingleOrDefault(u => u.Email == "RemoveUser@Norway.no");
            deleteUser.ShouldBeNull("User was not deleted");
        }
    }
}
