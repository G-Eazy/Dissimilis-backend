using Dissimilis.WebAPI.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.Repositories.Validators
{
    public class UpdateNoteDTOValidator : NewNoteDTOValidator, IValidator<UpdateNoteDTO>
    {
        public UpdateNoteDTOValidator() { }

        public bool IsValid(UpdateNoteDTO obj)
        {
            if (! base.IsValid(obj)) return false;

            if (obj.Id <= 0) return false;

            return true;
        }
    }
}
