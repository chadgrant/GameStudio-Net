using Microsoft.AspNetCore.Builder;

namespace GameStudio.Metrics
{
    public class NullMetricsProvider : IMetricsProvider
    {
        public void Configure(IApplicationBuilder app)
        {
        }
    }
}
