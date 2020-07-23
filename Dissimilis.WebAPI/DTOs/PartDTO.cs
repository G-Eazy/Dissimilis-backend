using Dissimilis.WebAPI.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.DTOs
{
    public class PartDTO : NewPartDTO
    {
        public int Id { get; set; }

        public PartDTO() { }

        public PartDTO(IPart part)
        {
            if (part != null && part.Instrument != null)
            {
                this.Id = part.Id;
                this.PartNumber = part.PartNumber;
                this.SongId = part.SongId;
                this.Title = part.Instrument.Name;
            }

            else
                throw new ArgumentNullException(nameof(part));
        }
    }
}