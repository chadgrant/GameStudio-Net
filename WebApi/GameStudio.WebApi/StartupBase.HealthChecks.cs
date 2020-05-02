using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace GameStudio.WebApi
{
	public partial class StartupBase
	{
		public virtual void ConfigureHealthCheckServices(IServiceCollection services)
		{
			services.AddHealthChecks();
		}

		public virtual void ConfigureHealthChecks(IApplicationBuilder app, IHostingEnvironment env)
		{
		}
	}
}
