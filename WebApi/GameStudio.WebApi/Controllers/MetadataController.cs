using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace GameStudio.WebApi
{
	[ApiVersionNeutral,Produces("application/json"),Route("[controller]"),ApiController]
	public sealed class MetadataController : ControllerBase
	{
        readonly IHostingEnvironment _env;

        public MetadataController(IHostingEnvironment env)
        {
            _env = env;
        }

		/// <summary>
		/// Retrieves information about the current build
		/// </summary>
		[HttpGet]
		[ProducesResponse(HttpStatus.OK)]
		[ProducesResponse(HttpStatus.InternalServerError)]
		public ActionResult<Metadata> Get()
		{
			return Ok(new Metadata { EnvironmentName =  _env.EnvironmentName });
		}
	}
}
