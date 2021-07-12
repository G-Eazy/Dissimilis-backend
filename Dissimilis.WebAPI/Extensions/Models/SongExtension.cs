using System.Linq;
using System.Threading.Tasks;
using Dissimilis.Core.Collections;
using Dissimilis.DbContext.Models;
using Dissimilis.DbContext.Models.Enums;
using Dissimilis.DbContext.Models.Song;
using Dissimilis.WebAPI.Exceptions;
using Dissimilis.WebAPI.Extensions.Interfaces;

namespace Dissimilis.WebAPI.Extensions.Models
{
    public static class SongExtension
    {
        /// <summary>
        /// Return true if the given user have readpermission on this song
        /// </summary>
        /// <param name="song"></param>
        /// <param name="user"> user to check readpermission</param>
        /// <returns> true if readpermission</returns>
        public static bool ReadAccessToSong(this Song song, User user)
        {
            return song.ProtectionLevel == ProtectionLevels.All
            //include songs created by you
            || song.ArrangerId == user.Id
            || song.CreatedById == user.Id
            //include only songs shared with you by userSharing
            || !song.SharedUsers.All(shared => shared.UserId != user.Id)
            //include only songs within your groups
            || !song.SharedGroups.All(songGroup =>
                user.Groups.All(userGroup =>
                songGroup.GroupId != userGroup.GroupId))
            //include only songs within your organisations
            || !song.SharedOrganisations.All(songOrg =>
                user.Organisations.All(userOrg =>
                songOrg.OrganisationId != userOrg.OrganisationId));
        }


        /// <summary>
        /// Get max bar positions for a song
        /// </summary>
        /// <param name="song"></param>
        /// <returns></returns>
        public static int GetMaxBarPosition(this Song song)
        {
            var eightParts = 8 / song.Denominator;

            var amountOfWhole = (double)song.Numerator / song.Denominator;

            return (int)((eightParts * song.Denominator) * amountOfWhole) - 1;
        }

        /// <summary>
        /// Ensures all voices has the same amount of bars
        /// </summary>
        /// <param name="song"></param>
        public static void SyncBarCountToMaxInAllVoices(this Song song)
        {

            var maxBarCount = song.Voices
                .Select(v => v.SongBars.Count)
                .OrderByDescending(v => v)
                .FirstOrDefault();
            if (maxBarCount == 0)
            {
                maxBarCount = 1;
            }

            foreach (var songVoice in song.Voices)
            {
                var highestBarNumber = songVoice.SongBars.Select(sb => sb.Position)
                    .OrderByDescending(v => v)
                    .FirstOrDefault();

                highestBarNumber++;
                while (songVoice.SongBars.Count < maxBarCount)
                {
                    songVoice.SongBars = songVoice.SongBars.Concat(new[] { new SongBar(highestBarNumber++) }).ToArray();
                }

                songVoice.SortBars();
            }
        }

        /// <summary>
        /// Sets the song and all voices to same Updated datetime and user
        /// </summary>
        /// <param name="song"></param>
        /// <param name="userId"></param>
        public static void SetUpdatedOverAll(this Song song, int userId)
        {
            foreach (var songVoice in song.Voices)
            {
                songVoice.SetSongVoiceUpdated(userId);
            }
            song.SetUpdated(userId);
        }

        /// <summary>
        /// Copies (length of) bars from one position to another position to in the song
        /// </summary>
        public static void CopyBars(this Song song, int fromPosition, int copyLength, int toPosition)
        {
            SyncBarCountToMaxInAllVoices(song);

            var firstVoice = song.Voices.FirstOrDefault();
            if (firstVoice == null || firstVoice.SongBars.Count() < fromPosition)
            {
                throw new NotFoundException("Voice or bar not found");
            }

            foreach (var voice in song.Voices)
            {
                foreach (var bar in voice.SongBars)
                {
                    if (bar.Position >= toPosition)
                    {
                        bar.Position += copyLength;
                    }
                }

                var barsToInsert = voice.SongBars.OrderBy(b => b.Position)
                    .Skip(fromPosition - 1)
                    .Take(copyLength)
                    .Select(b => b.Clone())
                    .ToArray();
                var startIndex = toPosition;
                foreach (var insertBar in barsToInsert)
                {
                    insertBar.Position = startIndex++;
                }

                voice.SongBars = voice.SongBars.Concat(barsToInsert).ToArray();
            }

        }

        /// <summary>
        /// Copies (length of) bars from one position to another position to in the song
        /// </summary>
        public static void MoveBars(this Song song, int fromPosition, int moveLength, int toPosition)
        {
            SyncBarCountToMaxInAllVoices(song);

            var firstVoice = song.Voices.FirstOrDefault();
            if (firstVoice == null || firstVoice.SongBars.Count() < fromPosition)
            {
                throw new NotFoundException("Voice or bar not found");
            }

            foreach (var voice in song.Voices)
            {
                foreach (var bar in voice.SongBars)
                {
                    if (bar.Position > toPosition)
                    {
                        bar.Position += moveLength;
                    }
                    else
                    {
                        bar.Position -= moveLength;
                    }
                }

                var barsToMove = voice.SongBars.OrderBy(b => b.Position)
                    .Skip(fromPosition - 1)
                    .Take(moveLength)
                    .ToArray();
                var startIndex = toPosition;
                foreach (var insertBar in barsToMove)
                {
                    insertBar.Position = startIndex++;
                }
            }

            SyncBarCountToMaxInAllVoices(song);
        }

        /// <summary>
        /// Deletes bars 
        /// </summary>
        public static void DeleteBars(this Song song, int fromPosition, int deleteLength)
        {
            SyncBarCountToMaxInAllVoices(song);

            var firstVoice = song.Voices.FirstOrDefault();
            var firstVoiceLength = firstVoice.SongBars.Count();
            if (firstVoice == null || firstVoiceLength < fromPosition)
            {
                throw new NotFoundException("Voice or bar not found");
            }

            foreach (var voice in song.Voices.ToList())
            {
                foreach (var bar in voice.SongBars.ToList())
                {
                    if (fromPosition <= bar.Position && bar.Position < deleteLength + fromPosition)
                    {
                        voice.SongBars.Remove(bar);
                    }
                    if (bar.Position >= deleteLength + fromPosition)
                    {
                        bar.Position -= deleteLength;
                    }
                }
            }
            
            if(firstVoiceLength == deleteLength)
            { 
                foreach(var voice in song.Voices)
                {
                    voice.SongBars.Add(new SongBar());
                }
            }

            SyncBarCountToMaxInAllVoices(song);
        }

        /// <summary>
        /// Removes a bar from a given position from all voices
        /// </summary>
        /// <param name="song"></param>
        /// <param name="position"></param>
        public static void RemoveSongBarFromAllVoices(this Song song, int position)
        {
            foreach (var songVoice in song.Voices)
            {
                var barToRemove = songVoice.SongBars.FirstOrDefault(sb => sb.Position == position) ?? songVoice.SongBars.LastOrDefault();

                songVoice.SongBars.Remove(barToRemove);
                songVoice.SortBars();
            }
        }

        /// <summary>
        /// Ensure all other voices in the song has the same setup as the masterVoice
        /// </summary>
        /// <param name="song"></param>
        /// <param name="masterVoice"></param>
        public static void SyncVoicesFrom(this Song song, SongVoice masterVoice)
        {
            var otherVoices = song.Voices.Where(v => v.Id != masterVoice.Id).ToArray();
            if (!otherVoices.Any())
            {
                return;
            }

            SyncBarCountToMaxInAllVoices(song);

            var barLength = masterVoice.SongBars.Count;
            for (var i = 0; i < barLength; i++)
            {
                foreach (var otherVoice in otherVoices)
                {
                    var masterBar = masterVoice.SongBars.ToArray()[i];
                    var slaveBar = otherVoice.SongBars.ToArray()[i];

                    slaveBar.House = masterBar.House;
                    slaveBar.RepAfter = masterBar.RepAfter;
                    slaveBar.RepBefore = masterBar.RepBefore;
                }
            }

        }

        public static Song Clone(this Song song, string title = null)
        {
            return new Song()
            {
                Title = title ?? song.Title,
                Denominator = song.Denominator,
                Numerator = song.Numerator,
                ArrangerId = song.ArrangerId,
                Speed = song.Speed,
                DegreeOfDifficulty = song.DegreeOfDifficulty,
                SongNotes = song.SongNotes,
                Voices = song.Voices.Select(v => v.Clone(v.VoiceName)).ToArray()
            };
        }

        public static Song Transpose(this Song song, int transpose = 0)
        {
            song.Voices = song.Voices.Select(v => v.Transpose(transpose)).ToArray();

            return song;
        }
    }
}
