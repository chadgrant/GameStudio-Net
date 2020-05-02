using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;
using Prometheus;

namespace GameStudio.Metrics.Prometheus
{
	public class MetricsProvider : IMetricsProvider
	{
		public void Configure(IApplicationBuilder app)
		{
			app.UseMetricServer();

			var opt = app.ApplicationServices.GetService( typeof(IOptions<PrometheusOptions>)) as IOptions<PrometheusOptions>;
			if (opt == null || string.IsNullOrEmpty(opt.Value.PushGateway))
				return;

			var metricServer = new MetricPusher(endpoint: opt.Value.PushGateway, job: opt.Value.Job);
			metricServer.Start();
		}
	}
}
