﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.DTOs
{
    public class UpdateNoteDTO : NewNoteDTO
    {
        public int Id { get; set; }
    }
}
