﻿using Dissimilis.WebAPI.xUnit.Setup;
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
        
        [Fact]
        public async Task TestAddMemberToGroupWhenCurrentUserIsAdminShouldSucceed()
        {
            TestServerFixture.ChangeCurrentUserId(SandvikaAdminUser.Id);

            await _mediator.Send(new AddMemberCommand(SandvikaGroup.Id, new AddMemberDto() { NewMemberUserId = RammsteinFanUser.Id, NewMemberRole = "Member" }));

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

            await _mediator.Send(new AddMemberCommand(BergenGroup.Id, new AddMemberDto() { NewMemberUserId = RammsteinFanUser.Id, NewMemberRole = "Admin" }));

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
                await _mediator.Send(new AddMemberCommand(BergenGroup.Id, new AddMemberDto() { NewMemberUserId = DeepPurpleFanUser.Id, NewMemberRole = "Admin" })));

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

            await _mediator.Send(new ChangeUserRoleCommand(TrondheimGroup.Id, DeepPurpleFanUser.Id, new ChangeUserRoleDto { RoleToSet = "Admin" }));

            _testServerFixture.GetContext()
                .Users.SingleOrDefault(user => user.Id == DeepPurpleFanUser.Id)
                .Groups.SingleOrDefault(groupUser => groupUser.GroupId == TrondheimGroup.Id && groupUser.UserId == DeepPurpleFanUser.Id)
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
                .Groups.SingleOrDefault(groupUser => groupUser.GroupId == SandvikaGroup.Id && groupUser.UserId == EdvardGriegFanUser.Id)
                .Role.ShouldBe(Role.Member);
        }

        [Fact]
        public async Task TestSetAdminToMemberWhenCurrentUserIsAdminShouldSucceed()
        {
            TestServerFixture.ChangeCurrentUserId(SandvikaAdminUser.Id);

            await _mediator.Send(new ChangeUserRoleCommand(SandvikaGroup.Id, SandvikaAdminUser2.Id, new ChangeUserRoleDto { RoleToSet = "Member" }));

            _testServerFixture.GetContext()
                .Users.SingleOrDefault(user => user.Id == SandvikaAdminUser2.Id)
                .Groups.SingleOrDefault(groupUser => groupUser.GroupId == SandvikaGroup.Id && groupUser.UserId == SandvikaAdminUser2.Id)
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
                .Groups.SingleOrDefault(groupUser => groupUser.GroupId == SandvikaGroup.Id && groupUser.UserId == SandvikaAdminUser.Id)
                .Role.ShouldBe(Role.Admin);
        }

        [Fact]
        public async Task TestSetAdminToMemberWhenCurrentUserIsLastAdminShouldFail()
        {
            TestServerFixture.ChangeCurrentUserId(TrondheimAdminUser.Id);

            await Should.ThrowAsync<InvalidOperationException>(async () =>
                await _mediator.Send(
                    new ChangeUserRoleCommand(TrondheimGroup.Id, TrondheimAdminUser.Id,
                                                new ChangeUserRoleDto { RoleToSet = "Member" })));

            _testServerFixture.GetContext()
                .Users.SingleOrDefault(user => user.Id == TrondheimAdminUser.Id)
                .Groups.SingleOrDefault(groupUser => groupUser.GroupId == TrondheimGroup.Id && groupUser.UserId == TrondheimAdminUser.Id)
                .Role.ShouldBe(Role.Admin);
        }
    }
}
