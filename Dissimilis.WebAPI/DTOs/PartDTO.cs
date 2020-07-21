using Dissimilis.WebAPI.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.DTOs
{
    public class PartDTO : NewPartDTO
    {
        public PartDTO(Part PartModelObject) {
            base.SongId = PartModelObject.SongId;
            base.Title = PartModelObject.Instrument.Name;
            base.Priority = PartModelObject.PartNumber;
            }
    }
}
