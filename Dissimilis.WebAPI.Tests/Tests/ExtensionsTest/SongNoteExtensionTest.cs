
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
                        },
                        new SongNote()
                        {
                            Position = 1,
                            ChordName = null,
                            Length = 1,
                            NoteValues = "C",
                        },
                        new SongNote()
                        {
                            Position = 1,
                            ChordName = null,
                            Length = 1,
                            NoteValues = "A#",
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
                        },
                        new SongNote()
                        {
                            Position = 1,
                            ChordName = null,
                            Length = 1,
                            NoteValues = "C",
                        },
                        new SongNote()
                        {
                            Position = 1,
                            ChordName = null,
                            Length = 1,
                            NoteValues = "A#",
                        }
                };
            }
        }

        [Fact]
        public void TestGetNoteValuesFromChordName()
        {
            string[] chordNamesToTest = new string[]
            {
                "C",
                "Cmaj7",
                "D",
                "E",
                "F",
                "F#",
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
                new List<string> { "F#", "A#", "C#" },
                new List<string> { "D", "F", "A" },
                new List<string> { "F", "A", "C#", "D#" },
                new List<string> { "A#", "D#", "F" },
            };

            for (int index = 0; index < chordNamesToTest.Length; index++)
            {
                SongNoteExtension.GetNoteValuesFromChordName(chordNamesToTest[index]).ShouldBeEquivalentTo(expectedNoteValues[index], $"Note values were not correct for chord {chordNamesToTest[index]}");
            }
        }

        //[Fact]
        //public void TestGetAllChordOptions()
        //{
        //    SongNoteExtension.GetIntervalNames("C").ShouldBe(new string[] { "Root", "Third", "Fifth" }, $"Failed for chord C");
        //    SongNoteExtension.GetIntervalNames("Dmaj7").ShouldBe(new string[] { "Root", "Third", "Fifth", "Seventh" }, $"Failed for chord Dmaj7");
        //    SongNoteExtension.GetIntervalNames("Esus4").ShouldBe(new string[] { "Root", "Forth", "Fifth" }, $"Failed for chord Esus4");
        //    SongNoteExtension.GetIntervalNames("G6/9").ShouldBe(new string[] { "Root", "Third", "Fifth", "Sixth", "Ninth" }, $"Failed for chord G6/9");
        //    SongNoteExtension.GetIntervalNames("A#maj13").ShouldBe(new string[] { "Root", "Third", "Fifth", "Seventh", "Ninth", "Thirteenth" }, $"Failed for chord A#maj13");
        //    SongNoteExtension.GetIntervalNames("D#7#5").ShouldBe(new string[] { "Root", "Third", "Fifth", "Seventh" }, $"Failed for chord D#7#5");
        //    SongNoteExtension.GetIntervalNames("Hdim7").ShouldBe(new string[] { "Root", "Third", "Fifth", "Seventh" }, $"Failed for chord Hdim7");
        //}

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

        [Fact]
        public void TestTranspose()
        {
            List<SongNote> songNotesToTest = CreateSongNotes(true);
            List<int> transposeValues = new List<int>() { 1, 0, 3, -3, 5, -1, 2 };
            List<string> expectedChordNames = new List<string>() { "C#", "D", "D#maj7", "F#m", "Gsus4", null, null };
            List<List<string>> expectedNoteValues = new List<List<string>>()
            {
                SongNoteExtension.GetNoteValuesFromChordName("C#"),
                SongNoteExtension.GetNoteValuesFromChordName("D"),
                SongNoteExtension.GetNoteValuesFromChordName("D#maj7"),
                SongNoteExtension.GetNoteValuesFromChordName("F#m"),
                SongNoteExtension.GetNoteValuesFromChordName("Gsus4"),
                new List<string>() { "H" },
                new List<string>() { "C" },
            };

            for (int index = 0; index < songNotesToTest.Count; index++)
            {
                songNotesToTest[index].Transpose(transposeValues[index]);
                songNotesToTest[index].ChordName.ShouldBe(expectedChordNames[index]);
                List<string> actualNoteValues = new List<string>(songNotesToTest[index].GetNoteValues());
                actualNoteValues.ShouldBeEquivalentTo(expectedNoteValues[index]);
            }
        }
    }
}
