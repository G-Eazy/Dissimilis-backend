using Experis.Ciber.Authentication.Microsoft;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.Authentication
{
    public class DissimilisServicePrincipal : AzureServicePrincipalBase
    {
        public override Guid ApplicationID => new Guid ("5a41da85-fa69-4aa0-93f2-1ce65104a1b2");

        public override string ClientSecret => "86a8.sy8U75O-owRYNV9Ku9~5.DDb6e-SU";

        public override Guid DirectoryID => new Guid("774897da-0c2c-4c71-9897-873c4d659aee");

        //Set it in the constructor (needs to be passed through the constructor later)
        protected override Uri WebAppURL { get; }

        //The maximum level of request we are going to request from the user
        protected override Permissions[] ScopePermissions =>  new[] { Permissions.UserRead };

        protected override bool UseSingleTenant => false;

        public DissimilisServicePrincipal()
        {
            this.WebAppURL = new Uri("https://localhost:5001");
        }
    }
}
