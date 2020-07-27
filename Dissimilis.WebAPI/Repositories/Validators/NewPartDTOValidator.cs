using Dissimilis.WebAPI.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.Repositories.Validators
{
    public class NewPartDTOValidator : IValidator<NewPartDTO>
    {
        public NewPartDTOValidator() { }

        public bool IsValid(NewPartDTO obj)
        {
            if (obj is null) return false;
            if (obj.SongId <= 0) return false;
            if (string.IsNullOrWhiteSpace(obj.Title)) return false;
            if (obj.PartNumber <= 0) return false;
            
            return true;
        }
    }
}
