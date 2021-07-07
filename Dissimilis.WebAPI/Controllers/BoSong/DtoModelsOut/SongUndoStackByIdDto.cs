using System;
using System.Collections.Generic;
using System.Linq;
using Dissimilis.DbContext.Models.Song;

namespace Dissimilis.WebAPI.Controllers.BoSong.DtoModelsOut
{
    public class SongUndoStackByIdDto
    {
        public int SongId { get; set; }
        public SongSnapshot[] Snapshots { get; set; }

        public SongUndoStackByIdDto(Song song)
        {
            SongId = song.Id;
            Snapshots = song.Snapshots.ToArray();
        }
    }
}
