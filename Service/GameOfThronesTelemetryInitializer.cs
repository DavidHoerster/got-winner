using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;

namespace got_winner_voting.Service.Telemetry
{
    public class GameOfThronesTelemetryInitializer : ITelemetryInitializer
    {
        public void Initialize(ITelemetry telemetry)
        {
            if (string.IsNullOrEmpty(telemetry.Context.Cloud.RoleName))
            {
                //set custom role name here
                telemetry.Context.Cloud.RoleName = "GoT App Role";
                telemetry.Context.Cloud.RoleInstance = "GoT App Instance";
            }
        }
    }
}