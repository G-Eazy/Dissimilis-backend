using System;
using System.Linq;
using Dissimilis.DbContext.Models.Song;
using Dissimilis.WebAPI.Controllers.BoVoice.DtoModelsOut;

namespace Dissimilis.WebAPI.Controllers.BoSong.DtoModelsOut
{
    public class SongSnapshotDto
    {
        public string SongTitle { get; set; }
        public SongVoiceDto[] SongVoices { get; set; }

    }
}
