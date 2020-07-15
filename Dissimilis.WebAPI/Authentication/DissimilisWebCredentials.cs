using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.Authentication
{
    public class DissimilisWebCredentials : Experis.Ciber.Web.API.Interfaces.IAPICredentials
    {
        public string APIKey { get ; set ; }

        public uint UserID { get; }

        public DissimilisWebCredentials(uint userId) {
            this.UserID = userId;
        }
    }
}
