using Microsoft.AspNetCore.Builder;

namespace GameStudio.Metrics
{
	public interface IMetricsProvider
	{
		void Configure(IApplicationBuilder app);
	}
}
