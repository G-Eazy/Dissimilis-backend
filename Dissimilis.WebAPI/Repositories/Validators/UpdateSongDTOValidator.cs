using Dissimilis.WebAPI.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.Repositories.Validators
{
    public class UpdateSongDTOValidator : NewSongDTOValidator, IValidator<UpdateSongDTO>
    {
        public UpdateSongDTOValidator() { }

        public bool IsValid(UpdateSongDTO obj)
        {
            if (! base.IsValid(obj)) return false;

            Type t = obj.GetType();
            var properties = t.GetProperties();

            foreach (PropertyInfo p in properties)
            {
                if (p.PropertyType == typeof(int))
                    if ((int)p.GetValue(obj) <= 0)
                        return false;
            } 

            return true;
        }
    }
}
