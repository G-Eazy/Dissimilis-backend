using System.Collections.Generic;
using System.Threading.Tasks;
using Shouldly;
using Xunit;
using Dissimilis.WebAPI.Extensions.Models;
using Dissimilis.DbContext.Models.Song;
using System;

namespace Dissimilis.WebAPI.xUnit.Tests
{
    [Collection("Serial")]
    [CollectionDefinition("Serial", DisableParallelization = true)]
    public class SongNoteExtensionTest
    {

        public List<SongNote> CreateSongNotes(bool hasRemovedIntervals = false)
        {
            if (hasRemovedIntervals)
            {
                return new List<SongNote>()
                    {
                        new SongNote()
                        {
                            Position = 1,
                            ChordName = "C",
                            Length = 1,
                            NoteValues = "C|X|G",
                        },
                        new SongNote()
                        {
                            Position = 1,
                            ChordName = "D",
                            Length = 1,
                            NoteValues = "X|F#|A",
                        },
                        new SongNote()
                        {
                            Position = 1,
                            ChordName = "Cmaj7",
                            Length = 1,
                            NoteValues = "C|E|G|X",
                        },
                        new SongNote()
                        {
                            Position = 1,
                            ChordName = "Am",
                            Length = 1,
                            NoteValues = "X|C|X",
                        },
                        new SongNote()
                        {
                            Position = 1,
                            ChordName = "Dsus4",
                            Length = 1,
                            NoteValues = "X|X|X",
                        }
            };
            }
            else
            {
                return new List<SongNote>()
                    {
                        new SongNote()
                        {
                            Position = 1,
                            ChordName = "C",
                            Length = 1,
                            NoteValues = String.Join("|", SongNoteExtension.GetNoteValuesFromChordName("C")),
                        },
                        new SongNote()
                        {
                            Position = 1,
                            ChordName = "D",
                            Length = 1,
                            NoteValues = String.Join("|", SongNoteExtension.GetNoteValuesFromChordName("D")),
                        },
                        new SongNote()
                        {
                            Position = 1,
                            ChordName = "Cmaj7",
                            Length = 1,
                            NoteValues = String.Join("|", SongNoteExtension.GetNoteValuesFromChordName("Cmaj7")),
                        },
                        new SongNote()
                        {
                            Position = 1,
                            ChordName = "Am",
                            Length = 1,
                            NoteValues = String.Join("|", SongNoteExtension.GetNoteValuesFromChordName("Am")),
                        },
                        new SongNote()
                        {
                            Position = 1,
                            ChordName = "Dsus4",
                            Length = 1,
                            NoteValues = String.Join("|", SongNoteExtension.GetNoteValuesFromChordName("Dsus4")),
                    }
                };
            }
        }

        [Fact]
        public void TestGetNoteValues()
        {
            string[] chordNamesToTest = new string[]
            {
                "C",
                "Cmaj7",
                "D",
                "E",
                "F",
                "Dm",
                "Faug7",
                "A#sus4",
            };
            List<List<string>> expectedNoteValues = new List<List<string>>()
            {
                new List<string> { "C", "E", "G" },
                new List<string> { "C", "E", "G", "H" },
                new List<string> { "D", "F#", "A" },
                new List<string> { "E", "G#", "H" },
                new List<string> { "F", "A", "C" },
                new List<string> { "D", "F", "A" },
                new List<string> { "F", "A", "C#", "D#" },
                new List<string> { "A#", "D#", "F" },
            };

            for (int index = 0; index < chordNamesToTest.Length; index++)
            {
                SongNoteExtension.GetNoteValuesFromChordName(chordNamesToTest[index]).ShouldBeEquivalentTo(expectedNoteValues[index], $"Note values were not correct for chord {chordNamesToTest[index]}");
            }
        }

        [Fact]
        public void TestRemoveComponentInterval()
        {
            List<SongNote> songNotesToTest = CreateSongNotes();
            List<SongNote> expectedSongNotes = CreateSongNotes(true);

            songNotesToTest[0].RemoveComponentInterval(1);
            songNotesToTest[1].RemoveComponentInterval(0);
            songNotesToTest[2].RemoveComponentInterval(3);
            songNotesToTest[3].RemoveComponentInterval(0);
            songNotesToTest[3].RemoveComponentInterval(2);
            songNotesToTest[4].RemoveComponentInterval(0);
            songNotesToTest[4].RemoveComponentInterval(1);
            songNotesToTest[4].RemoveComponentInterval(2);

            for (int index = 0; index < songNotesToTest.Count; index++)
            {
                songNotesToTest[index].ShouldBeEquivalentTo(expectedSongNotes[index], $"Songnote number {index} failed.");
            }
        }

        [Fact]
        public void TestAddComponentInterval()
        {
            List<SongNote> songNotesToTest = CreateSongNotes(true);
            List<SongNote> expectedSongNotes = CreateSongNotes();

            songNotesToTest[0].AddComponentInterval(1);
            songNotesToTest[1].AddComponentInterval(0);
            songNotesToTest[2].AddComponentInterval(3);
            songNotesToTest[3].AddComponentInterval(0);
            songNotesToTest[3].AddComponentInterval(2);
            songNotesToTest[4].AddComponentInterval(0);
            songNotesToTest[4].AddComponentInterval(1);
            songNotesToTest[4].AddComponentInterval(2);

            for (int index = 0; index < songNotesToTest.Count; index++)
            {
                songNotesToTest[index].ShouldBeEquivalentTo(expectedSongNotes[index], $"Songnote number {index} failed.");
            }
        }
    }
}
