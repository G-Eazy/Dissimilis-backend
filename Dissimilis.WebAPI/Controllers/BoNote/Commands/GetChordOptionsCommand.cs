using Dissimilis.WebAPI.Controllers.BoNote.DtoModelsOut;
using Dissimilis.WebAPI.Extensions.Models;
using MediatR;

namespace Dissimilis.WebAPI.Controllers.BoNote.Commands
{
    public class GetChordOptionsCommand : IRequest<ChordOptionsDto>
    {
        public GetChordOptionsCommand()
        {

        }
    }

    public class GetChordOptionsCommandHandler
    {
        public static ChordOptionsDto Handle()
        {
            return new ChordOptionsDto()
            {
                ChordOptions = SongNoteExtension.GetAllChordOptions()
            };
        }
    }
}
