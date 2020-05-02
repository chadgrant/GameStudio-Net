using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GameStudio.ConfigurationConventions
{
	public class RepositoryOptions: Dictionary<string, string[]>
	{
	}

	/// <summary>
	/// Registers / Wraps repositories with configured decorators
	/// Example:
	/// IThingRepository configured with :
	/// "Sql", "Cached" => new CachedThingRepository(new SqlThingRepository())
	/// </summary>
	public class RepositoryConvention : IConfigureServices
	{
        readonly IEnumerable<Type> _types;
        readonly RepositoryOptions _options;

		static readonly Func<Type, bool> InterfaceConvention = 
			type => type.IsInterface &&
			type.Name.EndsWith("Repository");

        static Func<Type, bool> RepoConvention(Type iface)
        {
            return type => !type.IsAbstract && iface.IsAssignableFrom(type);
        }

        public RepositoryConvention(IEnumerable<Type> types, RepositoryOptions options)
		{
            _types = types;
            _options = options;
		}

		public void ConfigureServices(IConfiguration configuration, IServiceCollection services)
		{
			var repoInterfaces = _types.Where(InterfaceConvention);

			foreach (var repoInteface in repoInterfaces)
			{
				var name = repoInteface.Name.Substring(1, repoInteface.Name.Length - 1);

				var repositories = _types.Where(RepoConvention(repoInteface))
					.ToDictionary(kv=>kv.Name.Replace(name, string.Empty),kv=>kv,StringComparer.OrdinalIgnoreCase);

                var repositoryNames = GetDecoratorOrder(name);

                if (!repositories.TryGetValue(repositoryNames.First(), out var main))
                {
                    throw new ApplicationException($"No repository found for {repositoryNames.First()}");
                }
                
				services.AddSingleton(repoInteface, main);

				foreach (var c in repositoryNames.Skip(1))
				{
					if (!repositories.TryGetValue(c, out var decorator))
					{
                        //TODO log not found
						continue;
					}
                    services.Decorate(repoInteface, decorator);
                }
			}
        }

        /// <summary>
        /// Looks up ordering of Repository Decorators
        /// </summary>
        string[] GetDecoratorOrder(string name)
		{
			if (_options.TryGetValue(name, out string[] values))
				return values;

			return _options["default"];
		}
	}
}
