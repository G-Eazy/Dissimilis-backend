﻿using Dissimilis.DbContext.Models.Song;
using Dissimilis.WebAPI.Controllers.BoSong.DtoModelsOut;
using Dissimilis.WebAPI.Services;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.Controllers.BoSong
{
    public class QuerySongToLibrary : IRequest<SongIndexDto[]> { }

    public class QuerySongToLibraryHandler : IRequestHandler<QuerySongToLibrary, SongIndexDto[]>
    {
        private readonly Repository _repository;
        private readonly IAuthService _authService;

        public QuerySongToLibraryHandler(Repository repository, IAuthService authService) 
        {
            _repository = repository;
            _authService = authService;
        }

        public async Task<SongIndexDto[]> Handle(QuerySongToLibrary request, CancellationToken cancellationToken)
        {
            var user = _authService.GetVerifiedCurrentUser();

            if (user == null)
                return null;

            var result = await _repository.GetAllSongsInMyLibrary(user.Id, cancellationToken);

            return result.Select(s => new SongIndexDto(s)).ToArray();
        }
    }
}
