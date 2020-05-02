using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace GameStudio.HealthChecks.Http
{
	public class HttpHealthCheck : IHealthCheck
	{
		readonly IOptionsMonitor<HttpHealthCheckOptions> _options;
		readonly HttpClient _client = new HttpClient();

		public HttpHealthCheck(IOptionsMonitor<HttpHealthCheckOptions> options)
		{
			_options = options ?? throw new ArgumentNullException(nameof(options));
		}

		public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default(CancellationToken))
		{
			var opt = _options.CurrentValue;

			foreach (var url in opt.Urls)
			{
				var t = _client.GetAsync(url, cancellationToken);
				if (!t.Wait(opt.TimeoutMilliseconds, cancellationToken))
					return Task.FromResult(HealthCheckResult.Unhealthy($"request failed/timed out for {url}"));

				if (t.Result == null || !t.Result.IsSuccessStatusCode)
					return Task.FromResult(HealthCheckResult.Unhealthy($"request failed for {url}"));
			}
			return Task.FromResult(HealthCheckResult.Healthy());
		}
	}
}
