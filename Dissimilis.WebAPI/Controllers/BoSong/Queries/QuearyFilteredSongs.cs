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
    public class QueryFilteredSongs : MediatR.IRequest<SongDTO[]>
    {
        public string Query { get; set; }
        public QueryFilteredSongs(string Query) {
            this.Query = Query;
        }
    }
    public class QueryFilteredSongsHandler : MediatR.IRequestHandler<QueryFilteredSongs, SongDTO[]>
    {
        private SongRepository _repository;
        public QueryFilteredSongsHandler(DissimilisDbContext context)
        {
            this._repository = new SongRepository(context);
        }
        public async Task<SongDTO[]> Handle(QueryFilteredSongs request, CancellationToken cancellationToken)
        {
            var SongModelArray = await _repository.GetFilteredSongs(request.Query, cancellationToken);
            var SongDTOArray = SongModelArray.Select(u => new SongDTO(u)).ToArray();
            return SongDTOArray;
        }

    }
}
