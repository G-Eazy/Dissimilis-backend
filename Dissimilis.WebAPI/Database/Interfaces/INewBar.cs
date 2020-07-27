using Dissimilis.WebAPI.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.Database.Interfaces
{
    public interface INewBar
    {
        public ushort BarNumber { get; }
        public int PartId { get; }
        public bool RepBefore { get; }
        public bool RepAfter { get; }
        public byte? House { get; }
        public NewNoteDTO[] ChordsAndNotes { get; }
    }
}
