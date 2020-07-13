using Dissimilis.WebAPI.Controllers.BoSong.DTOs;
using Dissimilis.WebAPI.Database;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.Controllers.BoSong.Queries
{   
    public class QueryAllSongs : MediatR.IRequest<SongDTO[]>
    {
        public QueryAllSongs() { }
    }
    public class QueryAllSongsHandler : MediatR.IRequestHandler<QueryAllSongs, SongDTO[]>
    {
        private SongRepository _repository;
        public QueryAllSongsHandler(DissimilisDbContext context)
        {
            this._repository = new SongRepository(context);
        }
        public async Task<SongDTO[]> Handle(QueryAllSongs request, CancellationToken cancellationToken)
        {
            var SongModelArray = await _repository.GetAllSongs(cancellationToken);
            var SongDTOArray = SongModelArray.Select(u => new SongDTO(u)).ToArray();
            return SongDTOArray;
        }
        
    }
}
