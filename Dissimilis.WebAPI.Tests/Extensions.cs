using System.Linq;
using Dissimilis.DbContext.Models;
using Dissimilis.DbContext.Models.Enums;
using Dissimilis.DbContext.Models.Song;
using Dissimilis.WebAPI.Controllers.BoBar.DtoModelsIn;
using Dissimilis.WebAPI.Controllers.BoBar.DtoModelsOut;
using Dissimilis.WebAPI.Controllers.BoGroup.DtoModelsIn;
using Dissimilis.WebAPI.Controllers.BoNote.DtoModelsIn;
using Dissimilis.WebAPI.Controllers.BoNote.DtoModelsOut;
using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsIn;
using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsOut;
using Dissimilis.WebAPI.Controllers.BoVoice.DtoModelsIn;
using Dissimilis.WebAPI.Controllers.BoVoice.DtoModelsOut;
using Dissimilis.WebAPI.Controllers.MultiUseDtos.DtoModelsIn;
using Dissimilis.WebAPI.Extensions.Models;
using Shouldly;

namespace Dissimilis.WebAPI.xUnit
{
    internal static class Extensions
    {
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
                    refVoice.Bars[i].CheckBarEqualTo(songVoiceDto.Bars[i], stepDescription: stepDescription);
                }
            }
        }

        internal static void CheckVoiceBarsEqualTo(this SongVoiceDto firstVoiceDto, SongVoiceDto secondVoiceDto, bool includeNoteComparison)
        {
            firstVoiceDto.Bars.Length.ShouldBe(secondVoiceDto.Bars.Length, "Number of bars are not equal");
            var barLength = firstVoiceDto.Bars.Length;
            for (var i = 0; i < barLength; i++)
            {
                firstVoiceDto.Bars[i].CheckBarEqualTo(secondVoiceDto.Bars[i], true, $"Check bar position {firstVoiceDto.Bars[i].Position}");
            }

        }

        internal static void CheckBarEqualTo(this BarDto firstBarDto, BarDto secondBarDto, bool includeNoteComparison = false, string stepDescription = null)
        {
            firstBarDto.House.ShouldBe(secondBarDto.House, "House not matching - " + stepDescription);
            firstBarDto.RepAfter.ShouldBe(secondBarDto.RepAfter, "RepAfter not matching - " + stepDescription);
            firstBarDto.RepBefore.ShouldBe(secondBarDto.RepBefore, "RepBefore not matching - " + stepDescription);

            if (!includeNoteComparison)
            {
                return;
            }

            firstBarDto.Chords.Length.ShouldBe(secondBarDto.Chords.Length, "Not matching amount of notes - " + stepDescription);
            var barLength = firstBarDto.Chords.Length;
            for (var barI = 0; barI < barLength; barI++)
            {
                firstBarDto.Chords[barI].Notes.Length.ShouldBe(secondBarDto.Chords[barI].Notes.Length, "Not matching amount of notes - " + stepDescription);
                var numberOfNotes = firstBarDto.Chords[barI].Notes.Length;
                for (var noteI = 0; noteI < numberOfNotes; noteI++)
                {
                    firstBarDto.Chords[barI].Notes[noteI].ShouldBe(secondBarDto.Chords[barI].Notes[noteI], "Note values is not as expected - " + stepDescription);
                }

            }
        }

        internal static CreateSongDto CreateSongDto(string title, int numerator = 4, int denominator = 4)
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
                VoiceName = voiceName,
                VoiceNumber = voiceNumber
            };
        }

        internal static DuplicateSongDto DuplicateSongDto(string title)
        {
            return new DuplicateSongDto()
            {
                Title = title
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

        internal static CopyBarDto CreateCopyBarsDto(int fromPosition, int copyLength, int toPosition)
        {
            return new CopyBarDto()
            {
                FromPosition = fromPosition,
                CopyLength = copyLength,
                ToPosition = toPosition
            };
        }

        internal static MoveBarDto CreateMoveBarsDto(int fromPosition, int moveLength, int toPosition)
        {
            return new MoveBarDto()
            {
                FromPosition = fromPosition,
                MoveLength = moveLength,
                ToPostition = toPosition
            };
        }

        internal static CreateNoteDto CreateNoteDto(int position, int length, string chordName, string[] value = null)
        {
            if (value == null && chordName == null)
            {
                value = new string[] { "A" };
            }

            return new CreateNoteDto()
            {
                Position = position,
                Length = length,
                Notes = chordName != null ? SongNoteExtension.GetNoteValuesFromChordName(chordName).ToArray() : value,
                ChordName = chordName
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

        internal static DuplicateAllChordsDto DuplicateAllChordsDto(int sourceVoiceId, bool includeComponentIntervals)
        {
            return new DuplicateAllChordsDto()
            {
                SourceVoiceId = sourceVoiceId,
                IncludeComponentIntervals = includeComponentIntervals,
            };
        }

        internal static AddComponentIntervalDto AddComponentIntervalDto(int IntervalPosition)
        {
            return new AddComponentIntervalDto()
            {
                IntervalPosition = IntervalPosition
            };
        }

        internal static RemoveComponentIntervalDto RemoveComponentIntervalDto(int IntervalPosition)
        {
            return new RemoveComponentIntervalDto()
            {
                IntervalPosition = IntervalPosition
            };
        }

        internal static SearchQueryDto SearchQueryDto(
            string title = "", string OrderBy = "date", bool orderDescending = true, bool includeAll = true)
        {
            return new SearchQueryDto()
            {
                Title = title,
                OrderBy = OrderBy,
                OrderDescending = orderDescending,
                IncludeAll = includeAll,
            };
        }
        internal static SearchQueryDto AllSearchQueryDto()
        {
            return new SearchQueryDto()
            {
                Title = "",
                OrderBy = "date",
                OrderDescending = true,
                IncludeAll = true,
            };

        }
        internal static SearchQueryDto MyLibarySearchQueryDto(int currentUserId)
        {
            return new SearchQueryDto()
            {
                Title = "",
                OrderBy = "date",
                OrderDescending = true,
                ArrangerId = currentUserId,
            };

        }
        internal static SearchQueryDto SharedWithUserSearchQueryDto()
        {
            return new SearchQueryDto()
            {
                Title = "",
                OrderBy = "date",
                IncludeAll = false,
                OrderDescending = true,
                IncludeSharedWithUser = true
            };

        }

        internal static SearchQueryDto GroupOrgSearchQueryDto(int[] groups, int[] orgs)
        {
            return new SearchQueryDto()
            {
                Title = "",
                OrderBy = "date",
                OrderDescending = true,
                IncludeAll = false,
                IncludedGroupIdArray = groups,
                IncludedOrganisationIdArray = orgs
            };

        }

        internal static CreateGroupDto GetCreateGroupDto(int groupNumber, int orgId, int adminId)
        {
            return new CreateGroupDto()
            {
                Name = $"TestGroup{groupNumber}",
                OrganisationId = orgId,
                FirstAdminId = adminId
            };
        }
        
        internal static UpdateGroupAndOrganisationDto GetUpdateGroupAndOrganisationDto()
        {
            return new UpdateGroupAndOrganisationDto()
            {
                Address = "Nidarosveien 58",
                Email = "NewAdmin@Trondheim_Norway.no",
                Description = "The new and hip Trondheim group.",
                PhoneNumber = "90944999",
            };
        }
    }
}
