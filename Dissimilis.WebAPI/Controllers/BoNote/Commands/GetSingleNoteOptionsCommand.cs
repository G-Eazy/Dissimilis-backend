using Dissimilis.WebAPI.Controllers.BoNote.DtoModelsOut;
using Dissimilis.WebAPI.Extensions.Models;
using MediatR;

namespace Dissimilis.WebAPI.Controllers.BoNote.Commands
{
    public class GetSingleNoteOptionsCommand : IRequest<ChordOptionsDto>
    {
        public GetSingleNoteOptionsCommand()
        {

        }
    }

    public class GetSingleNoteOptionsCommandHandler
    {
        public static SingleNoteOptionsDto Handle()
        {
            return new SingleNoteOptionsDto()
            {
                SingleNoteOptions = SongNoteExtension.GetAllSingleNoteOptions()
            };
        }
    }
}
