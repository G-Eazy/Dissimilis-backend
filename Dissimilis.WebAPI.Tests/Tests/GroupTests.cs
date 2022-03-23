using Dissimilis.WebAPI.xUnit.Setup;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Shouldly;
using Dissimilis.WebAPI.Controllers.BoGroup.DtoModelsIn;
using Dissimilis.WebAPI.Controllers.BoGroup.Commands;
using Dissimilis.WebAPI.Controllers.MultiUseDtos.DtoModelsIn;
using Dissimilis.WebAPI.Controllers.BoGroup.Query;
using Dissimilis.DbContext.Models.Enums;
using static Dissimilis.WebAPI.xUnit.Extensions;
using Dissimilis.WebAPI.Exceptions;

namespace Dissimilis.WebAPI.xUnit.Tests
{
    [Collection("Serial")]
    [CollectionDefinition("Serial", DisableParallelization = true)]
    public class GroupTests : BaseTestClass
    {
        public GroupTests(TestServerFixture testServerFixture) : base(testServerFixture)
        {
        }

        [Fact]
        public async Task TestAddMemberToGroupWhenCurrentUserIsAdminShouldSucceed()
        {
            TestServerFixture.ChangeCurrentUserId(SandvikaAdminUser.Id);

            await _mediator.Send(new AddMemberCommand(SandvikaGroup.Id, 
                new AddMemberDto() { NewMemberUserId = RammsteinFanUser.Id, NewMemberRole = "Member" }));

            var groupUser = _testServerFixture.GetContext()
                .Users.SingleOrDefault(user => user.Id == RammsteinFanUser.Id)
                .Groups.SingleOrDefault(groupUser => 
                groupUser.GroupId == SandvikaGroup.Id && groupUser.UserId == RammsteinFanUser.Id);
            groupUser.ShouldNotBe(null);
            groupUser.Role.ShouldBe(Role.Member);
        }

        [Fact]
        public async Task TestAddAdminToGroupWhenCurrentUserIsAdminShouldSucceed()
        {
            TestServerFixture.ChangeCurrentUserId(BergenAdminUser.Id);

            await _mediator.Send(new AddMemberCommand(BergenGroup.Id, 
                new AddMemberDto() { NewMemberUserId = RammsteinFanUser.Id, NewMemberRole = "Admin" }));

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
                await _mediator.Send(new AddMemberCommand(BergenGroup.Id, 
                new AddMemberDto() { NewMemberUserId = DeepPurpleFanUser.Id, NewMemberRole = "Admin" })));

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
        public async Task TestRemoveAdminWhenUserIsLastAdminShouldFail()
        {
            TestServerFixture.ChangeCurrentUserId(TrondheimAdminUser.Id);

            await Should.ThrowAsync<InvalidOperationException>(async () =>
                await _mediator.Send(
                    new RemoveMemberCommand(TrondheimGroup.Id, TrondheimAdminUser.Id)));

            _testServerFixture.GetContext()
                .Users.SingleOrDefault(user => user.Id == TrondheimAdminUser.Id)
                .Groups.Any(groupUser => groupUser.GroupId == TrondheimGroup.Id && groupUser.UserId == TrondheimAdminUser.Id)
                .ShouldBeTrue();
        }

        [Fact]
        public async Task TestSetMemberToAdminWhenCurrentUserIsAdminShouldSucceed()
        {
            TestServerFixture.ChangeCurrentUserId(TrondheimAdminUser.Id);

            await _mediator.Send(new ChangeUserRoleCommand(TrondheimGroup.Id, DeepPurpleFanUser.Id, 
                new ChangeUserRoleDto { RoleToSet = "Admin" }));

            _testServerFixture.GetContext()
                .Users.SingleOrDefault(user => user.Id == DeepPurpleFanUser.Id)
                .Groups.SingleOrDefault(groupUser => 
                    groupUser.GroupId == TrondheimGroup.Id && groupUser.UserId == DeepPurpleFanUser.Id)
                .Role.ShouldBe(Role.Admin);
        }

        [Fact]
        public async Task TestSetMemberToAdminWhenCurrentUserIsNotAdminShouldFail()
        {
            TestServerFixture.ChangeCurrentUserId(EdvardGriegFanUser.Id);

            await Should.ThrowAsync<UnauthorizedAccessException>(async () =>
                await _mediator.Send(
                    new ChangeUserRoleCommand(SandvikaGroup.Id, EdvardGriegFanUser.Id,
                                                new ChangeUserRoleDto { RoleToSet = "Admin" })));

            _testServerFixture.GetContext()
                .Users.SingleOrDefault(user => user.Id == EdvardGriegFanUser.Id)
                .Groups.SingleOrDefault(groupUser => 
                    groupUser.GroupId == SandvikaGroup.Id && groupUser.UserId == EdvardGriegFanUser.Id)
                .Role.ShouldBe(Role.Member);
        }

        [Fact]
        public async Task TestSetAdminToMemberWhenCurrentUserIsAdminShouldSucceed()
        {
            TestServerFixture.ChangeCurrentUserId(SandvikaAdminUser.Id);

            await _mediator.Send(new ChangeUserRoleCommand(SandvikaGroup.Id, SandvikaAdminUser2.Id, 
                new ChangeUserRoleDto { RoleToSet = "Member" }));

            _testServerFixture.GetContext()
                .Users.SingleOrDefault(user => user.Id == SandvikaAdminUser2.Id)
                .Groups.SingleOrDefault(groupUser => 
                     groupUser.GroupId == SandvikaGroup.Id && groupUser.UserId == SandvikaAdminUser2.Id)
                .Role.ShouldBe(Role.Member);
        }

        [Fact]
        public async Task TestSetAdminToMemberWhenCurrentUserIsNotAdminShouldFail()
        {
            TestServerFixture.ChangeCurrentUserId(EdvardGriegFanUser.Id);

            await Should.ThrowAsync<UnauthorizedAccessException>(async () =>
                await _mediator.Send(
                    new ChangeUserRoleCommand(SandvikaGroup.Id, SandvikaAdminUser.Id,
                                                new ChangeUserRoleDto { RoleToSet = "Member" })));

            _testServerFixture.GetContext()
                .Users.SingleOrDefault(user => user.Id == SandvikaAdminUser.Id)
                .Groups.SingleOrDefault(groupUser => 
                     groupUser.GroupId == SandvikaGroup.Id && groupUser.UserId == SandvikaAdminUser.Id)
                .Role.ShouldBe(Role.Admin);
        }

        [Fact]
        public async Task UpdateGroupShouldSucceed()
        {
            TestServerFixture.ChangeCurrentUserId(SysAdminUser.Id);

            var updateDto = GetUpdateGroupAndOrganisationDto();
            var updateItem = await _mediator.Send(new UpdateGroupCommand(TrondheimGroup.Id, updateDto));
            var updatedGroup = await _mediator.Send(new QueryGroupById(updateItem.GroupId));

            updatedGroup.Address.ShouldBeEquivalentTo(updateDto.Address, "Address did not match");
            updatedGroup.Email.ShouldBeEquivalentTo(updateDto.Email, "Email was not updated");
            updatedGroup.Description.ShouldBeEquivalentTo(updateDto.Description, "Description was not updated");
            updatedGroup.PhoneNumber.ShouldBeEquivalentTo(updateDto.PhoneNumber, "Phonenumber was not updated");
        }

        [Fact]
        public async Task DeleteGroupWithCorrectIdShouldSucceed()
        {
            TestServerFixture.ChangeCurrentUserId(SysAdminUser.Id);

            var groupId = DeleteGroup.Id;
            await _mediator.Send(new DeleteGroupCommand(groupId));
            var groupShouldBeNull = _testServerFixture.GetContext().Groups
                .SingleOrDefault(g => g.Id == groupId);
            groupShouldBeNull.ShouldBeNull("Group was not deleted...");
        }

        [Fact]
        public async Task DeleteGroupWithIncorrectIdShouldFail()
        {
            TestServerFixture.ChangeCurrentUserId(SysAdminUser.Id);

            var groupId = -1;
            await Should.ThrowAsync<NotFoundException>(async () => await _mediator.Send(new DeleteGroupCommand(groupId)));
        }

        [Fact]
        public async Task DeleteGroupGroupUserShouldBeRemoved()
        {
            TestServerFixture.ChangeCurrentUserId(SysAdminUser.Id);
            UpdateAllOrganisations();
            UpdateAllGroups();
            DeleteGroup = new DbContext.Models.Group()
            {
                Name = "DeleteGroup",
                OrganisationId = NorwayOrganisation.Id,
            };
            _testServerFixture.GetContext().Groups.Add(DeleteGroup);
            await _testServerFixture.GetContext().SaveChangesAsync();

            var deleteUser = new DbContext.Models.GroupUser()
            {
                UserId = SysAdminUser.Id,
                GroupId = DeleteGroup.Id
            };

            _testServerFixture.GetContext().GroupUsers.Add(deleteUser);

            await _testServerFixture.GetContext().SaveChangesAsync();

            var groupId = DeleteGroup.Id;

            var groupUser = _testServerFixture.GetContext().GroupUsers
                .SingleOrDefault(gu => gu.GroupId == groupId);
            groupUser.ShouldNotBeNull($"User with id {deleteUser.UserId}, {deleteUser.GroupId} not found");

            await _mediator.Send(new DeleteGroupCommand(groupId));
            var groupUserShouldBeNull = _testServerFixture.GetContext().GroupUsers
                .SingleOrDefault(gu => gu.GroupId == groupId);
            groupUserShouldBeNull.ShouldBeNull($"GroupUser was not deleted...");
        }

        [Fact]
        public async Task GetUsersInGroupShouldSucceed()
        {
            TestServerFixture.ChangeCurrentUserId(SysAdminUser.Id);
            var users = await _mediator.Send(new QueryUsersInGroup(SandvikaGroup.Id));
            users.ShouldNotBeNull("Users were not fetched...");
        }

        [Fact]
        public async Task TestSetAdminToMemberWhenCurrentUserIsLastAdminShouldFail()
        {
            TestServerFixture.ChangeCurrentUserId(QuetzaltenangoAdminUser.Id);

            await Should.ThrowAsync<InvalidOperationException>(async () =>
                await _mediator.Send(
                    new ChangeUserRoleCommand(QuetzaltenangoGroup.Id, QuetzaltenangoAdminUser.Id,
                                                new ChangeUserRoleDto { RoleToSet = "Member" })));

            _testServerFixture.GetContext()
                .Users.SingleOrDefault(user => user.Id == QuetzaltenangoAdminUser.Id)
                .Groups.SingleOrDefault(groupUser =>
                     groupUser.GroupId == QuetzaltenangoGroup.Id && groupUser.UserId == QuetzaltenangoAdminUser.Id)
                .Role.ShouldBe(Role.Admin);
        }

        [Fact]
        public async Task CreateGroupShouldSucceed()
        {
            TestServerFixture.ChangeCurrentUserId(SysAdminUser.Id);
            var createDto = GetCreateGroupDto(1, NorwayOrganisation.Id, NoSongsUser.Id);
            var item1 = await _mediator.Send(new CreateGroupCommand(createDto));
            var group1 = await _mediator.Send(new QueryGroupById(item1.GroupId));
            group1.GroupName.ShouldBeEquivalentTo("TestGroup1", "Group creation failed");
        }
    }
}
