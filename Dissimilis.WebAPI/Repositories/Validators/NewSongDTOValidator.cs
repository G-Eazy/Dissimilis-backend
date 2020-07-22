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

            Type t = obj.GetType();
            var properties = t.GetProperties();

            foreach(PropertyInfo p in properties)
            {
                if (p.PropertyType == typeof(string))
                    if (string.IsNullOrWhiteSpace((string)p.GetValue(obj)))
                        return false;
            }
            
            return true;
        }
    }
}
