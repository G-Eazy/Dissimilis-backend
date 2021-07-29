using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.Controllers.BoVoice.DtoModelsIn
{
    public class RemoveComponentIntervalDto
    {
        public bool DeleteChordsOnLastIntervalRemoved { get; set; } = false;
        public int IntervalPosition { get; set; }
    }
}
