using System;
using System.Collections.Generic;
using System.Linq;
using Dissimilis.DbContext.Models;
using Dissimilis.DbContext.Models.Song;
using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsOut;
using Dissimilis.WebAPI.Controllers.BoVoice.DtoModelsOut;
using Dissimilis.WebAPI.Extensions.Interfaces;
using Newtonsoft.Json.Linq;

namespace Dissimilis.WebAPI.Extensions.Models
{
    public static class SongVoiceExtension
    {
        public static void SortBars(this SongVoice songVoice)
        {
            var number = 1;
            var orderedList = songVoice.SongBars.OrderBy(b => b.Position);
            foreach (var songBar in orderedList)
            {
                songBar.Position = number++;
            }
        }

        public static void SetSongVoiceUpdated(this SongVoice songVoice, int userId)
        {
            songVoice.SetUpdated(userId);
            songVoice.Song.SetUpdated(userId);
        }


        public static (List<SongVoice>, List<SongNote> removeNotes) GetSongVoicesFromDto(Song song, SongSnapshot snapshot, SongVoiceDto[] voiceDtos)
        {
            List<SongVoice> voices = new List<SongVoice>();
            List<SongNote> removeNotes = new List<SongNote>();
            foreach (var voiceDto in voiceDtos)
            {
                SongVoice voice = song.Voices.SingleOrDefault(v => v.VoiceNumber == voiceDto.PartNumber);
                if (voice == null)
                    voice = SongVoiceDto.ConvertToSongVoice(voiceDto, DateTimeOffset.Now, snapshot.CreatedById, song);
               
                voice.SongBars = SongBarExtension.GetSongBarsFromDto(voiceDto.Bars, voice, removeNotes);
                voices.Add(voice);
            }
            return (voices, removeNotes);
        }

        public static SongVoice Clone(this SongVoice songVoice, string VoiceName, User user = null, Instrument instrument = null, int voiceNumber = -1)
        {
            var newSongVoice = new SongVoice()
            {
                SongBars = songVoice.SongBars.Select(b => b.Clone()).ToArray(),
                Instrument = instrument ?? songVoice.Instrument,
                VoiceName = VoiceName,
                VoiceNumber = voiceNumber == -1 ? songVoice.VoiceNumber : voiceNumber + 1
            };

            if (newSongVoice.VoiceNumber != songVoice.VoiceNumber)
            {
                newSongVoice.IsMainVoice = false;
            } 
            else
            {
                newSongVoice.IsMainVoice = songVoice.IsMainVoice;
            }

            newSongVoice.SetCreated(user?.Id ?? songVoice.CreatedById);

            return newSongVoice;
        }

        public static SongVoice AddComponentInterval(this SongVoice songVoice, int intervalPosition)
        {
            songVoice.SongBars = songVoice.SongBars.Select(bar =>
                    bar.AddComponentInterval(intervalPosition)
                ).ToArray();
            return songVoice;
        }

        public static SongVoice RemoveComponentInterval(this SongVoice songVoice, int intervalPosition)
        {
            songVoice.SongBars = songVoice.SongBars.Select(bar =>
                    bar.RemoveComponentInterval(intervalPosition)
                ).ToArray();
            return songVoice;
        }

        public static SongVoice DuplicateAllChords(this SongVoice songVoice, SongVoice sourceSongVoice, bool includeComponentIntervals = true)
        {
            songVoice.SongBars = songVoice.SongBars.Select(bar =>
            {
                var sourceBar = sourceSongVoice.SongBars.First(srcBar => srcBar.Position == bar.Position);
                return bar.DuplicateAllChords(sourceBar, includeComponentIntervals);
            }).ToList();
            return songVoice;
        }

        public static SongVoice Transpose(this SongVoice songVoice, int transpose = 0)
        {
            songVoice.SongBars = songVoice.SongBars.Select(bar =>
                    bar.Transpose(transpose)
                ).ToArray();
            return songVoice;
        }
    }
}
