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
using Dissimilis.WebAPI.Exceptions;

namespace Dissimilis.WebAPI.xUnit.Tests
{
    [Collection("Serial")]
    [CollectionDefinition("Serial", DisableParallelization = true)]
    public class OrganisationTests : BaseTestClass
    {

        public OrganisationTests(TestServerFixture testServerFixture) : base(testServerFixture)
        {
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
        public async Task CreateOrganisationShouldSucceed()
        {
            TestServerFixture.ChangeCurrentUserId(SysAdminUser.Id);
            CreateOrganisationDto orgDto = new CreateOrganisationDto()
            {
                Name = "TestOrg1",
                FirstAdminId = NorwayAdminUser.Id
            };

            var item = await _mediator.Send(new CreateOrganisationCommand(orgDto));
            var org = await _mediator.Send(new QueryOrganisationById(item.OrganisationId));
            org.Name.ShouldBeEquivalentTo("TestOrg1", "Creation of organisation failed");
        }


        [Fact]
        public async Task GetUsersInOrganisationShouldSucceed()
        {
            TestServerFixture.ChangeCurrentUserId(SysAdminUser.Id);
            var users = await _mediator.Send(new QueryUsersInOrganisation(NorwayOrganisation.Id));
            users.Length.ShouldBeGreaterThan(0, "Did not get all users");
        }

        [Fact]
        public async Task UpdateOrganisationShouldSucceed()
        {
            TestServerFixture.ChangeCurrentUserId(SysAdminUser.Id);
            var updateDto = GetUpdateGroupAndOrganisationDto();
            var updateItem = await _mediator.Send(new UpdateOrganisationCommand(GuatemalaOrganisation.Id, updateDto));
            var updatedOrg = await _mediator.Send(new QueryOrganisationById(updateItem.OrganisationId));

            updatedOrg.Name.ShouldBeEquivalentTo(updateDto.Name, "Name did not match");
            updatedOrg.Address.ShouldBeEquivalentTo(updateDto.Address, "Address did not match");
            updatedOrg.Email.ShouldBeEquivalentTo(updateDto.Email, "Email was not updated");
            updatedOrg.Description.ShouldBeEquivalentTo(updateDto.Description, "Description was not updated");
            updatedOrg.PhoneNumber.ShouldBeEquivalentTo(updateDto.PhoneNumber, "Phonenumber was not updated");
        }

        [Fact]
        public async Task DeleteOrganisationWithCorrectIdShouldSucceed()
        {
            TestServerFixture.ChangeCurrentUserId(SysAdminUser.Id);

            var organisationId = DeleteOrganisation.Id;
            await _mediator.Send(new DeleteOrganisationCommand(organisationId));
            var organisationShouldBeNull = _testServerFixture.GetContext().Organisations
                .SingleOrDefault(g => g.Id == organisationId);
            organisationShouldBeNull.ShouldBeNull($"Group was not deleted...");
        }

        [Fact]
        public async Task DeleteOrganisationWithIncorrectIdShouldSucceed()
        {
            TestServerFixture.ChangeCurrentUserId(SysAdminUser.Id);

            var organisationId = -1;
            await Should.ThrowAsync<NotFoundException>(async () => await _mediator.Send(new DeleteOrganisationCommand(organisationId)));
        }
    }
}
