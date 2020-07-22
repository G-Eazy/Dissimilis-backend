﻿using Dissimilis.WebAPI.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.DTOs
{
    public class PartDTO : NewPartDTO
    {
        public int Id { get; set; }
        public BarDTO[] Bars { get; set; }

        public PartDTO() { }

        public PartDTO(IPart part)
        {
            if (part is null)
                throw new ArgumentNullException(nameof(part));

            this.Id = part.Id;
            this.PartNumber = part.PartNumber;
            this.SongId = part.SongId;
            this.Title = part.Instrument.Name;
        }
    }
}