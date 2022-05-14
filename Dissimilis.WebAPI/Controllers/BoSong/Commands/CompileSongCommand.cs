using System;
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

        public CompileSongCommand (int songId)
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
            var jsonObject = await _songRepository.GetSongById(request.songId, cancellationToken);
            var takt = jsonObject.Numerator;
            var bpm = jsonObject.Speed;


            var notes = new List<string>();
            foreach (var voice in jsonObject.Voices)
            {
                foreach(var bar in voice.SongBars)
                {
                    foreach(var note in bar.Notes)
                    {
                        // Breaks on chords
                        notes.Add(note.NoteValues);
                    }
                }


                break; //after first voice
            }
            var basePath = Path.Combine(Environment.CurrentDirectory, "SongFiles");
            var audioList = new List<AudioFileReader>();
            foreach (var note in notes)
            {
                var path = Path.Combine(basePath, "samples", note + ".mp3");
                audioList.Add(new AudioFileReader(path));

            }


            var songName = jsonObject.Title;
            var playlist = new ConcatenatingSampleProvider(audioList);
            WaveFileWriter.CreateWaveFile16(Path.Combine(basePath, songName), playlist);



            var fileBytes = new byte[0];
            string fileLocation = Path.Combine(Environment.CurrentDirectory,  "SongFiles", songName);

            var ms = new MemoryStream();
            var fs = new FileStream(fileLocation, FileMode.Open, FileAccess.Read);
            await fs.CopyToAsync(ms);
            return ms;

        }
    }
}