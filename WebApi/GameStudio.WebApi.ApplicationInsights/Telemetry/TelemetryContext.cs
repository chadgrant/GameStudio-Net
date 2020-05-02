using System.Collections.Generic;

namespace GameStudio.WebApi.ApplicationInsights.Telemetry
{
	public class TelemetryContext : ITelemetryContext
	{
		public IEnumerable<TelemetryCondition> TelemetryConditions { get; set; }
	}
}