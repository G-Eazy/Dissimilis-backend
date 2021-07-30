using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Dissimilis.DbContext.Models.Song;
using Dissimilis.WebAPI.Controllers.BoBar.DtoModelsOut;
using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsOut;
using FoodLabellingAPI.Collections;
using Newtonsoft.Json.Linq;

namespace Dissimilis.WebAPI.Extensions.Models
{
    public static class SongBarExtension
    {
        /// <summary>
        /// Gets the stored bar notes and fills up with blank notes ready to send to frontend
        /// </summary>
        public static SongNote[] GetBarNotes(this SongBar songBar)
        {
            var finalNotes = new List<SongNote>();

            var currentPosition = 0;
            var maxBarPosition = songBar.GetMaxBarPosition();
            var maxBarLength = maxBarPosition + 1;
            var notesToCheck = songBar.Notes.OrderBy(n => n.Position).ToQueue();

            while (notesToCheck.Any())
            {
                // peak at note the see if it is supposed to be in current position
                var noteToCheck = notesToCheck.Peek();

                // if not fits current position, then add it
                if (noteToCheck.Position == currentPosition)
                {
                    finalNotes.Add(notesToCheck.Dequeue());
                    currentPosition = currentPosition + noteToCheck.Length;
                }
                // if note does not fits current position then fill with empty note up until the note's starting point
                else
                {
                    var lengthToFill = noteToCheck.Position - currentPosition;
                    finalNotes.Add(GetEmptyNote(currentPosition, lengthToFill));
                    currentPosition = currentPosition + lengthToFill;
                }
            }

            // Fill out the blanks with empty notes until the end of the bar
            if (currentPosition < maxBarLength)
            {
                finalNotes.Add(GetEmptyNote(currentPosition, maxBarLength - currentPosition));
            }

            return finalNotes.OrderBy(n => n.Position).ToArray();
        }

        public static SongNote GetEmptyNote(int position, int length)
        {
            return new SongNote()
            {
                Position = position,
                Length = length,
                NoteValues = "Z"
            };
        }

        public static int GetMaxBarPosition(this SongBar songBar)
        {
            return songBar.SongVoice.Song.GetMaxBarPosition();
        }

        public static bool CheckSongBarValidation(this SongBar songBar, bool throwValidationException = true)
        {
            var currentPosition = 0;
            var maxBarLength = songBar.GetMaxBarPosition() + 1;

            foreach (var note in songBar.Notes.OrderBy(n => n.Position))
            {
                if (note.Position < currentPosition)
                {
                    if (throwValidationException)
                    {
                        throw new ValidationException("A note seems be placed on top of another note");
                    }
                    return false;
                }

                if (note.Position + note.Length > maxBarLength)
                {
                    if (throwValidationException)
                    {
                        throw new ValidationException("A note seems to stretch over the max position of the SongBar");
                    }
                    return false;
                }

                currentPosition = note.Position + note.Length;
            }

            return true;
        }

        public static SongBar DuplicateAllChords(this SongBar songBar, SongBar sourceSongBar, bool includeComponentIntervals)
        {
            var updatedSongNotes = sourceSongBar.Notes
                .Select(srcNote => {
                    var originalNote = songBar.Notes
                            .Where(note =>
                            {
                                var (startPos, endPos) = note.GetNotePositionRange();
                                return startPos <= srcNote.Position && srcNote.Position <= endPos;
                            })
                            .SingleOrDefault();
                    //If the note is singular keep the original.
                    if (srcNote.ChordName == null)
                    {
                        if (originalNote != null)
                        {
                            originalNote.Length -= (srcNote.Position - originalNote.Position);
                            originalNote.Position = srcNote.Position;
                        }
                        return originalNote;
                    }
                    //If the original note already exists, do not override
                    else if (originalNote != null)
                    {
                        return originalNote;
                    }
                    else
                    {
                        return srcNote.Clone(includeComponentIntervals);
                    }
                })
                .Where(note => note != null)
                .ToList();
            songBar.Notes = updatedSongNotes;
            return songBar;
        }
        public static SongBar RemoveComponentInterval(this SongBar songBar, int intervalPosition)
        {
            songBar.Notes = songBar.Notes
                .Select(note => {
                    //If the note is a chord remove component interval.
                    if (note.ChordName != null)
                    {
                        var updatedNote = note.RemoveComponentInterval(intervalPosition);
                        return note.RemoveComponentInterval(intervalPosition);
                    }
                    return note;
                })
                .ToList();
            return songBar;
        }

        public static List<SongBar> GetSongBarsFromDto(BarDto[] barDtos, SongVoice voice)
        {
            List<SongBar> newBars = new List<SongBar>();
            foreach (var barDto in barDtos)
            {
                SongBar bar = voice.SongBars.SingleOrDefault(b => b.Position == barDto.Position);
                if (bar == null)
                    bar = BarDto.ConvertToSongBar(barDto, voice);

                bar.Notes = SongNoteExtension.GetSongNotesFromDto(barDto.Chords, bar);
                newBars.Add(bar);
            }
            return newBars;
        }

        public static SongBar AddComponentInterval(this SongBar songBar, int intervalPosition)
        {
            songBar.Notes = songBar.Notes
                .Select(note => {
                    //If the note is a chord add component interval.
                    if (note.ChordName != null)
                    {
                        return note.AddComponentInterval(intervalPosition);
                    }
                    return note;
                })
                .ToList();
            return songBar;
        }

        public static SongBar RemoveEmptyChords(this SongBar songBar)
        {
            songBar.Notes = songBar.Notes
                .Select(note =>
                {
                    if (note.NoteValues.Split("|").All(noteValue => noteValue == "X"))
                        return null;
                    return note;
                })
                .Where(note => note != null)
                .ToList();

            return songBar;
        }

        public static SongBar Transpose(this SongBar songBar, int transpose = 0)
        {
            songBar.Notes = songBar.Notes.Select(note => note.Transpose(transpose)).ToList();
            return songBar;
        }

        public static HashSet<string> GetAllIntervalNames(this SongBar songBar)
        {
            return songBar.Notes
                .Where(note => note.ChordName != null)
                .SelectMany(note => SongNoteExtension.GetIntervalNames(note.ChordName))
                .ToHashSet();
        }
    }
}
