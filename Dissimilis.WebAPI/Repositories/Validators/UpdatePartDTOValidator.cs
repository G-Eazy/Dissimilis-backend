using Dissimilis.WebAPI.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.Repositories.Validators
{
    public class UpdatePartDTOValidator : NewPartDTOValidator, IValidator<UpdatePartDTO>
    {
        public UpdatePartDTOValidator() { }

        public bool IsValid(UpdatePartDTO obj)
        {
            if (! base.IsValid(obj)) return false;

            if (obj.Id <= 0) return false;

            return true;
        }
    }
}
