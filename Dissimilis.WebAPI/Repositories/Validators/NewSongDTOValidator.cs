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
            if (obj is null)
                throw new ArgumentNullException(nameof(obj));

            if (string.IsNullOrWhiteSpace(obj.Title)) return false;
            if (string.IsNullOrWhiteSpace(obj.TimeSignature)) return false;
            
            return true;
        }
    }
}
