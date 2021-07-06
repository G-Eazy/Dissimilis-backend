using Dissimilis.Core.Collections;
using Dissimilis.DbContext.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using static Dissimilis.WebAPI.Extensions.Models.SongNoteExtension;


namespace Dissimilis.DbContext
{
    public static class DBMigrationJobs
    {
        public static void MigrateFromOldToNewChordFormat(DissimilisDbContext Context)
        {
            var notes = Context.SongNotes.ToArray();
            foreach (var note in notes)
            {
                note.SetNoteValues(ConvertToNewChordFormat(note.NoteValues.Split("|"), note.ChordName));
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
                newChordArr[i] = (oldChord.Contains(chordNotes.ElementAt(i))) ? chordNotes.ElementAt(i) : "";
            }
            return newChordArr;
        }

       
        public static async void SetInstrumentAsVoiceName(DissimilisDbContext Context)
        {
            var voices = await Context.SongVoices
                .Include(v => v.Instrument)
                .Where(v => v.VoiceName == null)
                .ToListAsync();

            //Updating the current voiceName with the Instrument.Name
            if(voices.Count > 0)
            {
                foreach(var voice in voices)
                {
                    voice.VoiceName = voice.Instrument?.Name;
                    voice.Instrument = null;
                }
            }
            //Deleting the "old" instruments, then making the new Instruments
            var instruments = await Context.Instruments
                .Where(i => i.DefinedInstrument == null)
                .ToListAsync();
            Context.RemoveRange(instruments);
            
            var allInstruments = EnumExtensions.GetEnumValues<DefinedInstruments>().ToArray();
            var storedInstruments = Context.Instruments.ToArray();

            //creating the new instruments from the defined enums

            var toAdd = allInstruments
                .Where(ai => storedInstruments.All(si => si.DefinedInstrument != ai))
                .Select(i=> new Instrument(i))
                .ToArray();
            Context.Instruments.AddRange(toAdd);

            //checking if some of the instrumentNames deviates from the enum description, if so changes it. 
            var updatedInstruments = Context.Instruments.ToArray();
            var toChange = updatedInstruments.Where(i => allInstruments.All(ai => i.Name != ai.GetDescription()))
                .Where(di => di.DefinedInstrument != null) 
            .Select(ti => ti.Name = ti.DefinedInstrument?.GetDescription());
        }
    }
}
