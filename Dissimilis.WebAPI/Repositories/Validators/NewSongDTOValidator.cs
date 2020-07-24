using Dissimilis.WebAPI.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.Repositories.Validators
{
    public class NewSongDTOValidator : IValidator<NewSongDTO>
    {
        public NewSongDTOValidator() { }

        public bool IsValid(NewSongDTO obj)
        {
            if (obj is null) return false;
            if (string.IsNullOrWhiteSpace(obj.Title)) return false;
            if (obj.TimeSignature.Count() < 0 && obj.TimeSignature.Count() > 2) return false;
            
            return true;
        }
    }
}
