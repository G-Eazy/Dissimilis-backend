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
    public class SongsByArrangerQuery : MediatR.IRequest<SongDTO[]>
    {
        public SongsByArrangerDTO SongsByArrangerObject { get; set; }

        public SongsByArrangerQuery(SongsByArrangerDTO SongsByArrangerObject)
        {
            this.SongsByArrangerObject = SongsByArrangerObject;
        }

    }
    public class SongsByArrangerQueryHandler : MediatR.IRequestHandler<SongsByArrangerQuery, SongDTO[]>
    {
        private SongRepository _repository;
        public SongsByArrangerQueryHandler(DissimilisDbContext context)
        {
            this._repository = new SongRepository(context);
        }
        public async Task<SongDTO[]> Handle(SongsByArrangerQuery request, CancellationToken cancellationToken)
        {
            var SongModelArray = await _repository.GetSongsByArranger(request.SongsByArrangerObject, cancellationToken);
            var SongDTOArray = SongModelArray.Select(u => new SongDTO(u)).ToArray();

            // Add by ArrangerID
            // Add ordering
            return SongDTOArray;
        }
        
    }
}
