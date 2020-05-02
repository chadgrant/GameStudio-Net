using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using GameStudio.ConfigurationConventions;

namespace GameStudio.WebApi.Convention
{
	public abstract class StartupBase : GameStudio.WebApi.StartupBase
	{
		protected StartupBase(IConfiguration config) : base(config)
		{
		}

		protected override IEnumerable<IConfigureServices> GetConfigureServicesConventions(IServiceCollection services)
		{
            
			return base.GetConfigureServicesConventions(services).Concat(
				new IConfigureServices[] {
					new OptionsConfigurationConvention(ExportedTypes.Value, Configuration),
					new MetricsConvention(ExportedTypes.Value),
					new HealthCheckConvention(ExportedTypes.Value,services.AddHealthChecks()),
                    new MapperConvention(ExportedTypes.Value), 
					new RepositoryConvention(ExportedTypes.Value,Configuration.GetSection("repositoryDecorators").Get<RepositoryOptions>()),
				}
			);
		}
	}
}
