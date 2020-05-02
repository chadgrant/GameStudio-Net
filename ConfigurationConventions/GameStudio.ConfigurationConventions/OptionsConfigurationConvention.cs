using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Inflector;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GameStudio.ConfigurationConventions
{
	/// <summary>
	/// Maps configuration settings to IOption&lt;T&gt; with a convention
	/// </summary>
	public class OptionsConfigurationConvention : IConfigureServices
	{
        readonly IEnumerable<Type> _types;
        readonly IConfiguration _configuration;

		public OptionsConfigurationConvention(IEnumerable<Type> types, IConfiguration configuration)
		{
            _types = types;
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
		}

		/// <summary>
		/// Convention for finding types in assemblies to auto wire up with configurations.
		/// Default:
		///		Classes that are not abstract, with a default constructor and name ends in "Options"
		///		i.e. ThingOptions
		/// </summary>
		static Func<Type, bool> ConfigTypeConvention => t => 
			t.IsClass && 
			!t.IsAbstract && 
			t.GetConstructor(Type.EmptyTypes) != null &&
			t.Name.EndsWith("Options");

		/// <summary>
		/// Matches Types with names in configurations by convention.
		/// Default:
		///		ThingOptions type -> "ThingOptions", "Thing", "Things" key in configuration
		///		RepositoryOptions type -> "RepositoryOptions", "Repository" or "Repositories"
		/// </summary>
		static IConfigurationSection ConfigSectionConvention(IConfigurationSection section, Type type)
		{
			var names = new[] { type.Name, type.Name.Replace("Options", string.Empty), type.Name.Replace("Options", string.Empty).Pluralize() };

			return names.Any(n => section.Key.Equals(n, StringComparison.OrdinalIgnoreCase)) ? section : null;
		}

		/// <summary>
		/// Maps ServiceCollection.Configure extension method to a function that can pass a type at runtime.
		/// Official version requires generics which tightly couples dependencies
		/// </summary>
		static readonly MethodInfo ConfigureMethod = typeof(OptionsConfigurationServiceCollectionExtensions)
			.GetMethod(nameof(OptionsConfigurationServiceCollectionExtensions.Configure),
				BindingFlags.Public | BindingFlags.Static,
				null,
				new[] { typeof(IServiceCollection), typeof(IConfiguration) },
				null);

		/// <summary>
		/// Extension method to wrap the above method
		/// </summary>
		static void Configure(IServiceCollection services, IConfigurationSection config, Type type)
		{
			if (services == null) throw new ArgumentNullException(nameof(services));
			if (config == null) throw new ArgumentNullException(nameof(config));
			if (type == null) throw new ArgumentNullException(nameof(type));

			var generic = ConfigureMethod.MakeGenericMethod(type);

			generic.Invoke(services, new object[] { services, config });
		}

		/// <summary>
		/// Configures types from specified assemblies with options retrieved matched by the conventions detailed above
		/// </summary>
		public void ConfigureServices(IConfiguration configuration, IServiceCollection services)
		{
			if (services == null) throw new ArgumentNullException(nameof(services));

			var optionTypes = _types.Where(ConfigTypeConvention);

			_configuration
				.GetChildren()
				.Select(section =>
				{
					var type = optionTypes.FirstOrDefault(t => ConfigSectionConvention(section, t) != null);

					return new
					{
						section,
						type
					};
				})
				.Where(it => it.type != null)
				.ToList()
				.ForEach(it => Configure(services, it.section, it.type));
		}

	}
}
