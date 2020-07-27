using Dissimilis.WebAPI.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.Repositories.Validators
{
    public class NewBarDTOValidator : IValidator<NewBarDTO>
    {
        public NewBarDTOValidator() { }

        public bool IsValid(NewBarDTO obj)
        {
            if (obj is null) return false;
            if (obj.BarNumber <= 0) return false;
            if (obj.PartId <= 0) return false;
            //if (obj.House < 0 && obj.House != null) return false;
            
            return true;
        }
    }
}
