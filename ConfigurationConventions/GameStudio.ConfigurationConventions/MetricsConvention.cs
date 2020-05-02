using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using GameStudio.Extensions;
using GameStudio.Metrics;

namespace GameStudio.ConfigurationConventions
{
	/// <summary>
	/// Metrics convention for a metric registry
	/// Registers the first Metrics Provider &amp; Factory it finds
	/// </summary>
	public class MetricsConvention : IConfigureServices
	{
        readonly IEnumerable<Type> _types;

        public MetricsConvention(IEnumerable<Type> types)
        {
            _types = types;
        }

		/// <summary>
		/// Find all classes that end with "Registry" and have a constructor that takes IMetricsFactory
		/// </summary>
		static readonly Func<Type, bool> RegistryConvention = 
			type => !type.IsAbstract && 
			type.IsClass && 
			type.Name.EndsWith("Registry") &&
			type.GetConstructor(new []{typeof(IMetricsFactory)}) != null;

		static readonly Func<Type, bool> FactoryConvention = 
			type => !type.IsAbstract &&
			type.IsAssignableTo<IMetricsFactory>();

		static readonly Func<Type, bool> ProviderConvention = 
			type => !type.IsAbstract &&
			type.IsAssignableTo<IMetricsProvider>();

		public void ConfigureServices(IConfiguration configuration, IServiceCollection services)
		{
			var provider = _types.Where(ProviderConvention).FirstOrDefault();
			var factory = _types.Where(FactoryConvention).FirstOrDefault();
			var registries = _types.Where(RegistryConvention);

			if (provider == null)
				return;

			if (factory == null)
				return;

			services.AddSingleton(typeof(IMetricsFactory), factory);

			foreach(var registry in registries)
				services.AddSingleton(registry);
		}
	}
}
