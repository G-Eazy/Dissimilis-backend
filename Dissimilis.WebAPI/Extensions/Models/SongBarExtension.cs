using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Dissimilis.DbContext.Models.Song;
using FoodLabellingAPI.Collections;

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

        public static SongBar CopyComponentInterval(this SongBar songBar, SongBar sourceSongBar, int intervalPosition)
        {
            //Adds all notes from the source bar which belongs to the specified interval position.
            var updatedBarNotes = new List<SongNote>();
            foreach (var note in sourceSongBar.Notes)
            {
                var newNote = new SongNote()
                {
                    BarId = songBar.Id,
                    SongBar = songBar,
                    Length = note.Length,
                    Position = note.Position,
                };
                newNote.SetNoteValues(new string[] { note.GetNoteValues()[intervalPosition] });
                //newNote.SetNoteValues(new string[] { "C" });
                updatedBarNotes.Add(newNote);
            }
            songBar.Notes = updatedBarNotes;
            //var songNotes = new List<SongNote>();
            //for (int i = 0; i < 4; i++)
            //{
            //    var newSongNote = new SongNote()
            //    {
            //        BarId = songBar.Id,
            //        SongBar = songBar,
            //        Position = i,
            //        ChordName = "C",
            //        Length = 1,
            //    };
            //    newSongNote.SetNoteValues(new string[] { "C", "E", "G" });
            //    songNotes.Add(newSongNote);
            //}
            //songBar.Notes = songNotes;
            return songBar;
        }

        public static SongBar Transpose(this SongBar songBar, int transpose = 0)
        {
            songBar.Notes = songBar.Notes.Select(n => n.TransposeNoteValues(transpose)).ToList();

            return songBar;
        }
    }
}
