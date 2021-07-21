using System;
using System.Linq;
using Dissimilis.Core.Collections;
using Dissimilis.DbContext.Models.Song;
using Dissimilis.DbContext.Models.Enums;
using Dissimilis.WebAPI.Controllers.BoVoice.DtoModelsOut;
using Dissimilis.DbContext.Models;
using System.Collections.Generic;

namespace Dissimilis.WebAPI.Controllers.BoSong.DtoModelsOut
{
    public class SongByIdDto
    {
        public int SongId { get; set; }
        public string Title { get; set; }

        public int Numerator { get; set; }
        public int Denominator { get; set; }

        public string ArrangerName { get; set; }

        public DateTimeOffset? UpdatedOn { get; set; }

        public SongVoiceDto[] Voices { get; set; }
        public string SongNotes { get; set; }

        public int? Speed { get; set; }

        public string ProtectionLevel { get; set; }

        public int? DegreeOfDifficulty { get; set; }

        public string Composer { get; set; }
        
        public string Creator { get; set; }
        public string UpdatedBy { get; set; }
        public SongByIdDto(Song song)
        {
            SongId = song.Id;
            Title = song.Title;
            ArrangerName = song.Arranger?.Name;
            UpdatedOn = song.UpdatedOn;
            UpdatedBy = song.UpdatedBy?.Name;
            Numerator = song.Numerator;
            ProtectionLevel = song.ProtectionLevel.GetDescription();
            Denominator = song.Denominator;
            Speed = song.Speed;
            DegreeOfDifficulty = song.DegreeOfDifficulty;
            SongNotes = song.SongNotes;
            Composer = song.Composer;
            Creator = song.CreatedBy?.Name;
            Voices = song.Voices
                .Select(p => new SongVoiceDto(p))
                .OrderBy(p => p.PartNumber)
                .ToArray();
        }
    }
}
