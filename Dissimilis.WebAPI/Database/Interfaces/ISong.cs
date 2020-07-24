using Dissimilis.WebAPI.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.Database.Interfaces
{
    public interface ISong : IBaseEntity
    {
        User Arranger { get; }
        int ArrangerId { get; }
        string Composer { get; }
        int Id { get; }
        string[] TimeSignature { get; }
        string Title { get; }
    }
}
