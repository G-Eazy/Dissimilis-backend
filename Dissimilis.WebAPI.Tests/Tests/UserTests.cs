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
        public async Task GetAllSysAdminsShouldReturn1()
        {
            var sysAdmin = await _mediator.Send(new QuerySysAdmins());
            sysAdmin.ShouldNotBeNull("Sysadmins were not fetched correctly");
            Assert.True(sysAdmin[0].Name == "SysAdminUser", "Sysadmin was not fetched correctly");
        }
    }
}
