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
using Dissimilis.DbContext.Models.Enums;

namespace Dissimilis.WebAPI.xUnit.Tests
{
    [Collection("Serial")]
    [CollectionDefinition("Serial", DisableParallelization = true)]
    public class GroupTests : BaseTestClass
    {
        public GroupTests(TestServerFixture testServerFixture) : base(testServerFixture)
        {
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
        public async Task TestAddMemberToGroupWhenCurrentUserIsAdminShouldSucceed()
        {
            TestServerFixture.ChangeCurrentUserId(SandvikaAdminUser.Id);
            await _mediator.Send(new AddMemberCommand(SandvikaGroup.Id, new AddMemberDto() { NewMemberUserId = RammsteinFanUser.Id, NewMemberRole = Role.Member }));

            var groupUser = _testServerFixture.GetContext()
                .Users.SingleOrDefault(user => user.Id == RammsteinFanUser.Id)
                .Groups.SingleOrDefault(groupUser => groupUser.GroupId == SandvikaGroup.Id && groupUser.UserId == RammsteinFanUser.Id);
            groupUser.ShouldNotBe(null);
            groupUser.Role.ShouldBe(Role.Member);
        }

        [Fact]
        public async Task TestAddAdminToGroupWhenCurrentUserIsAdminShouldSucceed()
        {
            TestServerFixture.ChangeCurrentUserId(BergenAdminUser.Id);

            await _mediator.Send(new AddMemberCommand(BergenGroup.Id, new AddMemberDto() { NewMemberUserId = RammsteinFanUser.Id, NewMemberRole = Role.Admin }));

            var groupUser = _testServerFixture.GetContext()
                .Users.SingleOrDefault(user => user.Id == RammsteinFanUser.Id)
                .Groups.SingleOrDefault(groupUser =>
                    groupUser.GroupId == BergenGroup.Id && groupUser.UserId == RammsteinFanUser.Id);
            groupUser.ShouldNotBe(null);
            groupUser.Role.ShouldBe(Role.Admin);
        }

        [Fact]
        public async Task TestAddMemberToGroupWhenCurrentUserIsNotAdminShouldFail()
        {
            TestServerFixture.ChangeCurrentUserId(TrondheimAdminUser.Id);

            await Should.ThrowAsync<UnauthorizedAccessException>(async () =>
                await _mediator.Send(new AddMemberCommand(BergenGroup.Id, new AddMemberDto() { NewMemberUserId = DeepPurpleFanUser.Id, NewMemberRole = Role.Admin })));

            _testServerFixture.GetContext()
                .Users.SingleOrDefault(user => user.Id == DeepPurpleFanUser.Id)
                .Groups.Any(groupUser =>
                    groupUser.GroupId == BergenGroup.Id && groupUser.UserId == DeepPurpleFanUser.Id)
                .ShouldBeFalse();
        }

        [Fact]
        public async Task TestRemoveMemberFromGroupWhenCurrentUserIsAdminShouldSucceed()
        {
            TestServerFixture.ChangeCurrentUserId(SandvikaAdminUser.Id);

            await _mediator.Send(new RemoveMemberCommand(SandvikaGroup.Id, EdvardGriegFanUser.Id));

            _testServerFixture.GetContext()
                .Users.SingleOrDefault(user => user.Id == EdvardGriegFanUser.Id)
                .Groups.Any(groupUser =>
                    groupUser.GroupId == SandvikaGroup.Id && groupUser.UserId == EdvardGriegFanUser.Id)
                .ShouldBeFalse();
        }

        [Fact]
        public async Task TestGetUsersInGroup()
        {
            TestServerFixture.ChangeCurrentUserId(SysAdminUser.Id);
            var users = await _mediator.Send(new QueryUsersInGroup(TrondheimGroup.Id));
            users.Length.ShouldBeGreaterThan(0, "Did not all users");
        }

        [Fact]
        public async Task TestCurrentUserLeaveGroupShouldSucceed()
        {
            TestServerFixture.ChangeCurrentUserId(U2FanUser.Id);

            await _mediator.Send(new RemoveMemberCommand(SandvikaGroup.Id, U2FanUser.Id));

            _testServerFixture.GetContext()
                .Users.SingleOrDefault(user => user.Id == U2FanUser.Id)
                .Groups.Any(groupUser =>
                    groupUser.GroupId == SandvikaGroup.Id && groupUser.UserId == U2FanUser.Id)
                .ShouldBeFalse();
        }

        [Fact]
        public async Task TestRemoveMemberFromGroupWhenCurrentUserIsNotAdminShouldFail()
        {
            TestServerFixture.ChangeCurrentUserId(U2FanUser.Id);

            await Should.ThrowAsync<UnauthorizedAccessException>(async () =>
                await _mediator.Send(new RemoveMemberCommand(TrondheimGroup.Id, DeepPurpleFanUser.Id)));

            _testServerFixture.GetContext()
                .Users.SingleOrDefault(user => user.Id == DeepPurpleFanUser.Id)
                .Groups.Any(groupUser => groupUser.GroupId == TrondheimGroup.Id && groupUser.UserId == DeepPurpleFanUser.Id)
                .ShouldBeTrue();
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
