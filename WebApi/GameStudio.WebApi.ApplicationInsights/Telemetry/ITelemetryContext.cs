using System.Collections.Generic;

namespace GameStudio.WebApi.ApplicationInsights.Telemetry
{
	public interface ITelemetryContext
	{
		IEnumerable<TelemetryCondition> TelemetryConditions { get; set; }
	}
}