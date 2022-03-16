using System;
using System.Collections.Generic;
using System.Linq;
using Dissimilis.DbContext.Models;
using Dissimilis.DbContext.Models.Song;
using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsOut;
using Dissimilis.WebAPI.Controllers.BoVoice;
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


        public static List<SongVoice> GetSongVoicesFromDto(Song song, SongSnapshot snapshot, SongVoiceDto[] voiceDtos)
        {
            List<SongVoice> voices = new List<SongVoice>();
            int count = 1;
            foreach (var voiceDto in voiceDtos)
            {
                SongVoice voice = song.Voices.SingleOrDefault(v => v.VoiceNumber == voiceDto.VoiceNumber);
                if (voice == null)
                    voice = SongVoiceDto.ConvertToSongVoice(voiceDto, DateTimeOffset.Now, snapshot.CreatedById, song);
                else
                {
                    voice.VoiceName = voiceDto.VoiceName;
                    voice.VoiceNumber = voiceDto.VoiceNumber;
                }
               
                voice.SongBars = SongBarExtension.GetSongBarsFromDto(voiceDto.Bars, voice);
                voices.Add(voice);
                count++;
            }
            return voices;
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

        public static SongVoice AddComponentInterval(this SongVoice songVoice, SongVoice sourceVoice, int intervalPosition)
        {
            songVoice.DuplicateAllChords(sourceVoice, false);
            songVoice.SongBars = songVoice.SongBars.Select(bar =>
                {
                    bar.AddComponentInterval(intervalPosition);
                    bar.RemoveEmptyChords();
                    return bar;
                } 
                ).ToArray();

            return songVoice;
        }

        public static SongVoice RemoveComponentInterval(this SongVoice songVoice, int intervalPosition, bool deleteChordsOnLastIntervalRemoved)
        {
            songVoice.SongBars = songVoice.SongBars.Select(bar =>
                {
                    bar.RemoveComponentInterval(intervalPosition);
                    if (deleteChordsOnLastIntervalRemoved)
                        bar.RemoveEmptyChords();
                    return bar;
                }
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

        public static HashSet<string> GetAllIntervalNames(this SongVoice songVoice)
        {
            return songVoice.SongBars
                .SelectMany(bar => bar.GetAllIntervalNames())
                .ToHashSet();
        }
    }
}
