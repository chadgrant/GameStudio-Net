using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;

namespace GameStudio.WebApi.TestApi
{
	public class Startup : GameStudio.WebApi.Convention.StartupBase
    {
        public Startup(IConfiguration config) : base(config)
        {
        }

        protected override ApiInfo CreateInfoForApiVersion(ApiVersionDescription version)
        {
            return new ApiInfo
            {
                Title = $"Test API {version.ApiVersion}",
                Version = version.ApiVersion.ToString(),
                Description = "A sample application with Swagger, Swashbuckle, and API versioning.",
                Contact = new ApiContact { Name = "Richard Hertz", Email = "dickhertz@gamestudio.com" }
            };
        }
    }
}
