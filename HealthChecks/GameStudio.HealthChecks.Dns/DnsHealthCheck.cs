using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace GameStudio.HealthChecks.Dns
{
	public class DnsHealthCheck : IHealthCheck
	{
		readonly IOptionsMonitor<DnsHealthCheckOptions> _options;

		public DnsHealthCheck(IOptionsMonitor<DnsHealthCheckOptions> options)
		{
			_options = options ?? throw new ArgumentNullException(nameof(options));
		}

		public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default(CancellationToken))
		{
			var opt = _options.CurrentValue;

			foreach (var host in opt.Hosts)
			{
				var t = System.Net.Dns.GetHostEntryAsync(host);
				if (!t.Wait(opt.TimeoutMilliseconds, cancellationToken))
					return Task.FromResult(HealthCheckResult.Unhealthy($"lookup failed/timed out for {host}"));

				if (t.Result == null || !t.Result.AddressList.Any())
					return Task.FromResult(HealthCheckResult.Unhealthy($"lookup failed for {host}"));
			}
			return Task.FromResult(HealthCheckResult.Healthy());
		}
	}
}
