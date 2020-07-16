using Dissimilis.WebAPI.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.DTOs
{
    public class NotesDTO
    {
        public int Length { get; set; }
        public string[] NoteValues { get; set; }
    }
}
