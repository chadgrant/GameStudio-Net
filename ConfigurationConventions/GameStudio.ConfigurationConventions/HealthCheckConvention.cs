using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using GameStudio.Extensions;

namespace GameStudio.ConfigurationConventions
{
	/// <summary>
	/// Automatically Registers Health Checks via convention
	/// Name ends with "HealthCheck" and implements IHealthCheck
	/// </summary>
	public class HealthCheckConvention : IConfigureServices
	{
        readonly IEnumerable<Type> _types;
        readonly IHealthChecksBuilder _builder;

		public HealthCheckConvention(IEnumerable<Type> types, IHealthChecksBuilder builder)
		{
            _types = types;
            _builder = builder;
		}

		static Func<Type, bool> Convention => type => 
			!type.IsAbstract &&
			 type.Name.EndsWith("HealthCheck") &&
			 type.IsAssignableTo<IHealthCheck>();

		public void ConfigureServices(IConfiguration configuration, IServiceCollection services)
		{
                _types.Where(Convention)
				.ForEach(t =>
				{
					_builder.Add(ToRegistration(t));
					services.AddSingleton(t,t);
				});
		}

		HealthCheckRegistration ToRegistration(Type type)
		{
			return new HealthCheckRegistration(
				type.Name.Replace("HealthCheck", string.Empty),
				f => new HealthCheckTimer((IHealthCheck)f.GetService(type) ),
				null, null);
		}
	}
}
