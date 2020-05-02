using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace GameStudio.ConfigurationConventions
{
	public interface IConfigure
	{
		void Configure(IConfiguration configuration, IApplicationBuilder app, IHostingEnvironment env);
	}
}
