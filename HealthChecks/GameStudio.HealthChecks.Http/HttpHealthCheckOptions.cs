using System;

namespace GameStudio.HealthChecks.Http
{
	public class HttpHealthCheckOptions
	{
		public Uri[] Urls { get; set; }
		public int TimeoutMilliseconds { get; set; }
	}
}