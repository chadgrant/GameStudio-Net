using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using GameStudio.Metrics;

namespace GameStudio.WebApi
{
	public partial class StartupBase
	{
		public virtual void ConfigureMetricServices(IServiceCollection services)
		{
		}

		public virtual void ConfigureMetrics(IApplicationBuilder app, IHostingEnvironment env)
		{
			var prov = app.ApplicationServices.GetService<IMetricsProvider>();
			prov?.Configure(app);
		}
	}
}
