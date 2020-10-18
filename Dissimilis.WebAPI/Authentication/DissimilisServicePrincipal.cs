using Experis.Ciber.Authentication.Microsoft;
using System;
using Dissimilis.Configuration;

namespace Dissimilis.WebAPI.Authentication
{
    public class DissimilisServicePrincipal : AzureServicePrincipalBase
    {
        public override Guid ApplicationID => ConfigurationInfo.GetAzureApplicationId();

        public override string ClientSecret => ConfigurationInfo.GetAzureClientSecret();

        public override Guid DirectoryID => ConfigurationInfo.GetAzureDirectoryId(); 

        //Set it in the constructor (needs to be passed through the constructor later)
        protected override Uri WebAppURL { get; }

        //The maximum level of request we are going to request from the user
        protected override Permissions[] ScopePermissions =>  new[] { Permissions.UserRead };

        protected override bool UseSingleTenant => false;

        public DissimilisServicePrincipal(string webAppUrl)
        {
            this.WebAppURL = new Uri(webAppUrl);
        }
    }
}
