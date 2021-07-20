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
using Dissimilis.WebAPI.Controllers.Boorganisation.DtoModelsOut;
using Dissimilis.WebAPI.Controllers.BoGroup.DtoModelsIn;
using Dissimilis.WebAPI.Controllers.BoGroup.Commands;
using Dissimilis.WebAPI.Controllers.Bogroup.Query;
using Dissimilis.WebAPI.Controllers.Bousers.Query;
using Dissimilis.WebAPI.Controllers.MultiUseDtos.DtoModelsIn;

namespace Dissimilis.WebAPI.xUnit.Tests
{
    [Collection("Serial")]
    [CollectionDefinition("Serial", DisableParallelization = true)]
    public class GroupTests : BaseTestClass
    {
        private readonly IMediator _mediator;
        private UserDto AdminUser;
        private UserDto SuppUser1;
        private UserDto SuppUser2;

        public GroupTests(TestServerFixture testServerFixture) : base(testServerFixture)
        {
            _mediator = _testServerFixture.GetServiceProvider().GetService<IMediator>();
            var users = GetAllUsers().Result;
            AdminUser = users.SingleOrDefault(user => user.Email == "test@test.no");
            SuppUser1 = users.SingleOrDefault(user => user.Email == "supUser1@test.no");
            SuppUser2 = users.SingleOrDefault(user => user.Email == "supUser2@test.no");
        }
        private async Task<UserDto[]> GetAllUsers()
        {
            return await _mediator.Send(new QueryAll());
        }

        /// <summary>
        /// Helper method to create org for tests. Remember to have correct permissions.
        /// </summary>
        /// <param name="orgNumber"></param>
        /// <returns></returns>
        private async Task<OrganisationByIdDto> CreateOrganisation(int orgNumber, int adminId)
        {
            CreateOrganisationDto orgDto = new CreateOrganisationDto()
            {
                Name = $"TestOrg{orgNumber}",
                Address = $"TestAdress{orgNumber}",
                EmailAddress = $"TestOrg1@test.com{orgNumber}",
                Description = $"TestDesc1{orgNumber}",
                PhoneNumber = $"1234567{orgNumber}",
                FirstAdminId = adminId
            };
            var item = await _mediator.Send(new CreateOrganisationCommand(orgDto));
            var org = await _mediator.Send(new QueryOrganisationById(item.OrganisationId));
            return org;
        }

        private CreateGroupDto GetCreateGroupDto(int groupNumber, int orgId, int adminId)
        {
            return new CreateGroupDto()
            {
                Name = $"TestGroup{groupNumber}",
                OrganisationId = orgId,
                Address = $"TestAdress{groupNumber}",
                EmailAddress = $"TestOrg1@test.com{groupNumber}",
                Description = $"TestDesc1{groupNumber}",
                PhoneNumber = $"1234567{groupNumber}",
                FirstAdminId = adminId
            };
        }

        public UpdateGroupAndOrganisationDto GetUpdateGroupAndOrganisationDto()
        {
            return new UpdateGroupAndOrganisationDto()
            {
                Name = "test",
                Address = "address123",
                EmailAddress = "email@address.no",
                Description = "maybe",
                PhoneNumber = "12345678"
            };
        }

        [Fact]
        public async Task TestCreateGroupPermissions()
        {
            TestServerFixture.ChangeCurrentUserId(AdminUser.UserId);
            OrganisationByIdDto org = await CreateOrganisation(3, SuppUser1.UserId);

            // Should be allowed
            var item1 = await _mediator.Send(new CreateGroupCommand(GetCreateGroupDto(4, org.Id, SuppUser1.UserId)));
            var group1 = await _mediator.Send(new QueryGroupById(item1.GroupId));
            group1.Name.ShouldBeEquivalentTo("TestGroup4", "Group creation failed");

            // Should be allowed, since SuppUser1 is admin of org
            TestServerFixture.ChangeCurrentUserId(SuppUser1.UserId);
            var item2 = await _mediator.Send(new CreateGroupCommand(GetCreateGroupDto(5, org.Id, SuppUser1.UserId)));
            var group2 = await _mediator.Send(new QueryGroupById(item2.GroupId));
            group2.Name.ShouldBeEquivalentTo("TestGroup5", "Group creation failed");


            // Should throw exception, since SuppUser2 is neither sysadmin or orgadmin
            TestServerFixture.ChangeCurrentUserId(SuppUser2.UserId);
            var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await _mediator.Send(new CreateGroupCommand(GetCreateGroupDto(6, org.Id, SuppUser1.UserId))));
            exception.Message.ShouldBeEquivalentTo("User does not have permission to create group in organisation", "Correct exception was not thrown");
        }

        [Fact]
        public async Task TestGetAllUsersInGroup()
        {
            OrganisationByIdDto org = await CreateOrganisation(1, SuppUser1.UserId);
            var item1 = await _mediator.Send(new CreateGroupCommand(GetCreateGroupDto(4, org.Id, SuppUser1.UserId)));
            var users = await _mediator.Send(new QueryUsersInGroup(1));
            users.Length.ShouldBeGreaterThan(0, "Did not get all users");
        }

        [Fact]
        public async Task TestUpdateGroup()
        {
            OrganisationByIdDto org = await CreateOrganisation(10, SuppUser1.UserId);
            var item = await _mediator.Send(new CreateGroupCommand(GetCreateGroupDto(10, org.Id, SuppUser1.UserId)));
            var group = await _mediator.Send(new QueryGroupById(item.GroupId));

            var updateDto = GetUpdateGroupAndOrganisationDto();
            var updateItem = await _mediator.Send(new UpdateGroupCommand(group.GroupId, updateDto));
            var updatedGroup = await _mediator.Send(new QueryGroupById(updateItem.GroupId));

            updatedGroup.Name.ShouldBeEquivalentTo(updateDto.Name, "Name did not match");
            updatedGroup.Address.ShouldBeEquivalentTo(updateDto.Address, "Address did not match");
            updatedGroup.EmailAddress.ShouldBeEquivalentTo(updateDto.EmailAddress, "Email was not updated");
            updatedGroup.Description.ShouldBeEquivalentTo(updateDto.Description, "Description was not updated");
            updatedGroup.PhoneNumber.ShouldBeEquivalentTo(updateDto.PhoneNumber, "Phonenumber was not updated");
        }
    }
}
