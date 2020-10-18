using System;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Dissimilis.DbContext.Models.Song;
using Dissimilis.WebAPI.Controllers.BoSong;
using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsIn;
using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsOut;
using Dissimilis.WebAPI.Controllers.BoVoice;
using Dissimilis.WebAPI.DTOs;
using Dissimilis.WebAPI.Extensions.Models;
using Dissimilis.WebAPI.xUnit.Setup;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Dissimilis.WebAPI.xUnit.Tests
{
    [Collection("Serial")]
    public class SongVoiceTests : BaseTestClass
    {
        public SongVoiceTests(TestServerFixture testServerFixture) : base(testServerFixture)
        {
        }

        [Fact]
        public async Task TestNewSongVoice()
        {
            var mediator = _testServerFixture.GetServiceProvider().GetService<IMediator>();

            var createSongDto = SongTests.newCreateSongDto(4, 4);

            var updatedSongCommandDto = await mediator.Send(new CreateSongCommand(createSongDto));
            var songDto = await mediator.Send(new QuerySongById(updatedSongCommandDto.SongId));

            songDto.Voices.FirstOrDefault()?.Bars.FirstOrDefault()?.ChordsAndNotes.FirstOrDefault()?.Length.ShouldBe(8, "Length of standard pause note is not correct");

            songDto.Voices.Length.ShouldBe(1, "It should only be one voice created by default");
            var voice = songDto.Voices.First();

            voice.Bars.Length.ShouldBe(1, "It should only be one bar created by default");
            var bar = voice.Bars.First();

            bar.ChordsAndNotes.Length.ShouldBe(1, "It should only be one cord/note created by default");
            var cordNote = bar.ChordsAndNotes.First();
            cordNote.Position.ShouldBe(0, "The default empty position should be 0");
            cordNote.Length.ShouldBe(8, "The default length should be 8");
        }

        [Fact]
        public async Task TestUpdatingBar()
        {
            var mediator = _testServerFixture.GetServiceProvider().GetService<IMediator>();

            var createSongDto = SongTests.newCreateSongDto(4, 4);

            var updatedSongCommandDto = await mediator.Send(new CreateSongCommand(createSongDto));
            var songDto = await mediator.Send(new QuerySongById(updatedSongCommandDto.SongId));

            songDto.Voices.FirstOrDefault()?.Bars.FirstOrDefault()?.ChordsAndNotes.FirstOrDefault()?.Length.ShouldBe(8, "Length of standard pause note is not correct");

            var voice = songDto.Voices.First();
            var bar = voice.Bars.First();

            var note1Post = await mediator.Send(new CreateSongNoteCommand(songDto.SongId, voice.SongVoiceId, bar.BarId, CreateNoteDto(6, 2)));
            songDto = await mediator.Send(new QuerySongById(updatedSongCommandDto.SongId));
            songDto.Voices.First().Bars.First().ChordsAndNotes.Length.ShouldBe(2);
            var zNote1Update1 = songDto.Voices.First().Bars.First().ChordsAndNotes.FirstOrDefault(n => n.Notes.Any(n => n == "Z"));
            zNote1Update1.Position.ShouldBe(0, "The pause should start at position 0");
            zNote1Update1.Length.ShouldBe(6, "The length of the pause should be 6");

            var note1Update1 = songDto.Voices.First().Bars.First().ChordsAndNotes.FirstOrDefault(n => n.NoteId == note1Post.SongNoteId);
            note1Update1.Position.ShouldBe(6, "Position of note1Update1 should be 6");
            note1Update1.Length.ShouldBe(2, "Length of note1Update1 should be 2");

            await mediator.Send(new UpdateSongNoteCommand(songDto.SongId, voice.SongVoiceId, bar.BarId, note1Post.SongNoteId, GetUpdateNoteDto(note1Update1, position: 7, length: 1)));
            await mediator.Send(new UpdateSongNoteCommand(songDto.SongId, voice.SongVoiceId, bar.BarId, note1Post.SongNoteId, GetUpdateNoteDto(note1Update1, position: 5, length: 3)));
            await mediator.Send(new UpdateSongNoteCommand(songDto.SongId, voice.SongVoiceId, bar.BarId, note1Post.SongNoteId, GetUpdateNoteDto(note1Update1, position: 4, length: 4)));
            await mediator.Send(new UpdateSongNoteCommand(songDto.SongId, voice.SongVoiceId, bar.BarId, note1Post.SongNoteId, GetUpdateNoteDto(note1Update1, position: 3, length: 5)));
            await mediator.Send(new UpdateSongNoteCommand(songDto.SongId, voice.SongVoiceId, bar.BarId, note1Post.SongNoteId, GetUpdateNoteDto(note1Update1, position: 2, length: 6)));
            await mediator.Send(new UpdateSongNoteCommand(songDto.SongId, voice.SongVoiceId, bar.BarId, note1Post.SongNoteId, GetUpdateNoteDto(note1Update1, position: 1, length: 7)));
            await mediator.Send(new UpdateSongNoteCommand(songDto.SongId, voice.SongVoiceId, bar.BarId, note1Post.SongNoteId, GetUpdateNoteDto(note1Update1, position: 0, length: 8)));
        }

        public static UpdateNoteDto GetUpdateNoteDto(NoteDto noteDto, int? position = null, int? length = null, string[] notes = null)
        {
            var updateDto = new UpdateNoteDto()
            {
                Position = noteDto.Position,
                Length = noteDto.Length,
                Notes = noteDto.Notes
            };

            if (position != null)
            {
                updateDto.Position = position.Value;
            }

            if (length != null)
            {
                updateDto.Length = length.Value;
            }

            if (notes != null)
            {
                updateDto.Notes = notes;
            }

            return updateDto;
        }

        public static CreateNoteDto CreateNoteDto(int position, int lenght, string[] value = null)
        {
            if (value == null)
            {
                value = new string[] { "A" };
            }

            return new CreateNoteDto()
            {
                Position = position,
                Length = lenght,
                Notes = value
            };
        }

    }


}

