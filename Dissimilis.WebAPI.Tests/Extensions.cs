using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dissimilis.DbContext.Models.Song;
using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsIn;
using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsOut;
using Dissimilis.WebAPI.Controllers.BoVoice.DtoModelsIn;
using Dissimilis.WebAPI.DTOs;
using Shouldly;

namespace Dissimilis.WebAPI.xUnit
{
    internal static class Extensions
    {

        internal const string DefaultTestSongTitle = "TestSong";

        internal static void CheckSongVoiceIntegrity(SongByIdDto songDto, string stepDescription)
        {
            var refVoice = songDto.Voices.FirstOrDefault();
            if (refVoice == null)
            {
                return;
            }

            foreach (var songVoiceDto in songDto.Voices)
            {
                refVoice.Bars.Length.ShouldBe(songVoiceDto.Bars.Length, "Amount of bars not matching - " + stepDescription);
            }

            var barLength = refVoice.Bars.Length;
            for (var i = 0; i < barLength; i++)
            {
                foreach (var songVoiceDto in songDto.Voices)
                {
                    refVoice.Bars[i].House.ShouldBe(songVoiceDto.Bars[i].House, "House not matching - " + stepDescription);
                    refVoice.Bars[i].RepAfter.ShouldBe(songVoiceDto.Bars[i].RepAfter, "RepAfter not matching - " + stepDescription);
                    refVoice.Bars[i].RepBefore.ShouldBe(songVoiceDto.Bars[i].RepBefore, "RepBefore not matching - " + stepDescription);
                }
            }
        }

        internal static CreateSongDto CreateSongDto(int numerator = 4, int denominator = 4, string title = DefaultTestSongTitle)
        {
            return new CreateSongDto()
            {
                Title = title,
                Numerator = numerator,
                Denominator = denominator,
            };
        }

        internal static Song NewSong(int numerator, int denominator)
        {
            return new Song()
            {
                Numerator = numerator,
                Denominator = denominator
            };
        }

        internal static CreateSongVoiceDto CreateSongVoiceDto(string voiceName, int? voiceNumber = null)
        {
            return new CreateSongVoiceDto()
            {
                Instrument = voiceName,
                VoiceNumber = voiceNumber
            };
        }

        internal static UpdateBarDto CreateUpdateBarDto(int? house = null, bool repAfter = false, bool repBefore = false)
        {
            return new UpdateBarDto()
            {
                House = house,
                RepAfter = repAfter,
                RepBefore = repBefore
            };
        }

        internal static UpdateNoteDto GetUpdateNoteDto(NoteDto noteDto, int? position = null, int? length = null, string[] notes = null)
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

        internal static CreateNoteDto CreateNoteDto(int position, int lenght, string[] value = null)
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

        internal static CreateBarDto CreateBarDto(int? house = null, bool repAfter = false, bool repBefore = false)
        {
            return new CreateBarDto()
            {
                House = house,
                RepAfter = repAfter,
                RepBefore = repBefore
            };
        }
    }
}
