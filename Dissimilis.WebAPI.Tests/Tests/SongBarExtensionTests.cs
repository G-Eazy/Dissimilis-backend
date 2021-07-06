using System.Collections.Generic;
using System.Threading.Tasks;
using Shouldly;
using Xunit;
using Dissimilis.WebAPI.Controllers.BoSong;
using Dissimilis.WebAPI.Controllers.BoSong.Commands;
using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsIn;
using Dissimilis.WebAPI.Controllers.BoVoice;
using Dissimilis.WebAPI.Extensions.Models;
using System;
using Dissimilis.WebAPI.xUnit.Setup;
using MediatR;
using static Dissimilis.WebAPI.xUnit.Extensions;
using System.Linq;
using Dissimilis.DbContext.Models.Song;

namespace Dissimilis.WebAPI.xUnit.Tests
{
    [Collection("Serial")]
    [CollectionDefinition("Serial", DisableParallelization = true)]
    public class SongBarExtensionTests
    {

        public SongBar CreateBarWithNotes(bool includeComponentIntervals)
        {
            if (includeComponentIntervals)
            {
                return new SongBar()
                {
                    Notes = new List<SongNote>()
                    {
                        new SongNote()
                        {
                            Position = 0,
                            Length = 1,
                            ChordName = "A#",
                            NoteValues = String.Join("|", SongNoteExtension.GetNoteValuesFromChordName("A#"))
                        },
                        new SongNote()
                        {
                            Position = 1,
                            Length = 1,
                            ChordName = "Cmaj7",
                            NoteValues = String.Join("|", SongNoteExtension.GetNoteValuesFromChordName("Cmaj7"))
                        },
                        new SongNote()
                        {
                            Position = 2,
                            Length = 1,
                            ChordName = "Gm9",
                            NoteValues = String.Join("|", SongNoteExtension.GetNoteValuesFromChordName("Gm9"))
                        },
                        new SongNote()
                        {
                            Position = 3,
                            Length = 1,
                            ChordName = null,
                            NoteValues = "D#"
                        }
                    }
                };
            }
            else
            {
                return new SongBar()
                {
                    Notes = new List<SongNote>()
                    {
                        new SongNote()
                        {
                            Position = 0,
                            Length = 1,
                            ChordName = "A#",
                            NoteValues = "X|X|X"
                        },
                        new SongNote()
                        {
                            Position = 1,
                            Length = 1,
                            ChordName = "Cmaj7",
                            NoteValues = "X|X|X|X"
                        },
                        new SongNote()
                        {
                            Position = 2,
                            Length = 1,
                            ChordName = "Gm9",
                            NoteValues = "X|X|X|X|X"
                        },
                        new SongNote()
                        {
                            Position = 3,
                            Length = 1,
                            ChordName = null,
                            NoteValues = "D#"
                        }
                    }
                };
            }
        }

        [Fact]
        public void DuplicateAllChordsTest()
        {
            //Create and populate a source bar with chords and single notes.
            var sourceBar = CreateBarWithNotes(true);

            //Create and populate a destination bar with single notes.
            var originalNotes = new List<SongNote>()
            {
                new SongNote()
                {
                    Position = 3,
                    Length = 1,
                    ChordName = null,
                    NoteValues = "C"
                }
            };

            var destinationBar = new SongBar()
            {
                Notes = originalNotes
            };

            destinationBar.DuplicateAllChords(sourceBar, true);

            foreach (var note in destinationBar.Notes)
            {
                var expectedNote = sourceBar.Notes.First(srcNote => srcNote.Position == note.Position);
                //If the note is not a chord, check that it has not been altered.
                if (expectedNote.ChordName == null)
                {
                    expectedNote = originalNotes.First(originalNote => originalNote.Position == note.Position);
                }
                note.ChordName.ShouldBe(expectedNote.ChordName, $"The chordname for note at position {note.Position} was incorrect.");
                note.NoteValues.ShouldBeEquivalentTo(expectedNote.NoteValues, $"The note values for note at postion {note.Position} was incorrect.");
            }
        }

        [Fact]
        public void RemoveComponentIntervalTest()
        {
            var barToTest = CreateBarWithNotes(true);

            var expectedSongNotesAfterRootRemoval = new List<SongNote>()
            {
                new SongNote()
                        {
                            Position = 0,
                            Length = 1,
                            ChordName = "A#",
                            NoteValues = "X|D|F"
                        },
                        new SongNote()
                        {
                            Position = 1,
                            Length = 1,
                            ChordName = "Cmaj7",
                            NoteValues = "X|E|G|H"
                        },
                        new SongNote()
                        {
                            Position = 2,
                            Length = 1,
                            ChordName = "Gm9",
                            NoteValues = "X|A#|D|F|A"
                        },
                        new SongNote()
                        {
                            Position = 3,
                            Length = 1,
                            ChordName = null,
                            NoteValues = "D#"
                        }
            };
            var expectedSongNotesAfterNinthRemoval = new List<SongNote>()
            {
                new SongNote()
                        {
                            Position = 0,
                            Length = 1,
                            ChordName = "A#",
                            NoteValues = "A#|D|F"
                        },
                        new SongNote()
                        {
                            Position = 1,
                            Length = 1,
                            ChordName = "Cmaj7",
                            NoteValues = "C|E|G|H"
                        },
                        new SongNote()
                        {
                            Position = 2,
                            Length = 1,
                            ChordName = "Gm9",
                            NoteValues = "G|A#|D|F|X"
                        },
                        new SongNote()
                        {
                            Position = 3,
                            Length = 1,
                            ChordName = null,
                            NoteValues = "D#"
                        }
            };

            var expectedSongNotesAfterThirdSeventhRemoval = new List<SongNote>()
            {
                new SongNote()
                        {
                            Position = 0,
                            Length = 1,
                            ChordName = "A#",
                            NoteValues = "A#|X|F"
                        },
                        new SongNote()
                        {
                            Position = 1,
                            Length = 1,
                            ChordName = "Cmaj7",
                            NoteValues = "C|X|G|X"
                        },
                        new SongNote()
                        {
                            Position = 2,
                            Length = 1,
                            ChordName = "Gm9",
                            NoteValues = "G|X|D|X|A"
                        },
                        new SongNote()
                        {
                            Position = 3,
                            Length = 1,
                            ChordName = null,
                            NoteValues = "D#"
                        }
            };

            var expectedSongNotesAfterRootFifthRemoval = new List<SongNote>()
            {
                new SongNote()
                        {
                            Position = 0,
                            Length = 1,
                            ChordName = "A#",
                            NoteValues = "X|D|X"
                        },
                        new SongNote()
                        {
                            Position = 1,
                            Length = 1,
                            ChordName = "Cmaj7",
                            NoteValues = "X|E|X|H"
                        },
                        new SongNote()
                        {
                            Position = 2,
                            Length = 1,
                            ChordName = "Gm9",
                            NoteValues = "X|A#|X|F|A"
                        },
                        new SongNote()
                        {
                            Position = 3,
                            Length = 1,
                            ChordName = null,
                            NoteValues = "D#"
                        }
            };

            //Test removing only root tone.
            barToTest.RemoveComponentInterval(0);

            foreach (var note in barToTest.Notes)
            {
                var expectedNote = expectedSongNotesAfterRootRemoval.First(expectedNote => expectedNote.Position == note.Position);
                note.ChordName.ShouldBe(expectedNote.ChordName, $"The chordname for note at position {note.Position} was incorrect.");
                note.NoteValues.ShouldBeEquivalentTo(expectedNote.NoteValues, $"The note values for note at postion {note.Position} was incorrect.");
            }

            //Reset for further testing.
            barToTest = CreateBarWithNotes(true);

            //Test removing only ninth tone.
            barToTest.RemoveComponentInterval(4);

            foreach (var note in barToTest.Notes)
            {
                var expectedNote = expectedSongNotesAfterNinthRemoval.First(expectedNote => expectedNote.Position == note.Position);
                note.ChordName.ShouldBe(expectedNote.ChordName, $"The chordname for note at position {note.Position} was incorrect.");
                note.NoteValues.ShouldBeEquivalentTo(expectedNote.NoteValues, $"The note values for note at postion {note.Position} was incorrect.");
            }

            //Reset for further testing.
            barToTest = CreateBarWithNotes(true);

            //Test removing third and seventh tone.
            barToTest.RemoveComponentInterval(1);
            barToTest.RemoveComponentInterval(3);

            foreach (var note in barToTest.Notes)
            {
                var expectedNote = expectedSongNotesAfterThirdSeventhRemoval.First(expectedNote => expectedNote.Position == note.Position);
                note.ChordName.ShouldBe(expectedNote.ChordName, $"The chordname for note at position {note.Position} was incorrect.");
                note.NoteValues.ShouldBeEquivalentTo(expectedNote.NoteValues, $"The note values for note at postion {note.Position} was incorrect.");
            }

            //Reset for further testing.
            barToTest = CreateBarWithNotes(true);

            //Test removing root and fifth tone.
            barToTest.RemoveComponentInterval(0);
            barToTest.RemoveComponentInterval(2);

            foreach (var note in barToTest.Notes)
            {
                var expectedNote = expectedSongNotesAfterRootFifthRemoval.First(expectedNote => expectedNote.Position == note.Position);
                note.ChordName.ShouldBe(expectedNote.ChordName, $"The chordname for note at position {note.Position} was incorrect.");
                note.NoteValues.ShouldBeEquivalentTo(expectedNote.NoteValues, $"The note values for note at postion {note.Position} was incorrect.");
            }


        }

        [Fact]
        public void AddComponentIntervalTest()
        {
            var barToTest = CreateBarWithNotes(false);

            var expectedSongNotesAfterRootAddition = new List<SongNote>()
            {
                new SongNote()
                        {
                            Position = 0,
                            Length = 1,
                            ChordName = "A#",
                            NoteValues = "A#|X|X"
                        },
                        new SongNote()
                        {
                            Position = 1,
                            Length = 1,
                            ChordName = "Cmaj7",
                            NoteValues = "C|X|X|X"
                        },
                        new SongNote()
                        {
                            Position = 2,
                            Length = 1,
                            ChordName = "Gm9",
                            NoteValues = "G|X|X|X|X"
                        },
                        new SongNote()
                        {
                            Position = 3,
                            Length = 1,
                            ChordName = null,
                            NoteValues = "D#"
                        }
            };
            var expectedSongNotesAfterNinthAddition = new List<SongNote>()
            {
                new SongNote()
                        {
                            Position = 0,
                            Length = 1,
                            ChordName = "A#",
                            NoteValues = "X|X|X"
                        },
                        new SongNote()
                        {
                            Position = 1,
                            Length = 1,
                            ChordName = "Cmaj7",
                            NoteValues = "X|X|X|X"
                        },
                        new SongNote()
                        {
                            Position = 2,
                            Length = 1,
                            ChordName = "Gm9",
                            NoteValues = "X|X|X|X|A"
                        },
                        new SongNote()
                        {
                            Position = 3,
                            Length = 1,
                            ChordName = null,
                            NoteValues = "D#"
                        }
            };

            var expectedSongNotesAfterThirdSeventhAddition = new List<SongNote>()
            {
                new SongNote()
                        {
                            Position = 0,
                            Length = 1,
                            ChordName = "A#",
                            NoteValues = "X|D|X"
                        },
                        new SongNote()
                        {
                            Position = 1,
                            Length = 1,
                            ChordName = "Cmaj7",
                            NoteValues = "X|E|X|H"
                        },
                        new SongNote()
                        {
                            Position = 2,
                            Length = 1,
                            ChordName = "Gm9",
                            NoteValues = "X|A#|X|F|X"
                        },
                        new SongNote()
                        {
                            Position = 3,
                            Length = 1,
                            ChordName = null,
                            NoteValues = "D#"
                        }
            };

            var expectedSongNotesAfterRootFifthAddition = new List<SongNote>()
            {
                new SongNote()
                        {
                            Position = 0,
                            Length = 1,
                            ChordName = "A#",
                            NoteValues = "A#|X|F"
                        },
                        new SongNote()
                        {
                            Position = 1,
                            Length = 1,
                            ChordName = "Cmaj7",
                            NoteValues = "C|X|G|X"
                        },
                        new SongNote()
                        {
                            Position = 2,
                            Length = 1,
                            ChordName = "Gm9",
                            NoteValues = "G|X|D|X|X"
                        },
                        new SongNote()
                        {
                            Position = 3,
                            Length = 1,
                            ChordName = null,
                            NoteValues = "D#"
                        }
            };

            //Test adding only root tone.
            barToTest.AddComponentInterval(0);

            foreach (var note in barToTest.Notes)
            {
                var expectedNote = expectedSongNotesAfterRootAddition.First(expectedNote => expectedNote.Position == note.Position);
                note.ChordName.ShouldBe(expectedNote.ChordName, $"The chordname for note at position {note.Position} was incorrect.");
                note.NoteValues.ShouldBeEquivalentTo(expectedNote.NoteValues, $"The note values for note at postion {note.Position} was incorrect.");
            }

            //Reset for further testing.
            barToTest = CreateBarWithNotes(false);

            //Test adding only ninth tone.
            barToTest.AddComponentInterval(4);

            foreach (var note in barToTest.Notes)
            {
                var expectedNote = expectedSongNotesAfterNinthAddition.First(expectedNote => expectedNote.Position == note.Position);
                note.ChordName.ShouldBe(expectedNote.ChordName, $"The chordname for note at position {note.Position} was incorrect.");
                note.NoteValues.ShouldBeEquivalentTo(expectedNote.NoteValues, $"The note values for note at postion {note.Position} was incorrect.");
            }

            //Reset for further testing.
            barToTest = CreateBarWithNotes(false);

            //Test adding third and seventh tone.
            barToTest.AddComponentInterval(1);
            barToTest.AddComponentInterval(3);

            foreach (var note in barToTest.Notes)
            {
                var expectedNote = expectedSongNotesAfterThirdSeventhAddition.First(expectedNote => expectedNote.Position == note.Position);
                note.ChordName.ShouldBe(expectedNote.ChordName, $"The chordname for note at position {note.Position} was incorrect.");
                note.NoteValues.ShouldBeEquivalentTo(expectedNote.NoteValues, $"The note values for note at postion {note.Position} was incorrect.");
            }

            //Reset for further testing.
            barToTest = CreateBarWithNotes(false);
                
            //Test adding root and fifth tone.
            barToTest.AddComponentInterval(0);
            barToTest.AddComponentInterval(2);

            foreach (var note in barToTest.Notes)
            {
                var expectedNote = expectedSongNotesAfterRootFifthAddition.First(expectedNote => expectedNote.Position == note.Position);
                note.ChordName.ShouldBe(expectedNote.ChordName, $"The chordname for note at position {note.Position} was incorrect.");
                note.NoteValues.ShouldBeEquivalentTo(expectedNote.NoteValues, $"The note values for note at postion {note.Position} was incorrect.");
            }
        }

        [Fact]
        public void TransposeTest()
        {
            var barToTest = CreateBarWithNotes(true);
            var expectedChordNamesMinusFour = new List<string>() { "F#", "G#maj7", "D#m9", null };
            var expectedChordNamesPlusTwo = new List<string>() { "C", "Dmaj7", "Am9", null };
            var expectedChordNamesMinusFifteen = new List<string>() { "G", "Amaj7", "Em9", null };
            var expectedChordNamesPlusSeventeen = new List<string>() { "D#", "Fmaj7", "Cm9", null };

            var expectedNoteValuesMinusFour = new List<string>() {
                String.Join("|", SongNoteExtension.GetNoteValuesFromChordName("F#")),
                String.Join("|", SongNoteExtension.GetNoteValuesFromChordName("G#maj7")),
                String.Join("|", SongNoteExtension.GetNoteValuesFromChordName("D#m9")),
                "H" };
            var expectedNoteValuesPlusTwo = new List<string>() {
                String.Join("|", SongNoteExtension.GetNoteValuesFromChordName("C")),
                String.Join("|", SongNoteExtension.GetNoteValuesFromChordName("Dmaj7")),
                String.Join("|", SongNoteExtension.GetNoteValuesFromChordName("Am9")),
                "F" };
            var expectedNoteValuesMinusFifteen = new List<string>() {
                String.Join("|", SongNoteExtension.GetNoteValuesFromChordName("G")),
                String.Join("|", SongNoteExtension.GetNoteValuesFromChordName("Amaj7")),
                String.Join("|", SongNoteExtension.GetNoteValuesFromChordName("Em9")),
                "C" };
            var expectedNoteValuesPlusSeventeen = new List<string>() {
                String.Join("|", SongNoteExtension.GetNoteValuesFromChordName("D#")),
                String.Join("|", SongNoteExtension.GetNoteValuesFromChordName("Fmaj7")),
                String.Join("|", SongNoteExtension.GetNoteValuesFromChordName("Cm9")),
                "G#" };

            //Test tranpose -4
            barToTest.Transpose(-4);
            foreach (var note in barToTest.Notes)
            {
                note.ChordName.ShouldBe(expectedChordNamesMinusFour[note.Position], $"The chordname for note at position {note.Position} was incorrect.");
                note.NoteValues.ShouldBe(expectedNoteValuesMinusFour[note.Position], $"The note values for note at position {note.Position} was incorrect.");
            }

            //Reset for further testing
            barToTest = CreateBarWithNotes(true);

            //Test tranpose +2
            barToTest.Transpose(+2);
            foreach (var note in barToTest.Notes)
            {
                note.ChordName.ShouldBe(expectedChordNamesPlusTwo[note.Position], $"The chordname for note at position {note.Position} was incorrect.");
                note.NoteValues.ShouldBe(expectedNoteValuesPlusTwo[note.Position], $"The note values for note at position {note.Position} was incorrect.");
            }

            //Reset for further testing
            barToTest = CreateBarWithNotes(true);

            //Test tranpose -15
            barToTest.Transpose(-15);
            foreach (var note in barToTest.Notes)
            {
                note.ChordName.ShouldBe(expectedChordNamesMinusFifteen[note.Position], $"The chordname for note at position {note.Position} was incorrect.");
                note.NoteValues.ShouldBe(expectedNoteValuesMinusFifteen[note.Position], $"The note values for note at position {note.Position} was incorrect.");
            }

            //Reset for further testing
            barToTest = CreateBarWithNotes(true);

            //Test tranpose +17
            barToTest.Transpose(+17);
            foreach (var note in barToTest.Notes)
            {
                note.ChordName.ShouldBe(expectedChordNamesPlusSeventeen[note.Position], $"The chordname for note at position {note.Position} was incorrect.");
                note.NoteValues.ShouldBe(expectedNoteValuesPlusSeventeen[note.Position], $"The note values for note at position {note.Position} was incorrect.");
            }
        }
    }
}
