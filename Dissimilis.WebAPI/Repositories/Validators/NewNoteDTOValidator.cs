using Dissimilis.WebAPI.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.Repositories.Validators
{
    public class NewNoteDTOValidator : IValidator<NewNoteDTO>
    {
        public NewNoteDTOValidator() { }

        public bool IsValid(NewNoteDTO obj)
        {
            if (obj is null)
                throw new ArgumentNullException(nameof(obj));

            if (obj.BarId <= 0) return false;
            if (obj.NoteNumber <= 0) return false;
            if (obj.Length < 0) return false;
            // No check on the NoteValues[] for now, as frontend uses empty arrays when creating new Bars
            return true;
        }
    }
}
