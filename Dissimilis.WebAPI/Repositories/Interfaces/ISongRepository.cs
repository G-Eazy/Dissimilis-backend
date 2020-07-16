using Dissimilis.WebAPI.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.Repositories.Interfaces
{
    public interface ISongRepository
    {
        Task<bool> DeleteSong(SuperDTO DeleteSongObject, uint userId);
        Task<bool> UpdateSong(UpdateSongDTO UpdateSongObject, uint userId);
        Task<SongDTO> CreateSong(NewSongDTO NewSongObject, uint userId);
        Task<SongDTO[]> SearchSongs(SongSearchDTO SongSearchObject);
        Task<SongDTO> GetSongById(SuperDTO SuperObject);

    }   
}
