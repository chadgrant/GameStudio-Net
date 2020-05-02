using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace GameStudio.ConfigurationConventions
{
	public class HealthCheckTimer : IHealthCheck
	{
		readonly IHealthCheck _decorated;

		public HealthCheckTimer(IHealthCheck decorated)
		{
			_decorated = decorated;
		}

		public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
		{
            if (_decorated == null)
                return HealthCheckResult.Unhealthy("Decorated is null");

            var data = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase) { { "testedAt", DateTime.UtcNow } };

			var timer = new Stopwatch();
			timer.Start();

            string message;
            HealthStatus status;
            Exception ex = null;

            try
            {
                var result = await _decorated.CheckHealthAsync(context, cancellationToken);

                if (Equals(default(HealthCheckResult), result))
                {
                    message = "Healthcheck returned null/default";
                    status = HealthStatus.Unhealthy;
                }
                else
                {
                    message = result.Description;
                    status = result.Status;

                    if (result.Data != null)
                    {
                        foreach (var kv in result.Data)
                            data[kv.Key] = kv.Value;
                    }
                }
            }
            catch (Exception e)
            {
                message = "Test failed";
                ex = e;
                status = HealthStatus.Unhealthy;
            }
            finally
            {
                timer.Stop();
                data["testDuration"] = timer.ElapsedMilliseconds;
            }

            if (status == HealthStatus.Healthy)
                return HealthCheckResult.Healthy(message, data);

            return HealthCheckResult.Unhealthy(message, ex, data);
        }
	}
}
