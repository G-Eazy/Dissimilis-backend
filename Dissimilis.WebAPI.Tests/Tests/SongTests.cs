using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dissimilis.DbContext.Models.Enums;
using Dissimilis.DbContext.Models.Song;
using Dissimilis.WebAPI.Controllers.BoBar.Commands;
using Dissimilis.WebAPI.Controllers.BoNote.Commands;
using Dissimilis.WebAPI.Controllers.BoSong;
using Dissimilis.WebAPI.Controllers.BoSong.Commands;
using Dissimilis.WebAPI.Controllers.BoSong.Commands.MultipleBars;
using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsIn;
using Dissimilis.WebAPI.Controllers.BoSong.Query;
using Dissimilis.WebAPI.Controllers.BoSong.ShareSong;
using Dissimilis.WebAPI.Controllers.BoVoice.Commands;
using Dissimilis.WebAPI.Extensions.Models;
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

        public string SongPrivateGroup3SuppOrg = "Supplement test song 1";
        public string SongPublicGroup1DefOrg = "Supplement test song 2";
        public string SongPublicDefOrg = "Supplement test song 3";
        public string SongPublicSuppOrg = "Supplement test song 4";
        public string DefaultSongDefOrg = "Default test song";

        internal static void ChangeToUserWithAdmin()
        {
            TestServerFixture.ChangeCurrentUserId(1);
        }

        internal static void ChangeToNormalUserOwnerOfSongPublicGroup1DefOrg()
        {
            TestServerFixture.ChangeCurrentUserId(3);
        }

        [Fact]
        public void TestMaxBarPositionFunction()
        {

            // Expect 4 parts, but starting at 0
            NewSong(2, 4).GetMaxBarPosition().ShouldBe(4 - 1);

            // Expect 6 parts, but starting at 0
            NewSong(3, 4).GetMaxBarPosition().ShouldBe(6 - 1);

            // Expect 8 parts, but starting at 0 
            NewSong(4, 4).GetMaxBarPosition().ShouldBe(8 - 1);

            // Expect 12 parts, but starting at 0
            NewSong(6, 4).GetMaxBarPosition().ShouldBe(12 - 1);

            // Expect 6 parts, but starting at 0
            NewSong(6, 8).GetMaxBarPosition().ShouldBe(6 - 1);
        }

        [Fact]
        public async Task TestMyLibaryShowRightSongs() 
        {
            //user 3 is the songOwner of SongPublicGroup1DefOrg
            var mediator = _testServerFixture.GetServiceProvider().GetService<IMediator>();
            ChangeToNormalUserOwnerOfSongPublicGroup1DefOrg();
            var CountBefore = await mediator.Send(new QuerySongToLibrary());
            ChangeToUserWithAdmin();

            var AllSongs = await mediator.Send(new QuerySongSearch(AllSearchQueryDto()));
            
            var SongToDuplicateAndTranspose = AllSongs.Where(s => s.Title.Equals(SongPublicGroup1DefOrg.ToString())).FirstOrDefault();

            var duplicate = await mediator.Send(new DuplicateSongCommand(SongToDuplicateAndTranspose.SongId, new DuplicateSongDto() { Title = "DupTestSong2"}));
            var transpose = await mediator.Send(new CreateTransposedSongCommand(SongToDuplicateAndTranspose.SongId, new TransposeSongDto() { Title = "TransposeTestSong2", Transpose = -2}));

            ChangeToNormalUserOwnerOfSongPublicGroup1DefOrg();
            var CountAfter = await mediator.Send(new QuerySongToLibrary());
            CountAfter.Length.ShouldBe(CountBefore.Length);
            ChangeToUserWithAdmin();
        }

        [Fact]
        public async Task TestSearchForSongs()
        {
            var mediator = _testServerFixture.GetServiceProvider().GetService<IMediator>();

            var createSongDto = CreateSongDto(4, 4);
            var updatedSongCommandDto = await mediator.Send(new CreateSongCommand(createSongDto));

            //Search for the user who created the song
            var searchQueryDto = SearchQueryDto();
            var songDtos = await mediator.Send(new QuerySongSearch(searchQueryDto));

            songDtos.Any(s => s.SongId == updatedSongCommandDto.SongId).ShouldBeTrue();
        }
        [Fact]
        public async Task TestSearchDoesNotGivePrivateSong()
        {
            ChangeToNormalUserOwnerOfSongPublicGroup1DefOrg();

            var mediator = _testServerFixture.GetServiceProvider().GetService<IMediator>();

            var searchQueryDto = AllSearchQueryDto();
            var privateSongTitle = await mediator.Send(new QuerySongSearch(searchQueryDto));
            privateSongTitle.Any(s => s.Title.Equals(SongPrivateGroup3SuppOrg)).ShouldBeFalse();
            ChangeToUserWithAdmin();
        }

        [Fact]
        public async Task TestSearchAllSongsAsSysAdmin()
        {
            var mediator = _testServerFixture.GetServiceProvider().GetService<IMediator>();

            var searchQueryDto = AllSearchQueryDto();
            var PrivateSongTitle = await mediator.Send(new QuerySongSearch(searchQueryDto));
            PrivateSongTitle.Any(s => s.Title.Equals(SongPrivateGroup3SuppOrg)).ShouldBeTrue();
        }

        [Fact]
        public async Task TestSearchFilterWhenGroupSpecified()
        {
            var mediator = _testServerFixture.GetServiceProvider().GetService<IMediator>();

            int[] groupWithOneSong = { 3 };
            int[] orgs = Array.Empty<int>();
            var searchQueryDto = GroupOrgSearchQueryDto(groupWithOneSong, orgs);
            var songDtos = await mediator.Send(new QuerySongSearch(searchQueryDto));
            songDtos.Any(s => s.Title.Equals(SongPrivateGroup3SuppOrg)).ShouldBeTrue();
            songDtos.Any(s => s.Title.Equals(DefaultSongDefOrg)).ShouldBeFalse();
            songDtos.Length.ShouldBe(1);
        }

        [Fact]
        public async Task TestSearchFilterWhenOrganisationsSpecifiedDefOrg()
        {
            var mediator = _testServerFixture.GetServiceProvider().GetService<IMediator>();

            int[] defOrg = { 1};
            int[] groups = Array.Empty<int>();
            var searchQueryDto = GroupOrgSearchQueryDto(groups, defOrg);
            var songDtos = await mediator.Send(new QuerySongSearch(searchQueryDto));
            songDtos.Any(s => s.Title.Equals(SongPrivateGroup3SuppOrg)).ShouldBeFalse();
            songDtos.Count().ShouldBe(3);

            int[] suppOrg = { 2 };
            searchQueryDto = GroupOrgSearchQueryDto(groups, suppOrg);
            songDtos = await mediator.Send(new QuerySongSearch(searchQueryDto));
            songDtos.Any(s => s.Title.Equals(SongPrivateGroup3SuppOrg)).ShouldBeTrue();
            songDtos.Any(s => s.Title.Equals(SongPublicSuppOrg)).ShouldBeTrue();
            songDtos.Length.ShouldBe(2);

        }

        [Fact]
        public async Task TestSearchFilterWhenOrganisationsSpecifiedSuppOrg()
        {
            var mediator = _testServerFixture.GetServiceProvider().GetService<IMediator>();

            int[] groups = Array.Empty<int>();
            int[] suppOrg = { 2 };

            var searchQueryDto = GroupOrgSearchQueryDto(groups, suppOrg);
            var filteredSongs = await mediator.Send(new QuerySongSearch(searchQueryDto));
            filteredSongs.Any(s => s.Title.Equals(SongPrivateGroup3SuppOrg)).ShouldBeTrue();
            filteredSongs.Any(s => s.Title.Equals(SongPublicSuppOrg)).ShouldBeTrue();
            filteredSongs.Length.ShouldBe(2);
        }

        [Fact]
        public async Task TestSearchFilterGroupsAndOrganisationsSameOrg()
        {
            var mediator = _testServerFixture.GetServiceProvider().GetService<IMediator>();

            int[] suppOrg = { 2 };
            int[] groups = { 3 };
            
            var searchQueryDto = GroupOrgSearchQueryDto(groups, suppOrg);
            var filteredSongs = await mediator.Send(new QuerySongSearch(searchQueryDto));
            
            filteredSongs.Any(s => s.Title.Equals(SongPrivateGroup3SuppOrg)).ShouldBeTrue();
            filteredSongs.Any(s => s.Title.Equals(SongPublicSuppOrg)).ShouldBeTrue();
            filteredSongs.Any(s => s.Title.Equals(SongPublicGroup1DefOrg)).ShouldBeFalse();
            filteredSongs.Any(s => s.Title.Equals(SongPublicDefOrg)).ShouldBeFalse();
            filteredSongs.Length.ShouldBe(2);

        }
        [Fact]
            public async Task TestSearchFilterGroupsAndOrganisationsDifferentOrg()
            {
            var mediator = _testServerFixture.GetServiceProvider().GetService<IMediator>();
            
            int[] groups = { 3 };
            int[] defOrg = { 1 };

            var searchQueryDto = GroupOrgSearchQueryDto(groups, defOrg);
            var filteredSongs = await mediator.Send(new QuerySongSearch(searchQueryDto));
            
            filteredSongs.Any(s => s.Title.Equals(SongPublicSuppOrg)).ShouldBeFalse();
            filteredSongs.Any(s => s.Title.Equals(DefaultSongDefOrg)).ShouldBeTrue();
            filteredSongs.Any(s => s.Title.Equals(SongPrivateGroup3SuppOrg)).ShouldBeTrue();
            filteredSongs.Length.ShouldBe(4);
        }

        [Fact]
        public async Task TestGetSongsFromMyLibrary()
        {
            var mediator = _testServerFixture.GetServiceProvider().GetService<IMediator>();

            var createSongDto = CreateSongDto(4, 4);
            var updatedSongCommandDto = await mediator.Send(new CreateSongCommand(createSongDto));
            var songDtos = await mediator.Send(new QuerySongToLibrary());

            songDtos.Any(s => s.SongId == updatedSongCommandDto.SongId).ShouldBeTrue();
        }
        [Fact]
        public async Task TestShareSongWithUser()
        {
            var mediator = _testServerFixture.GetServiceProvider().GetService<IMediator>();

            var testSong = await mediator.Send(new QuerySongById(TestServerFixture.TestSongId));
            testSong.ProtectionLevel = ProtectionLevels.Private;

            ChangeToNormalUserOwnerOfSongPublicGroup1DefOrg();
            var AllSongs = await mediator.Send(new QuerySongToLibrary());
            AllSongs.Any(song => song.SongId == testSong.SongId).ShouldBeFalse();

            //await mediator.Send(new ShareSongUserCommand(testSong.SongId, 3));
            //var AllSongs = await mediator.Send(new QuerySongToLibrary());
            //AllSongs.Any(song => song.SongId == testSong.SongId).ShouldBeTrue();

            ChangeToUserWithAdmin();

        }



        [Fact]
        public async Task TestNewSongSave()
        {
            var mediator = _testServerFixture.GetServiceProvider().GetService<IMediator>();

            var createSongDto = CreateSongDto(4, 4);

            var updatedSongCommandDto = await mediator.Send(new CreateSongCommand(createSongDto));
            var songDto = await mediator.Send(new QuerySongById(updatedSongCommandDto.SongId));

            createSongDto.Title.ShouldBe(songDto.Title, "Title not the same after creating");
            createSongDto.Numerator.ShouldBe(songDto.Numerator, "Numerator not the same after creating");
            createSongDto.Denominator.ShouldBe(songDto.Denominator, "Denominator not the same after creating");

            Assert.Single(songDto.Voices);

            songDto.Voices.FirstOrDefault()?.Bars.FirstOrDefault()?.Chords.FirstOrDefault()?.Length.ShouldBe(8, "Length of standard pause chord is not correct");
        }

        [Fact]
        public async Task TestGetTransposedCopyOfSong()
        {
            var mediator = _testServerFixture.GetServiceProvider().GetService<IMediator>();

            var updatedSongCommandDto = await mediator.Send(new CreateSongCommand(CreateSongDto(4, 4)));
            var songDto = await mediator.Send(new QuerySongById(updatedSongCommandDto.SongId));

            var firstVoice = songDto.Voices.First();

            // populate a voice with bars and notes
            var firstVoiceFirstBar = firstVoice.Bars.First();
            await mediator.Send(new CreateSongNoteCommand(songDto.SongId, firstVoice.SongVoiceId, firstVoiceFirstBar.BarId, CreateNoteDto(0, 8, "A#")));

            var firstVoiceSecondBar = await mediator.Send(new CreateSongBarCommand(songDto.SongId, firstVoice.SongVoiceId, CreateBarDto()));
            await mediator.Send(new CreateSongNoteCommand(songDto.SongId, firstVoice.SongVoiceId, firstVoiceSecondBar.SongBarId, CreateNoteDto(0, 8, null, new string[] { "D" } )));

            var firstVoiceThirdBar = await mediator.Send(new CreateSongBarCommand(songDto.SongId, firstVoice.SongVoiceId, CreateBarDto(house: 1)));
            await mediator.Send(new CreateSongNoteCommand(songDto.SongId, firstVoice.SongVoiceId, firstVoiceThirdBar.SongBarId, CreateNoteDto(0, 8, "Cmaj7")));

            // transpose song by +3
            var transposedSongByPlus3 = await mediator.Send(new CreateTransposedSongCommand(songDto.SongId, new TransposeSongDto { Transpose = 3, Title = $"{songDto.Title} (transposed +3)" }));
            var songDtoPlus3 = await mediator.Send(new QuerySongById(transposedSongByPlus3.SongId));

            // transpose song by -5
            var transposedSongByMinus5 = await mediator.Send(new CreateTransposedSongCommand(songDto.SongId, new TransposeSongDto { Transpose = -5, Title = $"{songDto.Title} (transposed -5)" }));
            var songDtoMinus5 = await mediator.Send(new QuerySongById(transposedSongByMinus5.SongId));

            // transpose song by -15
            var transposedSongByMinus15 = await mediator.Send(new CreateTransposedSongCommand(songDto.SongId, new TransposeSongDto { Transpose = -15 }));
            var songDtoMinus15 = await mediator.Send(new QuerySongById(transposedSongByMinus15.SongId));

            // check titles
            songDtoPlus3.Title.ShouldBe($"{songDto.Title} (transposed +3)");
            songDtoMinus5.Title.ShouldBe($"{songDto.Title} (transposed -5)");

            // check copies created
            songDto.SongId.ShouldNotBe(songDtoPlus3.SongId);
            songDto.SongId.ShouldNotBe(songDtoMinus5.SongId);
            songDtoPlus3.SongId.ShouldNotBe(songDtoMinus5.SongId);
            (songDto.SongId == songDtoPlus3.SongId).ShouldBeFalse();

            // check copies of notes created
            songDtoPlus3.Voices[0].Bars[0].Chords[0].Notes[0].ShouldNotBe(
                firstVoiceFirstBar.Chords[0].Notes[0]);

            // check noteValues transposed +3
            var firstCheck = songDtoPlus3.Voices[0].Bars[0].Chords[0].ChordName;
            firstCheck.ShouldBe("C#", "First copied chord value not as expected");

            var secondCheck = songDtoPlus3.Voices[0].Bars[1].Chords.FirstOrDefault(n => n.ChordId != null).Notes;
            secondCheck.ShouldBe(new[] { "F" }, "Second copied chord value not as expected");

            var thirdCheck = songDtoPlus3.Voices[0].Bars[2].Chords.FirstOrDefault(n => n.ChordId != null).ChordName;
            thirdCheck.ShouldBe("D#maj7", "Third copied chord value not as expected");

            // check noteValues transposed -5
            var fourthCheck = songDtoMinus5.Voices[0].Bars[0].Chords.FirstOrDefault(n => n.ChordId != null).ChordName;
            fourthCheck.ShouldBe("F", "First copied chord value not as expected");

            var fifthCheck = songDtoMinus5.Voices[0].Bars[1].Chords.FirstOrDefault(n => n.ChordId != null).Notes;
            fifthCheck.ShouldBe(new[] { "A" }, "Second copied chord value not as expected");

            var sixthCheck = songDtoMinus5.Voices[0].Bars[2].Chords.FirstOrDefault(n => n.ChordId != null).ChordName;
            sixthCheck.ShouldBe("Gmaj7", "Third copied chord value not as expected");

            // check noteValues transposed -15
            var seventhCheck = songDtoMinus15.Voices[0].Bars[0].Chords.FirstOrDefault(n => n.ChordId != null).ChordName;
            seventhCheck.ShouldBe("G", "First copied chord value not as expected");

            var eightCheck = songDtoMinus15.Voices[0].Bars[1].Chords.FirstOrDefault(n => n.ChordId != null).Notes;
            eightCheck.ShouldBe(new[] { "H" }, "Second copied chord value not as expected");

            var ninthCheck = songDtoMinus15.Voices[0].Bars[2].Chords.FirstOrDefault(n => n.ChordId != null).ChordName;
            ninthCheck.ShouldBe("Amaj7", "Third copied chord value not as expected");
        }


        [Fact]
        public async Task TestCopyBars()
        {
            var mediator = _testServerFixture.GetServiceProvider().GetService<IMediator>();

            var updatedSongCommandDto = await mediator.Send(new CreateSongCommand(CreateSongDto(4, 4)));
            var songDto = await mediator.Send(new QuerySongById(updatedSongCommandDto.SongId));

            var firstVoice = songDto.Voices.First();

            // populate a voice with bars and notes
            var firstVoiceFirstBar = firstVoice.Bars.First();
            await mediator.Send(new CreateSongNoteCommand(songDto.SongId, firstVoice.SongVoiceId, firstVoiceFirstBar.BarId, CreateNoteDto(6, 2, null, new[] { "H" })));

            var firstVoiceSecondBar = await mediator.Send(new CreateSongBarCommand(songDto.SongId, firstVoice.SongVoiceId, CreateBarDto()));
            await mediator.Send(new CreateSongNoteCommand(songDto.SongId, firstVoice.SongVoiceId, firstVoiceSecondBar.SongBarId, CreateNoteDto(0, 2, null, value: new[] { "A" })));

            var firstVoiceThirdBar = await mediator.Send(new CreateSongBarCommand(songDto.SongId, firstVoice.SongVoiceId, CreateBarDto(house: 1)));
            await mediator.Send(new CreateSongNoteCommand(songDto.SongId, firstVoice.SongVoiceId, firstVoiceThirdBar.SongBarId, CreateNoteDto(2, 6, null, value: new[] { "C" })));

            var firstVoiceFourthBar = await mediator.Send(new CreateSongBarCommand(songDto.SongId, firstVoice.SongVoiceId, CreateBarDto(house: 2, repAfter: true)));
            await mediator.Send(new CreateSongNoteCommand(songDto.SongId, firstVoice.SongVoiceId, firstVoiceFourthBar.SongBarId, CreateNoteDto(0, 8, null)));

            songDto = await mediator.Send(new QuerySongById(updatedSongCommandDto.SongId));
            songDto.Voices.Length.ShouldBe(1, "It is supposed to be only one voice");

            // make another voice, and set notes on 1 and 2
            var secondVoiceId = await mediator.Send(new CreateSongVoiceCommand(songDto.SongId, CreateSongVoiceDto("SecondVoice")));
            songDto = await mediator.Send(new QuerySongById(updatedSongCommandDto.SongId));
            var secondVoice = songDto.Voices.First(v => v.SongVoiceId == secondVoiceId.SongVoiceId);
            var secondVoiceFirstBar = secondVoice.Bars.OrderBy(b => b.Position).First();
            await mediator.Send(new CreateSongNoteCommand(songDto.SongId, secondVoice.SongVoiceId, secondVoiceFirstBar.BarId, CreateNoteDto(0, 2, null, value: new[] { "F" })));
            var secondVoiceSecondBar = secondVoice.Bars.OrderBy(b => b.Position).Skip(1).First();
            await mediator.Send(new CreateSongNoteCommand(songDto.SongId, secondVoice.SongVoiceId, secondVoiceSecondBar.BarId, CreateNoteDto(2, 6, null, value: new[] { "G" })));

            songDto.Voices.First().Bars.Length.ShouldBe(4, "It should be 4 bars before copying");


            // do Copying
            await mediator.Send(new CopyBarsCommand(songDto.SongId, CreateCopyBarsDto(1, 2, 4)));
            var afterCopySongDto = await mediator.Send(new QuerySongById(updatedSongCommandDto.SongId));

            // single check
            var fistCheck = afterCopySongDto.Voices.First(v => v.SongVoiceId == firstVoice.SongVoiceId).Bars.First(b => b.Position == 4)
                .Chords.FirstOrDefault(n => n.ChordId != null).Notes;
            fistCheck.ShouldBe(new[] { "H" }, "First copied chord value not as expected");

            var secondCheck = afterCopySongDto.Voices.First(v => v.SongVoiceId == firstVoice.SongVoiceId).Bars.First(b => b.Position == 5)
                            .Chords.FirstOrDefault(n => n.ChordId != null).Notes;
            secondCheck.ShouldBe(new[] { "A" }, "Second copied chord value not as expected");

            // Check that the first voice is the main voice and that the next voice is not the main
            afterCopySongDto.Voices[0].IsMain.ShouldBe(true);
            afterCopySongDto.Voices[1].IsMain.ShouldBe(false);

            // check index
            afterCopySongDto.Voices.First(v => v.SongVoiceId == firstVoiceFirstBar.SongVoiceId).Bars.Length.ShouldBe(6, "It should be 6 bars after copying");
            var index = 1;
            foreach (var bar in afterCopySongDto.Voices.First().Bars)
            {
                bar.Position.ShouldBe(index++, "Index is not as expected after copying");
            }


            // check that value in bar position 1 and 2 are equal to 4 and 5 in all voices
            foreach (var voiceAfterCopy in afterCopySongDto.Voices)
            {
                var firstBarAfterCopy = voiceAfterCopy.Bars.First(b => b.Position == 1);
                var fourthBarAfterCopy = voiceAfterCopy.Bars.First(b => b.Position == 4);
                firstBarAfterCopy.CheckBarEqualTo(fourthBarAfterCopy, includeNoteComparison: true, stepDescription: "Position 1 against 4 after copy");

                var secondBarAfterCopy = voiceAfterCopy.Bars.First(b => b.Position == 2);
                var fifthBarAfterCopy = voiceAfterCopy.Bars.First(b => b.Position == 5);
                secondBarAfterCopy.CheckBarEqualTo(fifthBarAfterCopy, includeNoteComparison: true, stepDescription: "Position 2 against 5 after copy");

            }

        }

        [Fact]
        public async Task TestMoveBars()
        {
            var mediator = _testServerFixture.GetServiceProvider().GetService<IMediator>();

            var updatedSongCommandDto = await mediator.Send(new CreateSongCommand(CreateSongDto(4, 4)));
            var songDto = await mediator.Send(new QuerySongById(updatedSongCommandDto.SongId));

            var firstVoice = songDto.Voices.First();

            // populate a voice with bars and notes
            var firstVoiceFirstBar = firstVoice.Bars.First();
            await mediator.Send(new CreateSongNoteCommand(songDto.SongId, firstVoice.SongVoiceId, firstVoiceFirstBar.BarId, CreateNoteDto(0, 8, "A")));

            var firstVoiceSecondBar = await mediator.Send(new CreateSongBarCommand(songDto.SongId, firstVoice.SongVoiceId, CreateBarDto()));
            await mediator.Send(new CreateSongNoteCommand(songDto.SongId, firstVoice.SongVoiceId, firstVoiceSecondBar.SongBarId, CreateNoteDto(0, 8, "D")));

            var firstVoiceThirdBar = await mediator.Send(new CreateSongBarCommand(songDto.SongId, firstVoice.SongVoiceId, CreateBarDto(house: 1)));
            await mediator.Send(new CreateSongNoteCommand(songDto.SongId, firstVoice.SongVoiceId, firstVoiceThirdBar.SongBarId, CreateNoteDto(0, 8, null, new string[] { "C" } )));

            var firstVoiceFourthBar = await mediator.Send(new CreateSongBarCommand(songDto.SongId, firstVoice.SongVoiceId, CreateBarDto(house: 2, repAfter: true)));
            await mediator.Send(new CreateSongNoteCommand(songDto.SongId, firstVoice.SongVoiceId, firstVoiceFourthBar.SongBarId, CreateNoteDto(0, 8, "Hm")));
            
            songDto = await mediator.Send(new QuerySongById(updatedSongCommandDto.SongId));
            songDto.Voices.Length.ShouldBe(1, "It is supposed to be only one voice");

            // make another voice, and set notes on 1 and 2
            var secondVoiceId = await mediator.Send(new CreateSongVoiceCommand(songDto.SongId, CreateSongVoiceDto("SecondVoice")));

            songDto = await mediator.Send(new QuerySongById(updatedSongCommandDto.SongId));
            var secondVoice = songDto.Voices.First(v => v.SongVoiceId == secondVoiceId.SongVoiceId);

            var secondVoiceFirstBar = secondVoice.Bars.OrderBy(b => b.Position).First();
            await mediator.Send(new CreateSongNoteCommand(songDto.SongId, secondVoice.SongVoiceId, secondVoiceFirstBar.BarId, CreateNoteDto(0, 2, null, value: new[] { "F" })));

            var secondVoiceSecondBar = secondVoice.Bars.OrderBy(b => b.Position).Skip(1).First();
            await mediator.Send(new CreateSongNoteCommand(songDto.SongId, secondVoice.SongVoiceId, secondVoiceSecondBar.BarId, CreateNoteDto(2, 6, null, value: new[] { "G" })));
            songDto.Voices.First().Bars.Length.ShouldBe(4, "It should be 4 bars before moving");


            // do Moving
            await mediator.Send(new MoveBarsCommand(songDto.SongId, CreateMoveBarsDto(0, 2, 3)));
            var afterCopySongDto = await mediator.Send(new QuerySongById(updatedSongCommandDto.SongId));

            // single check
            var fistCheck = afterCopySongDto.Voices.First(v => v.SongVoiceId == firstVoice.SongVoiceId).Bars.First(b => b.Position == 1).Chords.FirstOrDefault(n => n.ChordId != null).Notes;
            fistCheck.ShouldBe(new string[] { "C" }, "First copied chord value not as expected");

            var secondCheck = afterCopySongDto.Voices.First(v => v.SongVoiceId == firstVoice.SongVoiceId).Bars.First(b => b.Position == 4).Chords.FirstOrDefault(n => n.ChordId != null).ChordName;
            secondCheck.ShouldBe("Hm", "Second copied chord value not as expected");

            // check index
            afterCopySongDto.Voices.First(v => v.SongVoiceId == firstVoiceFirstBar.SongVoiceId).Bars.Length.ShouldBe(4, "It should be 4 bars after moving");
            var index = 1;
            foreach (var bar in afterCopySongDto.Voices.First().Bars)
            {
                bar.Position.ShouldBe(index++, "Index is not as expected after copying");
            }

        }
    }
}

