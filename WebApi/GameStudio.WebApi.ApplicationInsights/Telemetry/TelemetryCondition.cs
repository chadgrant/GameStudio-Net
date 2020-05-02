using System.Collections.Generic;

namespace GameStudio.WebApi.ApplicationInsights.Telemetry
{
	public class TelemetryCondition
	{
		public TelemetryType Type { get; set; }
		public Dictionary<LeftOperand, IEnumerable<string>> Condition { get; set; }
	}
}
