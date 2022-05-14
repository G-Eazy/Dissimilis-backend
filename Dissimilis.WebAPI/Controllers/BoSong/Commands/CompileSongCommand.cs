﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Dissimilis.DbContext.Models.Enums;
using Dissimilis.DbContext.Models.Song;
using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsIn;
using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsOut;
using Dissimilis.WebAPI.Extensions.Interfaces;
using Dissimilis.WebAPI.Services;
using MediatR;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace Dissimilis.WebAPI.Controllers.BoSong.Commands
{
    public class CompileSongCommand : IRequest<MemoryStream>
    {
        public int songId { get; }

        public CompileSongCommand(int songId)
        {
            this.songId = songId;
        }
    }

    public class CompileSongCommandHandler : IRequestHandler<CompileSongCommand, MemoryStream>
    {
        private readonly SongRepository _songRepository;
        private readonly IAuthService _authService;
        private readonly IPermissionCheckerService _IPermissionCheckerService;

        public CompileSongCommandHandler(SongRepository songRepository, IAuthService authService, IPermissionCheckerService IPermissionCheckerService)
        {
            _songRepository = songRepository;
            _authService = authService;
            _IPermissionCheckerService = IPermissionCheckerService;
        }

        public async Task<MemoryStream> Handle(CompileSongCommand request, CancellationToken cancellationToken)
        {
            /*
            var noo = new List<string>() { "A", "A#", "B", "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "Z"};
            var basePath2 = Path.Combine(Environment.CurrentDirectory, "SongFiles");
            foreach (var no in noo)
            {
                var baseNote = Path.Combine(basePath2, "samples", no + ".waw");
                var outputNote = Path.Combine(basePath2, "samples2", no + ".wav");
                TrimWavFile(baseNote, outputNote, new TimeSpan(0, 0, 0, 0, 0), new TimeSpan(0, 0, 0, 5));
            }


            return null;
            */

            var jsonObject = await _songRepository.GetSongById(request.songId, cancellationToken);
            var bpm1 = jsonObject.Speed;
            var bpm = 50;
            if (bpm1 is not null)
                bpm = bpm1.Value;

            var noteList = new List<Dictionary<int, string>>();
            var notes = new Dictionary<int, string>();
            var noteLengths = new List<TimeSpan>();
            foreach (var voice in jsonObject.Voices)
            {
                foreach (var bar in voice.SongBars)
                {
                    notes = new Dictionary<int, string>();
                    var positionCounter = 0;
                    foreach (var note in bar.Notes)
                    {
                        notes[note.Position] = note.NoteValues;
                        noteLengths.Add(CalculateNoteLength(bpm, new TimeSpan(0, 0, 0, note.Length)));

                        positionCounter++;
                    }

                    // fill remaining empty sounds or dont end bar with silence
                noteList.Add(notes);
                }

                break; //after first voice
            }
            
            
            
            var fullNoteList = new List<string>();
            foreach (var subDict in noteList)
            {
                for(int i = 0;i < 8; i++)
                {
                    try
                    {
                        fullNoteList.Add(subDict[i]);

                    }
                    catch (Exception ex)
                    {
                        fullNoteList.Add("Z");
                        noteLengths.Add(CalculateNoteLength(bpm, new TimeSpan(0, 0, 0, 1)));
                    }
                }
            }

            var noteArray = fullNoteList.ToArray();
            var noteLengthArray = noteLengths.ToArray();

            // Add sound file for each note
            var basePath = Path.Combine(Environment.CurrentDirectory, "SongFiles");
            var audioList = new List<AudioFileReader>();
            var listme = new List<string>();
            for (var i = 0; i < noteArray.Length; i++)
            {
                var baseNote = Path.Combine(basePath, "samples2", noteArray[i] + ".wav");
                var outputNote = Path.Combine(basePath, "CutFiles", noteArray[i] + ".wav");
                listme.Add(outputNote);
                var noteDuration = noteLengthArray[i];
                if (noteArray[i] == "Z")
                {
                    noteDuration = new TimeSpan(0, 0, 0, 0, 944);
                }
                TrimWavFile(baseNote, outputNote, new TimeSpan(0, 0, 0, 0, 0), noteDuration); // todo: 225 minus

                //audioList.Add(new AudioFileReader(outputNote));

            }

            foreach (var sfs in listme)
            {
                audioList.Add(new AudioFileReader(sfs));
            }

            // create songfile from notes
            var songName = jsonObject.Title;
            var playlist = new ConcatenatingSampleProvider(audioList);
            WaveFileWriter.CreateWaveFile16(Path.Combine(basePath, songName), playlist);


            // Stream file
            var fileBytes = new byte[0];
            string fileLocation = Path.Combine(Environment.CurrentDirectory, "SongFiles", songName);

            var ms = new MemoryStream();
            var fs = new FileStream(fileLocation, FileMode.Open, FileAccess.Read);
            await fs.CopyToAsync(ms);
            return ms;

        }

        private TimeSpan CalculateNoteLength(int bpm, TimeSpan noteLength)
        {
            var eights = ((1000 * 60) / bpm) / 2;
            var len = noteLength.TotalSeconds;
            var ms = len * eights;
            if (ms < 1000)
            {
                return new TimeSpan(0, 0, 0, 0, (int) ms);
            }
            else
            {
                var baseval = (int) Math.Round(ms/1000);
                return new TimeSpan(0, 0, 0, baseval, (int) (ms - baseval*1000) );
            }
        }
        private void TrimWavFile(string inPath, string outPath, TimeSpan cutFromStart, TimeSpan cutFromEnd)
        {
            var diff = (int) Math.Abs((new TimeSpan(0, 0, 0, 5, 719) - cutFromEnd).TotalMilliseconds);
            cutFromEnd = new TimeSpan(0, 0, 0, 0, diff);
            using (WaveFileReader reader = new WaveFileReader(inPath))
            {
                using (WaveFileWriter writer = new WaveFileWriter(outPath, reader.WaveFormat))
                {
                    int bytesPerMillisecond = reader.WaveFormat.AverageBytesPerSecond / 1000;

                    int startPos = (int)cutFromStart.TotalMilliseconds * bytesPerMillisecond;
                    startPos = startPos - startPos % reader.WaveFormat.BlockAlign;

                    int endBytes = (int)cutFromEnd.TotalMilliseconds * bytesPerMillisecond;
                    endBytes = endBytes - endBytes % reader.WaveFormat.BlockAlign;
                    int endPos = (int)reader.Length - endBytes;

                    TrimWavFile(reader, writer, startPos, endPos);
                    writer.Close();
                    reader.Close();
                }
            }
        }
        private void TrimWavFile(WaveFileReader reader, WaveFileWriter writer, int startPos, int endPos)
        {
            reader.Position = startPos;
            byte[] buffer = new byte[1024];
            while (reader.Position < endPos)
            {
                int bytesRequired = (int)(endPos - reader.Position);
                if (bytesRequired > 0)
                {
                    int bytesToRead = Math.Min(bytesRequired, buffer.Length);
                    int bytesRead = reader.Read(buffer, 0, bytesToRead);
                    if (bytesRead > 0)
                    {
                        writer.WriteData(buffer, 0, bytesRead);
                    }
                }
            }
        }
    }
}