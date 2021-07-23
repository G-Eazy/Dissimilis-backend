using Dissimilis.WebAPI.xUnit.Setup;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using Dissimilis.WebAPI.Controllers.BoGroup.Query;

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
        private UserDto SuppUser3;

        public GroupTests(TestServerFixture testServerFixture) : base(testServerFixture)
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
                FirstAdminId = adminId
            };
        }

        public UpdateGroupAndOrganisationDto GetUpdateGroupAndOrganisationDto()
        {
            return new UpdateGroupAndOrganisationDto()
            {
                Name = "test4321",
                Address = "address123",
                Email = "email@address.no",
                Description = "maybe",
                PhoneNumber = "12345678"
            };
        }

        [Fact]
        public async Task CreateGroupShouldSucceed()
        {
            TestServerFixture.ChangeCurrentUserId(SysAdminUser.Id);

            var item1 = await _mediator.Send(new CreateGroupCommand(GetCreateGroupDto(1, NorwayOrganisation.Id, TrondheimAdminUser.Id)));
            var group1 = await _mediator.Send(new QueryGroupById(item1.GroupId));
            group1.Name.ShouldBeEquivalentTo("TestGroup1", "Group creation failed");
        }

        [Fact]
        public async Task TestGetUsersInGroup()
        {
            TestServerFixture.ChangeCurrentUserId(SysAdminUser.Id);
            var users = await _mediator.Send(new QueryUsersInGroup(TrondheimGroup.Id));
            users.Length.ShouldBeGreaterThan(0, "Did not all users");
        }

        [Fact]
        public async Task UpdateGroupShouldSucceed()
        {
            TestServerFixture.ChangeCurrentUserId(SysAdminUser.Id);

            var updateDto = GetUpdateGroupAndOrganisationDto();
            var updateItem = await _mediator.Send(new UpdateGroupCommand(TrondheimGroup.Id, updateDto));
            var updatedGroup = await _mediator.Send(new QueryGroupById(updateItem.GroupId));

            updatedGroup.Name.ShouldBeEquivalentTo(updateDto.Name, "Name did not match");
            updatedGroup.Address.ShouldBeEquivalentTo(updateDto.Address, "Address did not match");
            updatedGroup.EmailAddress.ShouldBeEquivalentTo(updateDto.Email, "Email was not updated");
            updatedGroup.Description.ShouldBeEquivalentTo(updateDto.Description, "Description was not updated");
            updatedGroup.PhoneNumber.ShouldBeEquivalentTo(updateDto.PhoneNumber, "Phonenumber was not updated");
        }
    }
}
