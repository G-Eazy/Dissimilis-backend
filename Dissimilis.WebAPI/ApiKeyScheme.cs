using Microsoft.OpenApi.Models;

namespace Dissimilis.WebAPI
{
    internal class ApiKeyScheme : OpenApiSecurityScheme
    {
        public string Description { get; set; }
        public string In { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }
}