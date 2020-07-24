using Dissimilis.WebAPI.Database.Models;
using Dissimilis.WebAPI.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.Repositories.Interfaces
{
    public interface ISongRepository
    {
        Task<bool> DeleteSong(int songId, uint userId);
        Task<bool> UpdateSong(UpdateSongDTO UpdateSongObject, uint userId);
        Task<int> CreateSong(NewSongDTO NewSongObject, uint userId);
        Task<SongDTO> CreateSongWithPart(NewSongDTO NewSongObject, uint userId);
        Task<UpdateSongDTO[]> SearchSongs(SongQueryDTO SongQueryObject);
        Task<SongDTO> GetSongById(int songId);
        Task<PartDTO[]> GetAllPartsForSong(int songId);

    }   
}
