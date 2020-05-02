using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using SimpleInjector;

namespace GameStudio.WebApi
{
	[ApiVersionNeutral,Produces("application/json"),Route("[controller]"),ApiController]
	public class DebugController : ControllerBase
	{
		readonly IActionDescriptorCollectionProvider _actionDescriptors;
		readonly IHostingEnvironment _environment;
        readonly Container _container;
        readonly IServiceCollection _services;

        public DebugController(
            IActionDescriptorCollectionProvider actionDescriptorCollectionProvider,
            IHostingEnvironment environment,
            Container container,
            IServiceCollection services)
		{
			_actionDescriptors = actionDescriptorCollectionProvider;
			_environment = environment;
            _container = container;
            _services = services;
        }

		/// <summary>
		/// Returns configured environment
		/// </summary>
		[HttpGet("environment")]
		[ProducesResponse(HttpStatus.OK)]
		public ActionResult<string> Environment()
		{
			return Ok(_environment.EnvironmentName);
		}

		/// <summary>
		/// Useful in debugging headers echoed back from origin to determine
		/// headers the edge or CDN may be sending
		/// </summary>
		[HttpGet("headers")]
		[ProducesResponse(HttpStatus.OK)]
		public ActionResult<IHeaderDictionary> Headers()
		{
			return Ok(Request.Headers);
		}

		/// <summary>
		/// Route debugging, returns registered routes
		/// </summary>
		/// <returns></returns>
		[HttpGet("routes")]
		[ProducesResponse(HttpStatus.OK)]
		public ActionResult<string> Routes()
		{
			return Ok(_actionDescriptors.ActionDescriptors.Items.OrderBy(i=>i.AttributeRouteInfo.Order).Select(x => new 
			{
				Action = x.RouteValues["Action"],
				Controller = x.RouteValues["Controller"],
				Name = x.AttributeRouteInfo?.Name,
				Template = x.AttributeRouteInfo?.Template,
				Constraint = x.ActionConstraints
			}));
		}

		/// <summary>
		/// Returns the server time in UTC
		/// </summary>
		[HttpGet("time")]
		[ProducesResponse(HttpStatus.OK)]
		public ActionResult<DateTime> Time()
		{
			return Ok(DateTime.UtcNow);
		}

		/// <summary>
		/// Returns the server name
		/// </summary>
		/// <returns></returns>
		[HttpGet("name")]
		[ProducesResponse(HttpStatus.OK)]
		public ActionResult<string> Name()
		{
			return Ok(System.Environment.MachineName);
		}
		
		/// <summary>
		/// Helpful in determining stale files
		/// </summary>
		/// <returns></returns>
		[HttpGet("filedates")]
		[ProducesResponse(HttpStatus.OK)]
		public ActionResult<IEnumerable<FileDate>> FileDates()
		{
			return Ok(
				Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*")
					.Select(f => new FileInfo(f))
					.Select(fi => new FileDate{
						File = fi.Name,
						Modified = fi.LastWriteTimeUtc.ToShortDateString() + " " + fi.LastWriteTimeUtc.ToLongTimeString(),
						Size = fi.Length.ToString(),
					}));
		}

		/// <summary>
		/// Canary for error system. Throws an exception so that the exception management system may be checked
		/// </summary>
		[HttpGet("error")]
		public ActionResult Error()
		{
			throw new ApplicationException("Intentional exception. Testing error handling");
		}

		/// <summary>
		/// Echo's the environment variable keys only, not values
		/// </summary>
		[HttpGet("env")]
		[ProducesResponse(HttpStatus.OK)]
		public ActionResult<IEnumerable<string>> EnvironmentVariables()
		{
			var list = new List<string>();
			foreach(var k in System.Environment.GetEnvironmentVariables().Keys)
				list.Add(k.ToString());
			return list;
		}

        /// <summary>
        /// Echo's the Ioc Registrations back
        /// </summary>
        [HttpGet("ioc")]
        [ProducesResponse(HttpStatus.OK)]
        public ActionResult<IocRegistrations> Ioc()
        {
            var result = new IocRegistrations();

            foreach (var i in _container.GetCurrentRegistrations())
            {
                var reg = new IocRegistration
                {
                    Lifestyle = i.Lifestyle.Name,
                    Service = i.ServiceType.Name
                };

                if (i.Registration.ImplementationType != null)
                    reg.Implementation = i.Registration.ImplementationType.Name;

                result.Container.Add(reg);
            }

            foreach (var s in _services)
            {
                var reg = new IocRegistration
                {
                    Lifestyle = s.Lifetime.ToString(),
                    Service = s.ServiceType.Name
                };

                if (s.ImplementationType != null)
                    reg.Implementation = s.ImplementationType.Name;

                else if (s.ImplementationFactory != null)
                    reg.Implementation = s.ImplementationFactory.ToString();

                else if (s.ImplementationInstance != null)
                    reg.Implementation = $"Instance: {s.ImplementationInstance.GetType().Name}";

                result.ServiceCollection.Add(reg);
            }
            
            return result;
        }
    }
}
