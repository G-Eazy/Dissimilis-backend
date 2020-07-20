﻿using Dissimilis.WebAPI.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.DTOs
{
    public class SongQueryDTO
    {
        public string Title { get; set; }
        public int ArrangerId { get; set; }
        public uint Num { get; set; }
        public bool OrderByDateTime { get; set; }

    }
}