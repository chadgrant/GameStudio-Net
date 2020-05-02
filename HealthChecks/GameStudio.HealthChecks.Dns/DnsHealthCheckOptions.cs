namespace GameStudio.HealthChecks.Dns
{
	public class DnsHealthCheckOptions
	{
		public string[] Hosts { get; set; }
		public int TimeoutMilliseconds { get; set; }
	}
}