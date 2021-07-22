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

namespace Dissimilis.WebAPI.xUnit.Tests
{
    [Collection("Serial")]
    [CollectionDefinition("Serial", DisableParallelization = true)]
    public class OrganisationTests : BaseTestClass
    {
        private readonly IMediator _mediator;
        private UserDto AdminUser;
        private UserDto SuppUser1;

        public OrganisationTests(TestServerFixture testServerFixture) : base(testServerFixture)
        {
            _mediator = _testServerFixture.GetServiceProvider().GetService<IMediator>();
            var users = GetAllUsers().Result;
            AdminUser = users.SingleOrDefault(user => user.Email == "test@test.no");
            SuppUser1 = users.SingleOrDefault(user => user.Email == "supUser1@test.no");
        }

        private async Task<UserDto[]> GetAllUsers()
        {
            return await _mediator.Send(new QueryAll());
        }

        public UpdateGroupAndOrganisationDto GetUpdateGroupAndOrganisationDto()
        {
            return new UpdateGroupAndOrganisationDto()
            {
                Name = "test12345",
                Address = "address123",
                Email = "yeet",
                Description = "maybe",
                PhoneNumber = "12345678"
            };
        }

        [Fact]
        public async Task TestCreateOrganisationPermissionHandling()
        {

            CreateOrganisationDto orgDto = new CreateOrganisationDto()
            {
                Name = "TestOrg11234",
                FirstAdminId = AdminUser.UserId
            };

            // Creating org as sysadmin should be fine, but noone else should be able to
            TestServerFixture.ChangeCurrentUserId(AdminUser.UserId);

            var item = await _mediator.Send(new CreateOrganisationCommand(orgDto));
            var org = await _mediator.Send(new QueryOrganisationById(item.OrganisationId));
            org.Name.ShouldBeEquivalentTo("TestOrg11234", "Creation of organisation failed");

            //Change user and provoke exception
            TestServerFixture.ChangeCurrentUserId(SuppUser1.UserId);
            var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await _mediator.Send(new CreateOrganisationCommand(orgDto)));
            exception.Message.ShouldBeEquivalentTo("User does not have permission to create organisation", "Error did not match");
        }

        [Fact]
        public async Task TestCreateOrganisationCommand()
        {
            string name = "TestOrg2";

            TestServerFixture.ChangeCurrentUserId(AdminUser.UserId);

            var item = await _mediator.Send(new CreateOrganisationCommand(new CreateOrganisationDto()
            {
                Name = name,
                FirstAdminId = AdminUser.UserId
            }));
            var result = await _mediator.Send(new QueryOrganisationById(item.OrganisationId));

            result.Name.ShouldBeEquivalentTo(name, "Organisation creation failed");
            result.admins[0].UserId.ShouldBe(AdminUser.UserId, "Organisation creation failed");
        }

        [Fact]
        public async Task TestGetAllUsersInOrganisation()
        {
            CreateOrganisationDto orgDto = new CreateOrganisationDto()
            {
                Name = "TestOrg1",
                FirstAdminId = AdminUser.UserId
            };
            await _mediator.Send(new CreateOrganisationCommand(orgDto));
            var users = await _mediator.Send(new QueryUsersInOrganisation(1));
            users.Length.ShouldBeGreaterThan(0, "Did not get all users");
        }

        [Fact]
        public async Task TestUpdateOrganisation()
        {
            CreateOrganisationDto orgDto = new CreateOrganisationDto()
            {
                Name = "TestOrg4",
                FirstAdminId = AdminUser.UserId
            };
            var item = await _mediator.Send(new CreateOrganisationCommand(orgDto));
            var org = await _mediator.Send(new QueryOrganisationById(item.OrganisationId));

            var updateDto = GetUpdateGroupAndOrganisationDto();
            var updateItem = await _mediator.Send(new UpdateOrganisationCommand(org.Id, updateDto));
            var updatedOrg = await _mediator.Send(new QueryOrganisationById(updateItem.OrganisationId));

            updatedOrg.Name.ShouldBeEquivalentTo(updateDto.Name, "Name did not match");
            updatedOrg.Address.ShouldBeEquivalentTo(updateDto.Address, "Address did not match");
            updatedOrg.Email.ShouldBeEquivalentTo(updateDto.Email, "Email was not updated");
            updatedOrg.Description.ShouldBeEquivalentTo(updateDto.Description, "Description was not updated");
            updatedOrg.PhoneNumber.ShouldBeEquivalentTo(updateDto.PhoneNumber, "Phonenumber was not updated");
        }
    }
}
