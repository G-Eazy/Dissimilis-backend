using Dissimilis.Core.Collections;
using Dissimilis.DbContext.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using static Dissimilis.WebAPI.Extensions.Models.SongNoteExtension;


namespace Dissimilis.DbContext
{
    public static class DBMigrationJobs
    {
        public static void MigrateFromOldToNewChordFormatAsync(DissimilisDbContext Context)
        {
            var notes =  Context.SongNotes.ToArray();
            foreach (var note in notes)
            {
                if(note.ChordName != null)
                {
                note.SetNoteValues(ConvertToNewChordFormat(note.NoteValues.Split("|"), note.ChordName));

                }
            }
            Context.SaveChanges();
        }

        /// <summary>
        /// Converts from the old chord format to the new one.
        /// Old format would remove all evidence of an interval being in a chord when deleting a note, new format inserts an empty string instead.
        /// Example: 
        /// C-maj chord = ["C","E","G"]
        /// 
        /// Old format without major third:
        /// ["C","G"]
        /// New format without major third:
        /// ["C","","G"]
        /// </summary>
        /// <param name="oldChord"></param>
        /// <param name="chordName"></param>
        /// <returns></returns>
        private static string[] ConvertToNewChordFormat(string[] oldChord, string chordName)
        {
            List<string> chordNotes = GetNoteValuesFromChordName(chordName);
            string[] newChordArr = new string[chordNotes.Count];

            for (int i = 0; i < chordNotes.Count; i++)
            {
                newChordArr[i] = (oldChord.Contains(chordNotes.ElementAt(i))) ? chordNotes.ElementAt(i) : "X";
            }
            return newChordArr;
        }

       
        public static void SetInstrumentAsVoiceName(DissimilisDbContext Context)
        {
            var voices = Context.SongVoices
                .Include(v => v.Instrument)
                .Where(v => v.VoiceName == null)
                .ToArray();

            //Updating the current voiceName with the Instrument.Name
            if(voices.Length > 0)
            {
                foreach(var voice in voices)
                {
                    voice.VoiceName = voice.Instrument?.Name;
                    voice.Instrument = null;
                }
            }
        }
        public static void DeleteOldInstruments(DissimilisDbContext Context)
        {

            //Deleting the "old" instruments, then making the new Instruments
            var instruments = Context.Instruments
                .Where(i => i.DefinedInstrument == null)
                .ToArray();
            Context.RemoveRange(instruments);
            Context.SaveChanges();
        }

        public static void CreateNewInstruments(DissimilisDbContext Context)
        {
            //creating the new instruments from the defined enums
            
            var allInstruments = EnumExtensions.GetEnumValues<DefinedInstruments>().ToArray();
            var storedInstruments = Context.Instruments.ToArray();

        var toAdd = allInstruments
                .Where(ai => storedInstruments.All(si => si.DefinedInstrument != ai))
                .Select(i=> new Instrument(i))
                .ToArray();
            Context.Instruments.AddRange(toAdd);
            Context.SaveChanges();

        }

        public static void UpdateExistingInstrumentName(DissimilisDbContext Context)
        {
            var allInstruments = EnumExtensions.GetEnumValues<DefinedInstruments>().ToArray();
            //checking if some of the instrumentNames deviates from the enum description, if so changes it. 
            var updatedInstruments = Context.Instruments.ToArray();
            var toChange = updatedInstruments.Where(i => allInstruments.All(ai => i.Name != ai.GetDescription()))
                .Where(di => di.DefinedInstrument != null);
            toChange.ForEach(instrument => instrument.Name = instrument.DefinedInstrument?.GetDescription());
            Context.SaveChanges();
        }
    }
}
