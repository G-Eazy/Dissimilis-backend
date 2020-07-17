﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.DTOs
{
    public class BarDTO
    {
        public int Id { get; set; }
        public byte BarNumber { get; set; }
        public int PartId { get; set; }

        public BarDTO() { }
    }
}
