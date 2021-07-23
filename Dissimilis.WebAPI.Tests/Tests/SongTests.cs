using System;
using System.Linq;
using System.Threading.Tasks;
using Dissimilis.DbContext.Models.Enums;
using Dissimilis.WebAPI.Controllers.BoBar.Commands;
using Dissimilis.WebAPI.Controllers.BoNote.Commands;
using Dissimilis.WebAPI.Controllers.BoSong.Commands;
using Dissimilis.WebAPI.Controllers.BoSong.Commands.MultipleBars;
using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsIn;
using Dissimilis.WebAPI.Controllers.BoSong.Query;
using Dissimilis.WebAPI.Controllers.BoSong.ShareSong;
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

            songDtos.Length.ShouldBe(3);

            //Verify that all public songs are included.
            songDtos.Any(song => song.SongId == LisaGikkTilSkolenSong.Id).ShouldBeTrue();
            songDtos.Any(song => song.SongId == SpeedKingSong.Id).ShouldBeTrue();
            songDtos.Any(song => song.SongId == DovregubbensHallSong.Id).ShouldBeTrue();
        }
        [Fact]
        public async Task TestSearchForAllSongsAsRegularUserDoesNotGivePrivateSongs()
        {
            //Set current user to regular user with no songs.
            TestServerFixture.ChangeCurrentUserId(NoSongsUser.Id);

            //Search for all songs created by Deep Purple fan.
            var searchQueryDto = SearchQueryDto();
            var songDtos = await _mediator.Send(new QuerySongSearch(searchQueryDto));

            songDtos.Length.ShouldBe(3);

            //Verify that private songs are not included.
            songDtos.Any(song => song.SongId == SmokeOnTheWaterSong.Id).ShouldBeFalse();
            songDtos.Any(song => song.SongId == DuHastSong.Id).ShouldBeFalse();
            songDtos.Any(song => song.SongId == BabySong.Id).ShouldBeFalse();
        }

        //[Fact]
        //public async Task TestSearchFilterWhenGroupSpecified()
        //{

        //    int[] groupWithOneSong = { 3 };
        //    int[] orgs = Array.Empty<int>();
        //    var searchQueryDto = GroupOrgSearchQueryDto(groupWithOneSong, orgs);
        //    var songDtos = await mediator.Send(new QuerySongSearch(searchQueryDto));
        //    songDtos.Any(s => s.Title.Equals(SongPrivateGroup3SuppOrg)).ShouldBeTrue();
        //    songDtos.Any(s => s.Title.Equals(DefaultSongDefOrg)).ShouldBeFalse();
        //    songDtos.Length.ShouldBe(1);
        //}

        //[Fact]
        //public async Task TestSearchFilterWhenOrganisationsSpecifiedDefOrg()
        //{
        //    var mediator = _testServerFixture.GetServiceProvider().GetService<IMediator>();

        //    int[] defOrg = { 1};
        //    int[] groups = Array.Empty<int>();
        //    var searchQueryDto = GroupOrgSearchQueryDto(groups, defOrg);
        //    var songDtos = await mediator.Send(new QuerySongSearch(searchQueryDto));
        //    songDtos.Any(s => s.Title.Equals(SongPrivateGroup3SuppOrg)).ShouldBeFalse();
        //    songDtos.Count().ShouldBe(3);

        //    int[] suppOrg = { 2 };
        //    searchQueryDto = GroupOrgSearchQueryDto(groups, suppOrg);
        //    songDtos = await mediator.Send(new QuerySongSearch(searchQueryDto));
        //    songDtos.Any(s => s.Title.Equals(SongPrivateGroup3SuppOrg)).ShouldBeTrue();
        //    songDtos.Any(s => s.Title.Equals(SongPublicSuppOrg)).ShouldBeTrue();
        //    songDtos.Length.ShouldBe(2);

        //}

        //[Fact]
        //public async Task TestSearchFilterWhenOrganisationsSpecifiedSuppOrg()
        //{
        //    var mediator = _testServerFixture.GetServiceProvider().GetService<IMediator>();

        //    int[] groups = Array.Empty<int>();
        //    int[] suppOrg = { 2 };

        //    var searchQueryDto = GroupOrgSearchQueryDto(groups, suppOrg);
        //    var filteredSongs = await mediator.Send(new QuerySongSearch(searchQueryDto));
        //    filteredSongs.Any(s => s.Title.Equals(SongPrivateGroup3SuppOrg)).ShouldBeTrue();
        //    filteredSongs.Any(s => s.Title.Equals(SongPublicSuppOrg)).ShouldBeTrue();
        //    filteredSongs.Length.ShouldBe(2);
        //}

        //[Fact]
        //public async Task TestSearchFilterGroupsAndOrganisationsSameOrg()
        //{
        //    var mediator = _testServerFixture.GetServiceProvider().GetService<IMediator>();

        //    int[] suppOrg = { 2 };
        //    int[] groups = { 3 };
            
        //    var searchQueryDto = GroupOrgSearchQueryDto(groups, suppOrg);
        //    var filteredSongs = await mediator.Send(new QuerySongSearch(searchQueryDto));
            
        //    filteredSongs.Any(s => s.Title.Equals(SongPrivateGroup3SuppOrg)).ShouldBeTrue();
        //    filteredSongs.Any(s => s.Title.Equals(SongPublicSuppOrg)).ShouldBeTrue();
        //    filteredSongs.Any(s => s.Title.Equals(SongPublicGroup1DefOrg)).ShouldBeFalse();
        //    filteredSongs.Any(s => s.Title.Equals(SongPublicDefOrg)).ShouldBeFalse();
        //    filteredSongs.Length.ShouldBe(2);

        //}
        //[Fact]
        //    public async Task TestSearchFilterGroupsAndOrganisationsDifferentOrg()
        //    {
        //    var mediator = _testServerFixture.GetServiceProvider().GetService<IMediator>();
            
        //    int[] groups = { 3 };
        //    int[] defOrg = { 1 };

        //    var searchQueryDto = GroupOrgSearchQueryDto(groups, defOrg);
        //    var filteredSongs = await mediator.Send(new QuerySongSearch(searchQueryDto));
            
        //    filteredSongs.Any(s => s.Title.Equals(SongPublicSuppOrg)).ShouldBeFalse();
        //    filteredSongs.Any(s => s.Title.Equals(DefaultSongDefOrg)).ShouldBeTrue();
        //    filteredSongs.Any(s => s.Title.Equals(SongPrivateGroup3SuppOrg)).ShouldBeTrue();
        //    filteredSongs.Length.ShouldBe(4);
        //}

        //[Fact]
        //public async Task TestGetSongsFromMyLibrary()
        //{
        //    var mediator = _testServerFixture.GetServiceProvider().GetService<IMediator>();

        //    var createSongDto = CreateSongDto(4, 4);
        //    var updatedSongCommandDto = await mediator.Send(new CreateSongCommand(createSongDto));
        //    var songDtos = await mediator.Send(new QuerySongToLibrary());

        //    songDtos.Any(s => s.SongId == updatedSongCommandDto.SongId).ShouldBeTrue();

        //    _testServerFixture.GetContext().Database.CurrentTransaction.Rollback();
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
        public async Task TestGetTransposedCopyOfSong()
        {
            
        }


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

