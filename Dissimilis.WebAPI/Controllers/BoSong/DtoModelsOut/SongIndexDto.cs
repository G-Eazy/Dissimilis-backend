﻿using System;
using Dissimilis.DbContext.Models.Song;

namespace Dissimilis.WebAPI.Controllers.BoSong.DtoModelsOut
{
    public class SongIndexDto
    {
        public int SongId { get; set; }
        public string Title { get; set; }

        public int Numerator { get; set; }
        public int Denominator { get; set; }

        public string ArrangerName { get; set; }

        public DateTimeOffset? UpdatedOn { get; set; }

        public SongIndexDto(Song song)
        {
            SongId = song.Id;
            Title = song.Title;
            ArrangerName = song.Arranger?.Name;
            UpdatedOn = song.UpdatedOn;
            Numerator = song.Numerator;
            Denominator = song.Denominator;

        }
    }
}