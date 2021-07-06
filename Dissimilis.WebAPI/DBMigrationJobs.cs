using Dissimilis.DbContext.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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

        enum DefinedInstruments
        {
            Accordion      ,
            Acoustic_Guitar,
            Bagpipes       ,
            Banjo          ,
            Bass_Guitar    ,
            Bongo_Grums    ,
            Bugle          ,
            Cello          ,
            Clarinet       ,
            Cymbals        ,
            Drums          ,
            Electric_Guitar,
            Flute          ,
            French_Gorn    ,
            Harmonica      ,
            Keyboard       ,
            Maracas        ,
            Organ          ,
            Pan_Glute      ,
            Piano          ,
            Recorder       ,
            Saxophone      ,
            Tambourine     ,
            Triangle       ,
            Trombone       ,
            Trumpet        ,
            Tuba           ,
            Ukulele        ,
            Violin         ,
            Xylophone            
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
                .Where(i => !Enum.IsDefined(typeof(DefinedInstruments), i.Name))
                .ToListAsync();
            if(instruments.Count > 0)
            {
            foreach(var instrument in instruments)
            {
                Context.Instruments.Remove(instrument);
            }
            }
            //creating the new instruments from the defined enums
            var newInstruments = await Context.Instruments.ToListAsync();
            foreach(var enumInstrument in Enum.GetValues(typeof(DefinedInstruments)))
            {
                foreach( var instrument in newInstruments)
                {
                    if(!Enum.IsDefined(typeof(DefinedInstruments), (instrument.Name)))
                    {
                        var createdInstrument = new Instrument(Enum.GetName(typeof(DefinedInstruments), enumInstrument));
                        await Context.Instruments.AddAsync(createdInstrument);
                    }
                }
            }



        }
    }
}
