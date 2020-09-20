using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;

namespace Dissimilis.WebAPI.Services
{
    public class LoggingInitializer : ITelemetryInitializer
    {
        readonly string roleName;
        public LoggingInitializer(string roleName = null)
        {
            this.roleName = roleName ?? "api";
        }
        public void Initialize(ITelemetry telemetry)
        {
            telemetry.Context.Cloud.RoleName = roleName;
        }
    }
}
