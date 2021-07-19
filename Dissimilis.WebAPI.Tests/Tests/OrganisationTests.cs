using Dissimilis.WebAPI.xUnit.Setup;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using static Dissimilis.WebAPI.xUnit.Extensions;
using Dissimilis.WebAPI.Controllers.BoUser.DtoModelsOut;
using Dissimilis.WebAPI.Controllers.BoUser.Queries;

namespace Dissimilis.WebAPI.xUnit.Tests
{
    [Collection("Serial")]
    [CollectionDefinition("Serial", DisableParallelization = true)]
    public class OrganisationTests : BaseTestClass
    {
        private readonly IMediator _mediator;
        private UserDto AdminUser;
        private UserDto SuppUser1;
        private UserDto SuppUser2;
        private UserDto SuppUser3;

        public OrganisationTests(TestServerFixture testServerFixture) : base(testServerFixture)
        {
            _mediator = _testServerFixture.GetServiceProvider().GetService<IMediator>();
            var users = GetAllUsers().Result;
            AdminUser = users.SingleOrDefault(user => user.Email == "test@test.no");
            SuppUser1 = users.SingleOrDefault(user => user.Email == "supUser1@test.no");
            SuppUser2 = users.SingleOrDefault(user => user.Email == "supUser2@test.no");
            SuppUser3 = users.SingleOrDefault(user => user.Email == "supUser3@test.no");
        }

        private async Task<UserDto[]> GetAllUsers()
        {
            return await _mediator.Send(new QueryAll());
        }

        [Fact]
        public async Task TestCreateOrganisationCommand()
        {

        }
    }
}
