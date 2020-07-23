using Dissimilis.WebAPI.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.Repositories.Validators
{
    public class UpdateBarDTOValidator : NewBarDTOValidator, IValidator<UpdateBarDTO>
    {
        public UpdateBarDTOValidator() { }

        public bool IsValid(UpdateBarDTO obj)
        {
            if (! base.IsValid(obj)) return false;

            if (obj.Id <= 0) return false;

            return true;
        }
    }
}
