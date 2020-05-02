using CorrelationId;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GameStudio.WebApi
{
	public abstract partial class StartupBase
	{
		protected IConfiguration Configuration;

		protected StartupBase(IConfiguration config)
		{
			Configuration = config;
		}
		
		public virtual void ConfigureServices(IServiceCollection services)
		{
			ConfigureMvcServices(services);

            services.AddCorrelationId();

            ConfigureHealthCheckServices(services);

			ConfigureMetricServices(services);

			ConfigureSwaggerServices(services);

			ConfigureContainerServices(services);
		}
		
		public virtual void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
            app.UseCorrelationId(new CorrelationIdOptions { UseGuidForCorrelationId = true });

            ConfigureMvc(app,env);

            ConfigureHealthChecks(app,env);

			ConfigureMetrics(app, env);

			if (!env.IsProduction())
				ConfigureSwagger(app, env);
			else
				app.UseDeveloperExceptionPage();

			ConfigureContainer(app, env);
		}
	}
}
