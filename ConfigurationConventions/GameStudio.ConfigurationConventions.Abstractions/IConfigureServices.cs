using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GameStudio.ConfigurationConventions
{
	public interface  IConfigureServices
	{
		void ConfigureServices(IConfiguration configuration, IServiceCollection services);
	}
}
