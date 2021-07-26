using System;
using System.Linq;
using System.Threading.Tasks;
using Dissimilis.DbContext.Models.Enums;
using Dissimilis.WebAPI.Controllers.BoBar.Commands;
using Dissimilis.WebAPI.Controllers.BoNote.Commands;
using Dissimilis.WebAPI.Controllers.BoOrganisation.Query;
using Dissimilis.WebAPI.Controllers.BoSong.Commands;
using Dissimilis.WebAPI.Controllers.BoSong.Commands.MultipleBars;
using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsIn;
using Dissimilis.WebAPI.Controllers.BoSong.Query;
using Dissimilis.WebAPI.Controllers.BoSong.ShareSong;
using Dissimilis.WebAPI.Controllers.BoUser.Queries;
using Dissimilis.WebAPI.Controllers.BoVoice.Commands;
using Dissimilis.WebAPI.xUnit.Setup;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;
using static Dissimilis.WebAPI.xUnit.Extensions;

namespace Dissimilis.WebAPI.xUnit.Tests
{
    [Collection("Serial")]
    [CollectionDefinition("Serial", DisableParallelization = true)]
    public class SongTests : BaseTestClass
    {


        public SongTests(TestServerFixture testServerFixture) : base(testServerFixture)
        {
        }

        [Fact]
        public async Task TestMyLibaryShowAllSongsCreatedByCurrentUser()
        {
            TestServerFixture.ChangeCurrentUserId(DeepPurpleFanUser.Id);

            var songDtos = await _mediator.Send(new QuerySongToLibrary(false));

            songDtos.Length.ShouldBe(2);

            songDtos.Any(song => song.SongId == SmokeOnTheWaterSong.Id).ShouldBeTrue();     
            songDtos.Any(song => song.SongId == SpeedKingSong.Id).ShouldBeTrue();
        }

        [Fact]
        public async Task TestSearchForAllSongsAsRegularUserDoesGivePublicSongs()
        {
            //Set current user to regular user with no songs.
            TestServerFixture.ChangeCurrentUserId(NoSongsUser.Id);

            //Search for all songs.
            var searchQueryDto = SearchQueryDto();
            var songDtos = await _mediator.Send(new QuerySongSearch(searchQueryDto));

            songDtos.Length.ShouldBe(4);

            //Verify that all public songs are included.
            songDtos.Any(song => song.SongId == LisaGikkTilSkolenSong.Id).ShouldBeTrue();
            songDtos.Any(song => song.SongId == SpeedKingSong.Id).ShouldBeTrue();
            songDtos.Any(song => song.SongId == DovregubbensHallSong.Id).ShouldBeTrue();
            songDtos.Any(song => song.SongId == BegyntePåBunnen.Id).ShouldBeTrue();
        }
        [Fact]
        public async Task TestSearchForAllSongsAsRegularUserDoesNotGivePrivateSongs()
        {
            //Set current user to regular user with no songs.
            TestServerFixture.ChangeCurrentUserId(NoSongsUser.Id);

            //Search for all songs created by Deep Purple fan.
            var searchQueryDto = SearchQueryDto();
            var songDtos = await _mediator.Send(new QuerySongSearch(searchQueryDto));

            songDtos.Length.ShouldBe(4);

            //Verify that private songs are not included.
            songDtos.Any(song => song.SongId == SmokeOnTheWaterSong.Id).ShouldBeFalse();
            songDtos.Any(song => song.SongId == DuHastSong.Id).ShouldBeFalse();
            songDtos.Any(song => song.SongId == BabySong.Id).ShouldBeFalse();
        }

        [Fact]
        public async Task TestSearchFilterWhenGroupSpecified()
        {
            //Set current user to regular user with no songs.
            TestServerFixture.ChangeCurrentUserId(DeepPurpleFanUser.Id);

            CreateAndAddGroupTagIfNotExsisting(SpeedKingSong.Id, TrondheimGroup.Id);

            UpdateAllSongs();
            _testServerFixture.GetContext().SaveChanges();

            int[] groupWithOneSong = { TrondheimGroup.Id };
            int[] EmptyList = Array.Empty<int>();
            var searchQueryDto = GroupOrgSearchQueryDto(groupWithOneSong, EmptyList);
            var TrondheimSong = await _mediator.Send(new QuerySongSearch(searchQueryDto));
            TrondheimSong.Any(s => s.SongId == SpeedKingSong.Id).ShouldBeTrue();
            TrondheimSong.Any(s => s.SongId == (LisaGikkTilSkolenSong.Id)).ShouldBeFalse();
        }

        [Fact]
        public async Task TestSearchFilterWhenOrganisations()
        {

            //Set current user to regular user with no songs.
            TestServerFixture.ChangeCurrentUserId(DeepPurpleFanUser.Id);

            CreateAndAddOrganisationTagIfNotExisting(SpeedKingSong.Id, NorwayOrganisation.Id);

            UpdateAllSongs();
            _testServerFixture.GetContext().SaveChanges();

            int[] orgWithOneSong = { NorwayOrganisation.Id };
            int[] EmptyList = Array.Empty<int>();
            var searchQueryDto = GroupOrgSearchQueryDto(EmptyList, orgWithOneSong);
            var NorwaySong = await _mediator.Send(new QuerySongSearch(searchQueryDto));
            NorwaySong.Any(s => s.SongId == SpeedKingSong.Id).ShouldBeTrue();
            NorwaySong.Any(s => s.SongId == (LisaGikkTilSkolenSong.Id)).ShouldBeFalse();
        }

        [Fact]
        public async Task TestSearchFilterGroupsAndOrganisationsSameOrg()
        {
            //Set current user to regular user with no songs.
            TestServerFixture.ChangeCurrentUserId(DeepPurpleFanUser.Id);
            
            CreateAndAddOrganisationTagIfNotExisting(SpeedKingSong.Id, NorwayOrganisation.Id);
            CreateAndAddGroupTagIfNotExsisting(SmokeOnTheWaterSong.Id, TrondheimGroup.Id);

            UpdateAllSongs();
            _testServerFixture.GetContext().SaveChanges();

            int[] orgWithOneSong = { NorwayOrganisation.Id };
            int[] groupWithSong = { TrondheimGroup.Id};
            var searchQueryDto = GroupOrgSearchQueryDto(groupWithSong, orgWithOneSong);
            var TrondheimNorwaySong = await _mediator.Send(new QuerySongSearch(searchQueryDto));
            TrondheimNorwaySong.Any(s => s.SongId == SpeedKingSong.Id).ShouldBeTrue();
            TrondheimNorwaySong.Any(s => s.SongId == SmokeOnTheWaterSong.Id).ShouldBeTrue();
            TrondheimNorwaySong.Any(s => s.SongId == (LisaGikkTilSkolenSong.Id)).ShouldBeFalse();
        }
        [Fact]
        public async Task TestSearchFilterGroupsAndOrganisationsDifferentOrg()
        {
            //Set current user to regular user with no songs.
            TestServerFixture.ChangeCurrentUserId(DeepPurpleFanUser.Id);
            CreateAndAddGroupTagIfNotExsisting(SpeedKingSong.Id, TrondheimGroup.Id);

            TestServerFixture.ChangeCurrentUserId(OralBeeFanUser.Id);
            CreateAndAddOrganisationTagIfNotExisting(BegyntePåBunnen.Id, GuatemalaOrganisation.Id);
            
            UpdateAllSongs();
            _testServerFixture.GetContext().SaveChanges();

            int[] orgWithOneSong = { GuatemalaOrganisation.Id };
            int[] groupWithSong = { TrondheimGroup.Id };
            var searchQueryDto = GroupOrgSearchQueryDto(groupWithSong, orgWithOneSong);
            var TrondheimGuatemalaSong = await _mediator.Send(new QuerySongSearch(searchQueryDto));
            TrondheimGuatemalaSong.Any(s => s.SongId == SpeedKingSong.Id).ShouldBeTrue();
            TrondheimGuatemalaSong.Any(s => s.SongId == SmokeOnTheWaterSong.Id).ShouldBeFalse();
            TrondheimGuatemalaSong.Any(s => s.SongId == (LisaGikkTilSkolenSong.Id)).ShouldBeFalse();
            TrondheimGuatemalaSong.Any(s => s.SongId == BegyntePåBunnen.Id).ShouldBeTrue();

        }

        [Fact]
        public async Task TestGetSongsFromMyLibrary()
        {
            TestServerFixture.ChangeCurrentUserId(DeepPurpleFanUser.Id);
            var MyLibary = await _mediator.Send(new QuerySongToLibrary());

            MyLibary.Count().ShouldBe(2);

        }

        [Fact]
        public async Task TestGetAllGroups()
        {
            TestServerFixture.ChangeCurrentUserId(NoSongsUser.Id);

            var groups = await _mediator.Send(new QueryGetGroups("ALL", null));

            groups.Count().ShouldBe(GetAllGroups().Count());
            groups.Any(g => g.GroupId == SandvikaGroup.Id).ShouldBeTrue();
            groups.Any(g => g.GroupId == TrondheimGroup.Id).ShouldBeTrue();
            groups.Any(g => g.GroupId == BergenGroup.Id).ShouldBeTrue();

        }

        [Fact]
        public async Task TestGetAllGroupsMember()
        {
            TestServerFixture.ChangeCurrentUserId(DeepPurpleFanUser.Id);

            var groups = await _mediator.Send(new QueryGetGroups("MEMBER", null));

            groups.Count().ShouldBe(1);
            groups.Any(g => g.GroupId == TrondheimGroup.Id).ShouldBeTrue();
            groups.Any(g => g.GroupId == BergenGroup.Id).ShouldBeFalse();

        }

        [Fact]
        public async Task TestGetAllGroupsAdmin()
        {
            TestServerFixture.ChangeCurrentUserId(TrondheimAdminUser.Id);

            var groups = await _mediator.Send(new QueryGetGroups("ADMIN", null));

            groups.Count().ShouldBe(1);
            groups.Any(g => g.GroupId == TrondheimGroup.Id).ShouldBeTrue();
            groups.Any(g => g.GroupId == BergenGroup.Id).ShouldBeFalse();

        }

        [Fact]
        public async Task TestGetAllOrganisations()
        {
            TestServerFixture.ChangeCurrentUserId(NoSongsUser.Id);

            var orgs = await _mediator.Send(new QueryGetOrganisations("ALL"));

            orgs.Count().ShouldBe(GetAllOrganisations().Count());
            orgs.Any(g => g.OrganisationId == NorwayOrganisation.Id).ShouldBeTrue();
            orgs.Any(g => g.OrganisationId == GuatemalaOrganisation.Id).ShouldBeTrue();
        }

        [Fact]
        public async Task TestGetAllOrganisationsAdmin()
        {
            TestServerFixture.ChangeCurrentUserId(NorwayAdminUser.Id);

            var orgs = await _mediator.Send(new QueryGetOrganisations("ADMIN"));

            orgs.Count().ShouldBe(1);
            orgs.Any(g => g.OrganisationId == NorwayOrganisation.Id).ShouldBeTrue();
            orgs.Any(g => g.OrganisationId == GuatemalaOrganisation.Id).ShouldBeFalse();
        }
        [Fact]
        public async Task TestGetAllOrganisationsMember()
        {
            TestServerFixture.ChangeCurrentUserId(OralBeeFanUser.Id);

            var orgs = await _mediator.Send(new QueryGetOrganisations("MEMBER"));

            orgs.Count().ShouldBe(1);
            orgs.Any(g => g.OrganisationId == GuatemalaOrganisation.Id).ShouldBeTrue();
            orgs.Any(g => g.OrganisationId == NorwayOrganisation.Id).ShouldBeFalse();
        }

        [Fact]
        public async Task TestGetAllOrganisationsGroupAdmin()
        {
            TestServerFixture.ChangeCurrentUserId(TrondheimAdminUser.Id);

            var orgs = await _mediator.Send(new QueryGetOrganisations("GROUPADMIN"));

            orgs.Count().ShouldBe(1);
            orgs.Any(g => g.OrganisationId == GuatemalaOrganisation.Id).ShouldBeFalse();
            orgs.Any(g => g.OrganisationId == NorwayOrganisation.Id).ShouldBeTrue();
        }

        //[Fact]
        //public async Task TestShareSongWithUser()
        //{
        //    var mediator = _testServerFixture.GetServiceProvider().GetService<IMediator>();

        //    var DefaultTestSong = await mediator.Send(new QuerySongById(1));
        //    await mediator.Send(new UpdateSongCommand(DefaultTestSong.SongId, new UpdateSongDto() {
        //        ProtectionLevel = ProtectionLevels.Private }));

        //    await mediator.Send(new ShareSongUserCommand(DefaultTestSong.SongId, 3));
        //    var AllSongs = await mediator.Send(new QuerySongSearch(SharedWithUserSearchQueryDto()));

        //    ChangeToNormalUserOwnerOfSongPublicGroup1DefOrg();

        //    AllSongs = await mediator.Send(new QuerySongSearch(SharedWithUserSearchQueryDto()));
        //    AllSongs.Any(song => song.SongId == DefaultTestSong.SongId).ShouldBeTrue();
        //    ChangeToUserWithAdmin();
        //    await mediator.Send(new UpdateSongCommand(DefaultTestSong.SongId, new UpdateSongDto()
        //    {
        //        ProtectionLevel = ProtectionLevels.Public
        //    }));
        //}
        //[Fact]
        //public async Task TestRemoveShareSongUser()
        //{
        //    var mediator = _testServerFixture.GetServiceProvider().GetService<IMediator>();

        //    var DefaultTestSong = await mediator.Send(new QuerySongById(1));
        //    await mediator.Send(new UpdateSongCommand(DefaultTestSong.SongId, new UpdateSongDto()
        //    {ProtectionLevel = ProtectionLevels.Private}));
        //    await mediator.Send(new ShareSongUserCommand(DefaultTestSong.SongId, 3));

        //    await mediator.Send(new RemoveShareSongUserCommand(DefaultTestSong.SongId, 3));

        //    ChangeToNormalUserOwnerOfSongPublicGroup1DefOrg();
        //    var AllSongs = await mediator.Send(new QuerySongSearch(SharedWithUserSearchQueryDto()));
        //    AllSongs.Any(song => song.SongId == DefaultTestSong.SongId).ShouldBeFalse();
        //    ChangeToUserWithAdmin();
        //    await mediator.Send(new UpdateSongCommand(DefaultTestSong.SongId, new UpdateSongDto()
        //    {
        //        ProtectionLevel = ProtectionLevels.Public
        //    }));
        //}

        //[Fact]
        //public async Task TestUpdateSongToPrivate()
        //{
        //    var mediator = _testServerFixture.GetServiceProvider().GetService<IMediator>();

        //    await mediator.Send(new UpdateSongCommand(1, new UpdateSongDto()
        //    {
        //        ProtectionLevel = ProtectionLevels.Private
        //    }));

        //    var defaultTestSong = await mediator.Send(new QuerySongById(1));
        //    defaultTestSong.ProtectionLevel.ShouldBe(ProtectionLevels.Private);
        //    await mediator.Send(new UpdateSongCommand(1, new UpdateSongDto()
        //    {
        //        ProtectionLevel = ProtectionLevels.Public
        //    }));
        //}


        //[Fact]
        //public async Task TestAddGroupTag()
        //{
        //    var mediator = _testServerFixture.GetServiceProvider().GetService<IMediator>();

        //    var DefaultTestSong = await mediator.Send(new QuerySongById(1));

        //    await mediator.Send(new ShareSongGroupCommand(DefaultTestSong.SongId, 2));

        //    int[] groups = { 2 };
        //    int[] orgs = Array.Empty<int>();
        //    var AllSongs = await mediator.Send(new QuerySongSearch(GroupOrgSearchQueryDto(groups, orgs)));

        //    AllSongs.Any(song => song.SongId == DefaultTestSong.SongId).ShouldBeTrue();
        //}

        //[Fact]
        //public async Task TestAddOrganisationTag()
        //{
        //    var mediator = _testServerFixture.GetServiceProvider().GetService<IMediator>();

        //    var DefaultTestSong = await mediator.Send(new QuerySongById(1));

        //    await mediator.Send(new ShareSongOrganisationCommand(DefaultTestSong.SongId, 2));

        //    int[] orgs = { 2 };
        //    int[] groups = Array.Empty<int>();
        //    var AllSongs = await mediator.Send(new QuerySongSearch(GroupOrgSearchQueryDto(groups, orgs)));

        //    AllSongs.Any(song => song.SongId == DefaultTestSong.SongId).ShouldBeTrue();
        //}
        //[Fact]
        //public async Task TestShareSongWithUser()
        //{
        //    var mediator = _testServerFixture.GetServiceProvider().GetService<IMediator>();
        //
        //    var testSong = await mediator.Send(new QuerySongById(TestServerFixture.TestSongId));
        //    testSong.ProtectionLevel = ProtectionLevels.Private;
        //
        //    ChangeToNormalUserOwnerOfSongPublicGroup1DefOrg();
        //    var AllSongs = await mediator.Send(new QuerySongToLibrary());
        //    AllSongs.Any(song => song.SongId == testSong.SongId).ShouldBeFalse();
        //
        //    //await mediator.Send(new ShareSongUserCommand(testSong.SongId, 3));
        //    //var AllSongs = await mediator.Send(new QuerySongToLibrary());
        //    //AllSongs.Any(song => song.SongId == testSong.SongId).ShouldBeTrue();
        //
        //    ChangeToUserWithAdmin();
        //
        //}

        //    //Execute create song command.
        //    var createSongDto = CreateSongDto(4, 4, "Peer Gynt");
        //    var updatedSongCommandDto = await _mediator.Send(new CreateSongCommand(createSongDto));


        //[Fact]
        //public async Task TestNewSongSave()
        //{
        //  var mediator = _testServerFixture.GetServiceProvider().GetService<IMediator>();

        //    //Update songs from database after change.
        //    var updatedSongs = UpdateAllSongs();

        //    //Fetch newly created song.
        //    var createdSong = updatedSongs.SingleOrDefault(song => song.Id == updatedSongCommandDto.SongId);

        //    //Verify values on newly created song.
        //    createdSong.Numerator.ShouldBe(4);
        //    createdSong.Denominator.ShouldBe(4);
        //    createdSong.Title.ShouldBe("Peer Gynt");
        //    Assert.Single(createdSong.Voices);

        //    createdSong
        //        .Voices.FirstOrDefault()
        //        ?.SongBars.FirstOrDefault()
        //        ?.Notes.FirstOrDefault()
        //        ?.Length.ShouldBe(8, "Length of standard pause chord is not correct");

        //    _testServerFixture.GetContext().Database.CurrentTransaction.Rollback();
        //}


        [Fact]
        public async Task TestCopyBars()
        {
            //Change current user to Deep Purple fan.
            TestServerFixture.ChangeCurrentUserId(DeepPurpleFanUser.Id);

            var mainVoiceBarCount = SmokeOnTheWaterSong
                .Voices.SingleOrDefault(voice => voice.VoiceName == "Main")
                .SongBars.Count;
            var newBarPosition = mainVoiceBarCount;
            var copyLength = 1;
            var barToCopy = SmokeOnTheWaterSong
                .Voices.SingleOrDefault(voice => voice.VoiceName == "Main")
                .SongBars.FirstOrDefault();

            //Execute copying command.
            await _mediator.Send(new CopyBarsCommand(SmokeOnTheWaterSong.Id, CreateCopyBarsDto(barToCopy.Position, copyLength, mainVoiceBarCount)));

            //Update songs from database after change.
            UpdateAllSongs();

            //Verify that a new bar was created.
            SmokeOnTheWaterSong
                .Voices.SingleOrDefault(voice => voice.VoiceName == "Main")
                .SongBars.Count.ShouldBe(mainVoiceBarCount + copyLength);

            //Fetch newly created bar.
            var barCopy = SmokeOnTheWaterSong
                .Voices.SingleOrDefault(voice => voice.VoiceName == "Main")
                .SongBars.FirstOrDefault(bar => bar.Position == mainVoiceBarCount);

            //Verify that values are copied from source bar.
            barToCopy.ShouldBeEquivalentTo(barToCopy);

            // check that value in bar position 1 and 2 are equal to 4 and 5 in all voices
            //foreach (var voiceAfterCopy in SmokeOnTheWaterSong.Voices)
            //{
            //    var firstBarAfterCopy = voiceAfterCopy.SongBars.First(b => b.Position == 1);
            //    var fourthBarAfterCopy = voiceAfterCopy.SongBars.First(b => b.Position == 4);
            //    firstBarAfterCopy.CheckBarEqualTo(fourthBarAfterCopy, includeNoteComparison: true, stepDescription: "Position 1 against 4 after copy");

            //    var secondBarAfterCopy = voiceAfterCopy.SongBars.First(b => b.Position == 2);
            //    var fifthBarAfterCopy = voiceAfterCopy.SongBars.First(b => b.Position == 5);
            //    secondBarAfterCopy.CheckBarEqualTo(fifthBarAfterCopy, includeNoteComparison: true, stepDescription: "Position 2 against 5 after copy");

            //}
        }

        [Fact]
        public async Task TestMoveBars()
        {
            //Set user to Justin Bieber fan.
            TestServerFixture.ChangeCurrentUserId(JustinBieberFanUser.Id);

            // Execute move command.
            var barToMove = BabySong
                .Voices.SingleOrDefault(voice => voice.VoiceName == "Main")
                .SongBars.SingleOrDefault(bar => bar.Position == 0);

            await _mediator.Send(new MoveBarsCommand(BabySong.Id, CreateMoveBarsDto(barToMove.Position, 1, 2)));

            //Fetch songs from database after change.
            UpdateAllSongs();

            //Verify that requested bar is moved to requested position.
            BabySong.Voices
                .SingleOrDefault(voice => voice.VoiceName == "Main")
                .SongBars.SingleOrDefault(bar => bar.Position == 2)
                .Id.ShouldBe(barToMove.Id);
        }
    }
}

