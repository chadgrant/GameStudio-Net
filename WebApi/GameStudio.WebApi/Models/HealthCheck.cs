using System;
using System.Collections.Generic;

namespace GameStudio.WebApi
{
	public class HealthCheckReport
	{
		/// <summary>
		/// The time at which this report was generated (this may not be the current time)
		/// </summary>
		/// <example>2015-03-12T19:40:18.877Z</example>
		public DateTime ReportAsOf { get; set; } = DateTime.UtcNow;

		/// <summary>
		/// Number of milliseconds taken to run the report
		/// </summary>
		/// <example>1000</example>
		public long DurationMilliseconds { get; set; }

		/// <summary>
		/// Array of health check test reports
		/// </summary>
		public List<HealthCheckResult> Tests { get; set; } = new List<HealthCheckResult>();
	}

	public class HealthCheckResult
	{
		/// <summary>
		/// Number of milliseconds taken to run the report
		/// </summary>
		/// <example>50</example>
		public long DurationMilliseconds { get; set; }

		/// <summary>
		/// Name of the healthcheck test
		/// </summary>
		/// <example>Mongo</example>
		public string Name { get; set; }

		/// <summary>
		/// The state of the test, may be "notrun", "running", "passed", "failed"
		/// </summary>
		/// <example>passed</example>
		public string Result { get; set; }

		/// <summary>
		/// The last time the test was run
		/// </summary>
		/// <example>2015-03-12T19:40:18.877Z</example>
		public DateTime TestedAt { get; set; }

		public string Message { get; set; }
	}
}
