using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System;
using Dissimilis.Core.Collections;
using Dissimilis.DbContext.Models.Song;
using Dissimilis.WebAPI.Controllers.BoNote.DtoModelsOut;

namespace Dissimilis.WebAPI.Extensions.Models
{
    public static class SongNoteExtension
    {
        private static List<string> _allNotes = new List<string>
        {
            "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "H"
        };

        //Source: https://en.wikipedia.org/wiki/Interval_(music)
        //Provides a mapping from interval types to how many semitones used to calculate next note value.
        //If a chord contains the intervals perfect unison (1P), minor third (3m), and perfect fifth (5P)
        //The intervals would be 0, 3, and 7 semitones respectivily. For further explaination, please refer to
        //the provided source article from Wikipedia linked above.
        public static Dictionary<string, int> IntervalMapping = new Dictionary<string, int>()
        {
            //==Main intervals==
            //'''Minor, major, perfect'''
            ["1P"] = 0,
            ["2m"] = 1,
            ["2M"] = 2,
            ["3m"] = 3,
            ["3M"] = 4,
            ["4P"] = 5,
            ["5P"] = 7,
            ["6m"] = 8,
            ["6M"] = 9,
            ["7m"] = 10,
            ["7M"] = 11,
            ["8P"] = 12,
            //'''Augmented, diminished'''
            ["2d"] = 0,
            ["1A"] = 1,
            ["3d"] = 2,
            ["2A"] = 3,
            ["4d"] = 4,
            ["3A"] = 5,
            ["5d"] = 6,
            ["4A"] = 6,
            ["6d"] = 7,
            ["5A"] = 8,
            ["7d"] = 9,
            ["6A"] = 10,
            ["8d"] = 11,
            ["7A"] = 12,

            //==Compound intervals==
            //'''Minor, major, perfect'''
            ["9m"] = 13,
            ["9M"] = 14,
            ["10m"] = 15,
            ["10M"] = 16,
            ["11P"] = 17,
            ["12P"] = 19,
            ["13m"] = 20,
            ["13M"] = 21,
            ["14m"] = 22,
            ["14M"] = 23,
            ["15P"] = 24,

            //'''Augmented, diminished'''
            ["9d"] = 12,
            ["8A"] = 13,
            ["10d"] = 14,
            ["9A"] = 15,
            ["11d"] = 16,
            ["10A"] = 17,
            ["12d"] = 18,
            ["11A"] = 18,
            ["13d"] = 19,
            ["12A"] = 20,
            ["14d"] = 21,
            ["13A"] = 22,
            ["15d"] = 23,
            ["14A"] = 24,
            ["15A"] = 25,
        };

        //Source: https://en.wikibooks.org/wiki/Music_Theory/Complete_List_of_Chord_Patterns
        //Provides a list of all chordpatterns. Each pattern can be identified by its name or one of its abreviations.
        //Each element in the list is an array on the format [pattern, name, abbreviations].
        //The pattern can be used together with the interval mapper to construct a chord based on its root tone and
        //interval values. For further explanation please see Wikibooks source article provided as a link above.
        private static List<string[]> ChordFormulas = new List<string[]> {
            new string[] {"1P 3M 5P", "major", "M ^ "},
            new string[] {"1P 3M 5P 7M", "major seventh", "maj7 Δ ma7 M7 Maj7 ^7"},
            new string[] {"1P 3M 5P 7M 9M", "major ninth", "maj9 Δ9 ^9"},
            new string[] {"1P 3M 5P 7M 9M 13M", "major thirteenth", "maj13 Maj13 ^13"},
            new string[] {"1P 3M 5P 6M", "sixth", "6 add6 add13 M6"},
            new string[] {"1P 3M 5P 6M 9M", "sixth/ninth", "6/9 69 M69"},
            new string[] {"1P 3M 6m 7M", "major seventh flat sixth", "M7b6 ^7b6"},
            new string[] {"1P 3M 5P 7M 11A", "major seventh sharp eleventh", "maj#4 Δ#4 Δ#11 M7#11 ^7#11 maj7#11"},
            // ==Minor==
            // '''Normal'''
            new string[] {"1P 3m 5P", "minor", "m min -"},
            new string[] {"1P 3m 5P 7m", "minor seventh", "m7 min7 mi7 -7"},
            new string[] {
                "1P 3m 5P 7M",
                "minor/major seventh",
                "m/ma7 m/maj7 mM7 mMaj7 m/M7 -Δ7 mΔ -^7",
            },
            new string[] {"1P 3m 5P 6M", "minor sixth", "m6 -6"},
            new string[] {"1P 3m 5P 7m 9M", "minor ninth", "m9 -9"},
            new string[] {"1P 3m 5P 7M 9M", "minor/major ninth", "mM9 mMaj9 -^9"},
            new string[] {"1P 3m 5P 7m 9M 11P", "minor eleventh", "m11 -11"},
            new string[] {"1P 3m 5P 7m 9M 13M", "minor thirteenth", "m13 -13"},
            // '''Diminished'''
            new string[] {"1P 3m 5d", "diminished", "dim ° o"},
            new string[] {"1P 3m 5d 7d", "diminished seventh", "dim7 °7 o7"},
            new string[] {"1P 3m 5d 7m", "half-diminished", "m7b5 ø -7b5 h7 h"},
            // ==Dominant/Seventh==
            // '''Normal'''
            new string[] {"1P 3M 5P 7m", "dominant seventh", "7 dom"},
            new string[] {"1P 3M 5P 7m 9M", "dominant ninth", "9"},
            new string[] {"1P 3M 5P 7m 9M 13M", "dominant thirteenth", "13"},
            new string[] {"1P 3M 5P 7m 11A", "lydian dominant seventh", "7#11 7#4"},
            // '''Altered'''
            new string[] {"1P 3M 5P 7m 9m", "dominant flat ninth", "7b9"},
            new string[] {"1P 3M 5P 7m 9A", "dominant sharp ninth", "7#9"},
            new string[] {"1P 3M 7m 9m", "altered", "alt7"},
            // '''Suspended'''
            new string[] {"1P 4P 5P", "suspended fourth", "sus4 sus"},
            new string[] {"1P 2M 5P", "suspended second", "sus2"},
            new string[] {"1P 4P 5P 7m", "suspended fourth seventh", "7sus4 7sus"},
            new string[] {"1P 5P 7m 9M 11P", "eleventh", "11"},
            new string[] {
                "1P 4P 5P 7m 9m",
                "suspended fourth flat ninth",
                "b9sus phryg 7b9sus 7b9sus4",
            },
            // ==Other==
            new string[] {"1P 5P", "fifth", "5"},
            new string[] {"1P 3M 5A", "augmented", "aug + +5 ^#5"},
            new string[] {"1P 3m 5A", "minor augmented", "m#5 -#5 m+"},
            new string[] {"1P 3M 5A 7M", "augmented seventh", "maj7#5 maj7+5 +maj7 ^7#5"},
            new string[] {
                "1P 3M 5P 7M 9M 11A",
                "major sharp eleventh (lydian)",
                "maj9#11 Δ9#11 ^9#11",
            },
            // ==Legacy==
            new string[] {"1P 2M 4P 5P", "", "sus24 sus4add9"},
            new string[] {"1P 3M 5A 7M 9M", "", "maj9#5 Maj9#5"},
            new string[] {"1P 3M 5A 7m", "", "7#5 +7 7+ 7aug aug7"},
            new string[] {"1P 3M 5A 7m 9A", "", "7#5#9 7#9#5 7alt"},
            new string[] {"1P 3M 5A 7m 9M", "", "9#5 9+"},
            new string[] {"1P 3M 5A 7m 9M 11A", "", "9#5#11"},
            new string[] {"1P 3M 5A 7m 9m", "", "7#5b9 7b9#5"},
            new string[] {"1P 3M 5A 7m 9m 11A", "", "7#5b9#11"},
            new string[] {"1P 3M 5A 9A", "", "+add#9"},
            new string[] {"1P 3M 5A 9M", "", "M#5add9 +add9"},
            new string[] {"1P 3M 5P 6M 11A", "", "M6#11 M6b5 6#11 6b5"},
            new string[] {"1P 3M 5P 6M 7M 9M", "", "M7add13"},
            new string[] {"1P 3M 5P 6M 9M 11A", "", "69#11"},
            new string[] {"1P 3m 5P 6M 9M", "", "m69 -69"},
            new string[] {"1P 3M 5P 6m 7m", "", "7b6"},
            new string[] {"1P 3M 5P 7M 9A 11A", "", "maj7#9#11"},
            new string[] {"1P 3M 5P 7M 9M 11A 13M", "", "M13#11 maj13#11 M13+4 M13#4"},
            new string[] {"1P 3M 5P 7M 9m", "", "M7b9"},
            new string[] {"1P 3M 5P 7m 11A 13m", "", "7#11b13 7b5b13"},
            new string[] {"1P 3M 5P 7m 13M", "", "7add6 67 7add13"},
            new string[] {"1P 3M 5P 7m 9A 11A", "", "7#9#11 7b5#9 7#9b5"},
            new string[] {"1P 3M 5P 7m 9A 11A 13M", "", "13#9#11"},
            new string[] {"1P 3M 5P 7m 9A 11A 13m", "", "7#9#11b13"},
            new string[] {"1P 3M 5P 7m 9A 13M", "", "13#9"},
            new string[] {"1P 3M 5P 7m 9A 13m", "", "7#9b13"},
            new string[] {"1P 3M 5P 7m 9M 11A", "", "9#11 9+4 9#4"},
            new string[] {"1P 3M 5P 7m 9M 11A 13M", "", "13#11 13+4 13#4"},
            new string[] {"1P 3M 5P 7m 9M 11A 13m", "", "9#11b13 9b5b13"},
            new string[] {"1P 3M 5P 7m 9m 11A", "", "7b9#11 7b5b9 7b9b5"},
            new string[] {"1P 3M 5P 7m 9m 11A 13M", "", "13b9#11"},
            new string[] {"1P 3M 5P 7m 9m 11A 13m", "", "7b9b13#11 7b9#11b13 7b5b9b13"},
            new string[] {"1P 3M 5P 7m 9m 13M", "", "13b9"},
            new string[] {"1P 3M 5P 7m 9m 13m", "", "7b9b13"},
            new string[] {"1P 3M 5P 7m 9m 9A", "", "7b9#9"},
            new string[] {"1P 3M 5P 9M", "", "Madd9 2 add9 add2"},
            new string[] {"1P 3M 5P 9m", "", "Maddb9"},
            new string[] {"1P 3M 5d", "", "Mb5"},
            new string[] {"1P 3M 5d 6M 7m 9M", "", "13b5"},
            new string[] {"1P 3M 5d 7M", "", "M7b5"},
            new string[] {"1P 3M 5d 7M 9M", "", "M9b5"},
            new string[] {"1P 3M 5d 7m", "", "7b5"},
            new string[] {"1P 3M 5d 7m 9M", "", "9b5"},
            new string[] {"1P 3M 7m", "", "7no5"},
            new string[] {"1P 3M 7m 13m", "", "7b13"},
            new string[] {"1P 3M 7m 9M", "", "9no5"},
            new string[] {"1P 3M 7m 9M 13M", "", "13no5"},
            new string[] {"1P 3M 7m 9M 13m", "", "9b13"},
            new string[] {"1P 3m 4P 5P", "", "madd4"},
            new string[] {"1P 3m 5P 6m 7M", "", "mMaj7b6"},
            new string[] {"1P 3m 5P 6m 7M 9M", "", "mMaj9b6"},
            new string[] {"1P 3m 5P 7m 11P", "", "m7add11 m7add4"},
            new string[] {"1P 3m 5P 9M", "", "madd9"},
            new string[] {"1P 3m 5d 6M 7M", "", "o7M7"},
            new string[] {"1P 3m 5d 7M", "", "oM7"},
            new string[] {"1P 3m 6m 7M", "", "mb6M7"},
            new string[] {"1P 3m 6m 7m", "", "m7#5"},
            new string[] {"1P 3m 6m 7m 9M", "", "m9#5"},
            new string[] {"1P 3m 5A 7m 9M 11P", "", "m11A"},
            new string[] {"1P 3m 6m 9m", "", "mb6b9"},
            new string[] {"1P 2M 3m 5d 7m", "", "m9b5"},
            new string[] {"1P 4P 5A 7M", "", "M7#5sus4"},
            new string[] {"1P 4P 5A 7M 9M", "", "M9#5sus4"},
            new string[] {"1P 4P 5A 7m", "", "7#5sus4"},
            new string[] {"1P 4P 5P 7M", "", "M7sus4"},
            new string[] {"1P 4P 5P 7M 9M", "", "M9sus4"},
            new string[] {"1P 4P 5P 7m 9M", "", "9sus4 9sus"},
            new string[] {"1P 4P 5P 7m 9M 13M", "", "13sus4 13sus"},
            new string[] {"1P 4P 5P 7m 9m 13m", "", "7sus4b9b13 7b9b13sus4"},
            new string[] {"1P 4P 7m 10m", "", "4 quartal"},
            new string[] {"1P 5P 7m 9m 11P", "", "11b9"}
        };

        //The names of all intervals that a chords may consist of.
        private static string[] IntervalNames = new string[]
        {
            "Root", "Second", "Third", "Fourth", "Fifth", "Sixth", "Seventh", "Octave",
            "Ninth", "Tenth", "Eleventh", "Twelfth", "Thirteenth", "Fourteenth", "Fifteenth"
        };

        public static string[] GetIntervalNames(string chordName)
        {
            var chordPattern = GetRootNoteAndChordPattern(chordName).Item2;
            return GetIntervalCodesFromChordPattern(chordPattern)
                .Select(ic => IntervalNames[Int64.Parse(ic.Substring(0, ic.Length - 1)) - 1])
                .ToArray();
        }

        public static HashSet<string> SortIntervalNames(this HashSet<string> intervalNames)
        {
            return intervalNames
                .OrderBy(iN => Array.FindIndex(IntervalNames, item => item == iN))
                .ToHashSet();
        }

        public static List<string> GenerateAllChordOptions()
        {
            List<string> chordOptions = new();

            foreach (var rootNote in _allNotes)
            {
                foreach (var chordFormula in ChordFormulas)
                {
                    string chordPattern = chordFormula[2].Split(" ")[0];
                    chordPattern = chordPattern == "M" ? "" : chordPattern;

                    chordOptions.Add(rootNote + chordPattern);
                }
            }

            return chordOptions;
        }

        private static (string, string) GetRootNoteAndChordPattern(string chordName)
        {
            string rootNote;
            string chordPattern;
            if (chordName.Length == 1)
            {
                rootNote = chordName.Substring(0, 1);
                chordPattern = "M";
            }
            else if (chordName[1] == '#')
            {
                rootNote = chordName.Substring(0, 2);
                if (chordName.Length == 2)
                {
                    chordPattern = "M";
                }
                else
                {
                    chordPattern = chordName[2..];
                }
            }
            else
            {
                rootNote = chordName.Substring(0, 1);
                chordPattern = chordName[1..];
            }

            return (rootNote, chordPattern);
        }

        private static string[] GetIntervalCodesFromChordPattern(string chordPattern)
        {
            return ChordFormulas
                .Where(formula => formula[2].Split(" ").Contains(chordPattern))
                .Select(formula => formula[0].Split(" "))
                .SingleOrDefault();
        }

        public static List<string> GetNoteValuesFromChordName(string chordName)
        {
            var (rootNote, chordPattern) = GetRootNoteAndChordPattern(chordName);

            int startIndex = _allNotes.IndexOf(rootNote);
            string[] intervalCodes = GetIntervalCodesFromChordPattern(chordPattern);
            
            List<string> noteValues = new();

            foreach (string intervalCode in intervalCodes)
            {
                noteValues.Add(_allNotes[(startIndex + IntervalMapping[intervalCode]) % _allNotes.Count]);
            }
            return noteValues;
        }

        public static List<string> GetAllSingleNoteOptions()
        {
            return new List<string>(_allNotes);
        }

      
        public static (int startPos, int endPos) GetNotePositionRange(this SongNote songNote)
        {
            int startPosition = songNote.Position;
            int endPosition = startPosition + songNote.Length - 1;
            return (startPosition, endPosition);
        }

        public static SongNote RemoveComponentInterval(this SongNote songNote, int intervalPosition)
        {
            var updatedNoteValues = songNote.GetNoteValues();
            if (songNote.ChordName == null) throw new ValidationException("A note needs to be a chord to have component intervals removed.");

            if (updatedNoteValues.Length > intervalPosition)
            {
                updatedNoteValues[intervalPosition] = "X";
            }
            songNote.SetNoteValues(updatedNoteValues);
            return songNote;
        }

        public static SongNote AddComponentInterval(this SongNote songNote, int intervalPosition)
        {
            var updatedNoteValues = songNote.GetNoteValues();
            if (songNote.ChordName == null)
            {
                throw new ValidationException("A note needs to be a chord to have component intervals added.");
            }
            if (updatedNoteValues.Length > intervalPosition)
            {
                updatedNoteValues[intervalPosition] = GetNoteValuesFromChordName(songNote.ChordName)[intervalPosition];
            }
            songNote.SetNoteValues(updatedNoteValues);
            return songNote;
        }

        public static List<SongNote> GetSongNotesFromDto(NoteDto[] noteDtos, SongBar bar)
        {
            List<SongNote> newNotes = new List<SongNote>();
            foreach (var noteDto in noteDtos)
            {
                bool emptyNote = noteDto.Notes[0] == "Z";
                if (emptyNote)
                {
                    for (int i = noteDto.Position; i < noteDto.Position + noteDto.Length; i++)
                    {
                        var noteToBeRemoved = bar.Notes.SingleOrDefault(n => n.Position == i);
                        bar.Notes.Remove(noteToBeRemoved);
                    }
                }
                else
                {
                    SongNote note = bar.Notes.SingleOrDefault(n => n.Position == noteDto.Position);
                    if (note == null)
                        note = NoteDto.ConvertToSongNote(noteDto, bar);
                    else
                    {
                        note.ChordName = noteDto.ChordName;
                        note.SetNoteValues(noteDto.Notes);
                        note.Length = noteDto.Length;
                    }
                    newNotes.Add(note);
                }
            }
            return newNotes;
        }


        public static SongNote Transpose(this SongNote songNote, int transposeValue)
        {
            if (songNote.ChordName == null)
            {
                int newNoteIndex = (_allNotes.IndexOf(songNote.GetNoteValues()[0]) + transposeValue) % _allNotes.Count;
                string[] tranposedNotes = new string[]
                {
                    _allNotes[newNoteIndex < 0 ? newNoteIndex + _allNotes.Count : newNoteIndex]
                };
                songNote.SetNoteValues(tranposedNotes);
            }
            else
            {
                var (rootNote, chordPattern) = GetRootNoteAndChordPattern(songNote.ChordName);
                int tranposedRootNoteIndex = (_allNotes.IndexOf(rootNote) + transposeValue) % _allNotes.Count;
                string transposedRootNote = _allNotes[tranposedRootNoteIndex < 0 ? tranposedRootNoteIndex + _allNotes.Count : tranposedRootNoteIndex];
                string transposedChordPattern = chordPattern == "M" ? "" : chordPattern;

                songNote.ChordName = transposedRootNote + transposedChordPattern;

                songNote.SetNoteValues(SongNoteExtension.GetNoteValuesFromChordName(songNote.ChordName).ToArray());
            }
            return songNote;
        }
    }
}
